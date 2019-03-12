using Rhino.Geometry;
using Rhino.Geometry.Intersect;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;


namespace PTK
{
    public class Assembly
    {
        // --- field ---
        public List<Element1D> Elements { get; private set; } = new List<Element1D>();
        public List<int> ElementID { get; private set; }=new List<int>();
        public int ElementIDCounter { get; private set; }
        public List<Node> Nodes { get; private set; } = new List<Node>();
        public List<int> NodeID { get; private set; } = new List<int>();
        public int NodeIDCounter { get; private set; }

        public List<string> Tags { get; private set; } = new List<string>();
        public List<CrossSection> CrossSections { get; private set; } = new List<CrossSection>();
        public Dictionary<Element1D, List<int>> NodeMap { get; private set; } = new Dictionary<Element1D, List<int>>();
        public List<Detail> Details { get; private set; } = new List<Detail>();
        public List<DetailingGroup> DetailingGroups { get; private set; } = new List<DetailingGroup>();
        public List<DetailingGroupRulesDefinition> DetailingGroupDefinitions { get; set; } = new List<DetailingGroupRulesDefinition>();
        public RTree ElementrTree { get; private set; } = new RTree();
        public RTree NoderTree { get; private set; } = new RTree();


        // --- constructors --- 
        public Assembly() { }
        public Assembly(Assembly _assembly)
        {
            ElementIDCounter = 0;
            NodeIDCounter = 0;
            Elements = _assembly.Elements;
            Nodes = _assembly.Nodes;
            Tags = _assembly.Tags;
            CrossSections = _assembly.CrossSections;
            NodeMap = _assembly.NodeMap;
            Details = _assembly.Details;
            DetailingGroups = _assembly.DetailingGroups;
            DetailingGroupDefinitions = _assembly.DetailingGroupDefinitions;
            
        }

        // --- methods ---
        public int AddElement(Element1D _element)
        {
            if (!Elements.Contains(_element))
            {

                //Adding the centerpoint of the element in an Rtree
                


                SearchNodes(_element);

                ElementID.Add(ElementIDCounter);
                ElementrTree.Insert(_element.BaseCurve.PointAtLength(_element.BaseCurve.GetLength() / 2), ElementIDCounter);
                ElementIDCounter++;

                Elements.Add(_element);
                string tag = _element.Tag;
                if (!Tags.Contains(tag))
                {
                    Tags.Add(tag);
                }

                if(_element.CrossSection is Composite comp)
                {
                    var secs = comp.RecursionCrossSectionSearch();
                    foreach(var sec in secs.ConvertAll(s=>s.Item1))
                    {
                        if (!CrossSections.Contains(sec))
                        {
                            CrossSections.Add(sec);
                        }
                    }
                }
                else
                {
                    CrossSection sec = _element.CrossSection;
                    if (!CrossSections.Contains(sec))
                    {
                        CrossSections.Add(sec);
                    }
                }
            }
            return Elements.Count;
        }

        private void SearchNodes(Element1D _element)
        {
            // Add a new Key if NodeMap doesn't contain "_element"
            if (!NodeMap.ContainsKey(_element))
            {
                NodeMap.Add(_element, new List<int>());
            }

            // Register both ends of the element as nodes
            AddPointToNodeMap(_element, _element.PointAtStart);
            AddPointToNodeMap(_element, _element.PointAtEnd);

            //Register intersection with other elements as a node

            double Length = _element.BaseCurve.GetLength();
            Sphere TempSphere = new Sphere(_element.BaseCurve.PointAtLength(Length/2),Length/2);

            List<double> nIds = new List<double>();
            bool nodeExists;

            EventHandler<RTreeEventArgs> ElementClose =
                (object sender, RTreeEventArgs args) =>
                {
                    nodeExists = true;
                    nIds.Add(args.Id);
                };


            ElementrTree.Search(TempSphere, ElementClose);


            for(int i = 0; i < nIds.Count; i++)
            {
                Element1D otherElem = Elements[i];
                if (otherElem.IsIntersectWithOther)
                {
                    //Check if both curves are linear

                    Curve OtherCurve = otherElem.BaseCurve;
                    Curve ThisCurve = _element.BaseCurve;

                    if (OtherCurve.IsLinear() && ThisCurve.IsLinear())
                    {
                        Line OtherLine = new Line(OtherCurve.PointAtStart, OtherCurve.PointAtStart);
                        Line ThisLine = new Line(ThisCurve.PointAtStart, ThisCurve.PointAtEnd);



                        //
                        double OtherParam;
                        double ThisParam;

                        var eventsLine = Intersection.LineLine(OtherLine, ThisLine, out OtherParam, out ThisParam, CommonProps.tolerances, false);

                        if (eventsLine)
                        {
                            Point3d tempPoint = OtherLine.PointAt(OtherParam);
                            AddPointToNodeMap(_element, tempPoint);
                            AddPointToNodeMap(otherElem, tempPoint);
                        }


                    }

                    else
                    {
                        var events = Intersection.CurveCurve(otherElem.BaseCurve, _element.BaseCurve, CommonProps.tolerances, CommonProps.tolerances);
                        if (events != null)
                        {
                            foreach (IntersectionEvent e in events)
                            {
                                if (!_element.IsIntersectWithOther)
                                //When it does not intersect with another member, only endpoint contact is detected
                                {
                                    if (e.PointA == _element.BaseCurve.PointAtStart || e.PointA == _element.BaseCurve.PointAtEnd)
                                    {
                                        AddPointToNodeMap(otherElem, e.PointA);
                                    }
                                }
                                else
                                // intersection happens
                                {
                                    AddPointToNodeMap(_element, e.PointA);
                                    AddPointToNodeMap(otherElem, e.PointA);
                                    if (e.IsOverlap)    //When overlap is an interval
                                    {
                                        AddPointToNodeMap(_element, e.PointA2);
                                        AddPointToNodeMap(otherElem, e.PointA2);
                                    }
                                }
                            }
                        }
                        else
                        {
                            continue;
                        }
                    }
                }

            }


        }

        private void AddPointToNodeMap(Element1D _element, Point3d _pt)
        {
            // When there is no node found at the position
            if (!Nodes.Exists(n => n.Equals(_pt)))
            {
                Node newNode = new Node(_pt);

                Nodes.Add(newNode);
                NodeMap[_element].Add(Nodes.Count-1);   //Attach a node to an element with a new ID
                NoderTree.Insert(_pt, NodeIDCounter);
                NodeID.Add(NodeIDCounter);
                NodeIDCounter++;

                Detail newDetail = new Detail(newNode);
                newDetail.AddElement(_element);

                Details.Add(newDetail);



            }
            //If there is already a node, map its index
            else
            {
                int ind = Nodes.FindIndex(n => n.Equals(_pt));
                if (!NodeMap[_element].Contains(ind))
                {
                    NodeMap[_element].Add(ind);
                    Details[ind].AddElement(_element);
                }
            }
        }

        public void GenerateDetails()
        {


            foreach (Node node in Nodes)
            {
                int ind = Nodes.IndexOf(node);
                List<Element1D> Elements = NodeMap.Where(p => p.Value.Contains(ind)).ToList().ConvertAll(p => p.Key);
                Details.Add(new Detail(node, Elements));
            }
        }

        public void AddDetailingGroup(DetailingGroup _dg)
        {
            DetailingGroups.Add(_dg);
        }

        public List<double> SearchNodeParamsAtElement(Element1D _element)
        {
            List<double> param = new List<double>();
            if (NodeMap.ContainsKey(_element))
            {
                foreach(int i in NodeMap[_element])
                {
                    _element.BaseCurve.ClosestPoint(Nodes[i].Point, out double p);
                    // remove same values
                    if (param.Count == 0)
                    {
                        param.Add(p);
                    }
                    else
                    {
                        // double closestParam = param.Min(c => Math.Abs(c - p));
                        double closestParam = param.Aggregate((x, y) => Math.Abs(x - p) < Math.Abs(y - p) ? x : y);
                        if (Math.Abs(closestParam - p) > CommonProps.tolerances)
                        {
                            param.Add(p);
                        }
                    }
                }
                param.Sort();
            }
            return param;
        }

        public Assembly DeepCopy()
        {
            return (Assembly)base.MemberwiseClone();
        }
        public override string ToString()
        {
            string info;
            info = "<Assembly>\n Elements:" + Elements.Count.ToString() + "\n" +
                " Nodes:" + Nodes.Count.ToString() + "\n" +
                " CrossSections:" + CrossSections.Count.ToString();
            return info;
        }
        public bool IsValid()
        {
            return Elements.Count != 0;
        }
    }

    public class StructuralAssembly : Assembly
    {
        // --- field ---
        public Assembly Assembly { get; private set; } = new Assembly();
        public List<Support> Supports { get; private set; } = new List<Support>();
        public List<Load> Loads { get; private set; } = new List<Load>();
        public Dictionary<Element1D, Force> ElementForce { get; private set; } = new Dictionary<Element1D, Force>();
        public Dictionary<Element1D, PTK_StructuralAnalysis> ElementReport { get; private set; } = new Dictionary<Element1D, PTK_StructuralAnalysis>();
        //public List<Karamba.Models.GH_Model> krmb_GH_Model { get; private set; } = new List<Karamba.Models.GH_Model>();


        // --- constructors ---
        public StructuralAssembly() { }
        public StructuralAssembly(Assembly _assembly) : base(_assembly)
        {
            //Assembly = _assembly;
            
        }

        // --- methods ---
        public int AddSupport(Support _support)
        {
            if (!Supports.Contains(_support))
            {
                Supports.Add(_support);
            }
            return Supports.Count;
        }

        public int AddLoad(Load _load)
        {
            if (!Loads.Contains(_load))
            {
                Loads.Add(_load);
            }
            return Loads.Count;
        }
        
        public new StructuralAssembly DeepCopy()
        {
            return (StructuralAssembly)base.MemberwiseClone();
        }
        public override string ToString()
        {
            string info;
            info = "<StructuralAssembly>\n"+
                " Supports:" + Supports.Count.ToString() + "\n" +
                " Loads:" + Loads.Count.ToString();
            return info;
        }
        //public new bool IsValid()
        //{
        //    return Assembly != null && Assembly.IsValid();
        //}
    }

    public class GH_Assembly : GH_Goo<Assembly>
    {
        public GH_Assembly() { }
        public GH_Assembly(GH_Assembly other) : base(other.Value) { this.Value = other.Value.DeepCopy(); }
        public GH_Assembly(Assembly ass) : base(ass) { this.Value = ass; }
        public override IGH_Goo Duplicate()
        {
            return new GH_Assembly(this);
        }
        public override bool IsValid => base.m_value.IsValid();
        public override string TypeName => "Assembly";
        public override string TypeDescription => "A model that gathers elements and has intersection points";
        public override string ToString()
        {
            return Value.ToString();
        }
        public override bool CastFrom(object source)
        {
            return base.CastFrom(source);
        }
        public override bool CastTo<Q>(ref Q target)
        {
            return base.CastTo(ref target);
        }
    }

    public class Param_Assembly : GH_PersistentParam<GH_Assembly>
    {
        public Param_Assembly() : base(new GH_InstanceDescription("Assembly", "Assembly", "A model that gathers elements and has intersection points", CommonProps.category, CommonProps.subcate0)) { }

        protected override System.Drawing.Bitmap Icon { get { return Properties.Resources.ParaAssemble; } }  //Set icon image

        public override Guid ComponentGuid => new Guid("E49369AA-4F29-498E-9808-E3197929FF51");

        protected override GH_GetterResult Prompt_Plural(ref List<GH_Assembly> values)
        {
            return GH_GetterResult.success;
        }

        protected override GH_GetterResult Prompt_Singular(ref GH_Assembly value)
        {
            return GH_GetterResult.success;
        }
    }

    public class GH_StructuralAssembly : GH_Goo<StructuralAssembly>
    {
        public GH_StructuralAssembly() { }
        public GH_StructuralAssembly(GH_StructuralAssembly other) : base(other.Value) { this.Value = other.Value.DeepCopy(); }
        public GH_StructuralAssembly(StructuralAssembly ass) : base(ass) { this.Value = ass; }
        public override IGH_Goo Duplicate()
        {
            return new GH_StructuralAssembly(this);
        }
        public override bool IsValid => base.m_value.IsValid();
        public override string TypeName => "Structural Assembly";
        public override string TypeDescription => "A model that gathers elements and has intersection points";
        public override string ToString()
        {
            return Value.ToString();
        }
        public override bool CastTo<Q>(ref Q target)
        {
            //if (typeof(Q).IsAssignableFrom(typeof(Assembly)))
            //{
            //    object ptr = Value;
            //    target = (Q)ptr;
            //    return true;
            //}
            if (typeof(Q).IsAssignableFrom(typeof(GH_Assembly)))
            {
                object ptr = new GH_Assembly(Value);
                target = (Q)ptr;
                return true;
            }
            return false;
            //return base.CastTo(ref target);
        }
    }

    public class Param_StructuralAssembly : GH_PersistentParam<GH_StructuralAssembly>
    {
        public Param_StructuralAssembly() : base(new GH_InstanceDescription("Structural Assembly", "Str Assembly", "A model that gathers elements and has intersection points", CommonProps.category, CommonProps.subcate0)) { }

        protected override System.Drawing.Bitmap Icon { get { return Properties.Resources.ParaStrAssemble; } }  //Set icon image

        public override Guid ComponentGuid => new Guid("4B468C32-EC87-47F8-A995-0832EDADEBA0");

        protected override GH_GetterResult Prompt_Plural(ref List<GH_StructuralAssembly> values)
        {
            return GH_GetterResult.success;
        }

        protected override GH_GetterResult Prompt_Singular(ref GH_StructuralAssembly value)
        {
            return GH_GetterResult.success;
        }
    }
}

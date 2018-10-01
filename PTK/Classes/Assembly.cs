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
        public List<Element1D> Elements { get; private set; }
        public List<Node> Nodes { get; private set; }
        public List<string> Tags { get; private set; }
        public List<CrossSection> CrossSections { get; private set; }
        public List<MaterialProperty> MaterialProperties { get; private set; }
        public Dictionary<Element1D,List<int>> NodeMap { get; private set; }
        public Dictionary<CrossSection, MaterialProperty> CrossSectionMap { get; private set; }
        public List<Detail> Details { get; private set; }
        public List<DetailingGroup> DetailingGroups { get; private set; }

        // --- constructors --- 
        public Assembly()
        {
            Elements = new List<Element1D>();
            Nodes = new List<Node>();
            Tags = new List<string>();
            CrossSections = new List<CrossSection>();
            MaterialProperties = new List<MaterialProperty>();
            NodeMap = new Dictionary<Element1D, List<int>>();
            CrossSectionMap = new Dictionary<CrossSection, MaterialProperty>();
            Details = new List<Detail>();
            DetailingGroups = new List<DetailingGroup>();
        }

        // --- methods ---
        public void GenerateDetails()
        {
            //Making Detail
            foreach(Node node in Nodes)
            {
                int ind = Nodes.IndexOf(node);
                List<Element1D> Elements = NodeMap.Where(p => p.Value.Contains(ind)).ToList().ConvertAll(p => p.Key);
                Details.Add(new Detail(node, Elements));
            }
        }

        public int AddElement(Element1D _element)
        {
            if (!Elements.Contains(_element))
            {
                // SearchNodes:
                SearchNodes(_element);

                Elements.Add(_element);
                string tag = _element.Tag;
                if (!Tags.Contains(tag))
                {
                    Tags.Add(tag);
                }
                // foreach(CrossSection sec in _element.CrossSections)
                for (int i=0; i< _element.CrossSections.Count; i++)
                {
                    CrossSection sec = _element.CrossSections[i];
                    if(!CrossSectionMap.ContainsKey(sec))
                    {
                        CrossSections.Add(sec);
                        // MaterialProperty matProp = sec.MaterialProperty;
                        MaterialProperty matProp = _element.Sub2DElements[i].MaterialProperty;

                        CrossSectionMap.Add(sec, matProp);
                        if (!MaterialProperties.Contains(matProp))
                        {
                            MaterialProperties.Add(matProp);
                        }
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
            foreach (Element1D otherElem in Elements.FindAll(e => e.IsIntersectWithOther == true))
            {
                var events = Intersection.CurveCurve(otherElem.BaseCurve, _element.BaseCurve, Rhino.RhinoDoc.ActiveDoc.ModelAbsoluteTolerance, 0.0);
                if(events != null)
                {
                    foreach(IntersectionEvent e in events)
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

        private void AddPointToNodeMap(Element1D _element, Point3d _pt)
        {
            // When there is no node found at the position
            if (!Nodes.Exists(n => n.Equals(_pt)))
            {
                // node location less than the tolerance
                Nodes.Add(new Node(_pt));
                NodeMap[_element].Add(Nodes.Count-1);
            }
            else // when there's an existent node
            {
                //If there is already a node, map its index
                int ind = Nodes.FindIndex(n => n.Point.Equals(_pt));
                if (!NodeMap[_element].Contains(ind))
                {
                    NodeMap[_element].Add(ind);
                }
            }
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
            info = "<Assembly> Elements:" + Elements.Count.ToString() +
                " Nodes:" + Nodes.Count.ToString() +
                " Material Properties:" + MaterialProperties.Count.ToString() +
                " Sections:" + CrossSections.Count.ToString();
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
        public Assembly Assembly { get; private set; }
        public List<Support> Supports { get; private set; }
        public List<Load> Loads { get; private set; }

        // --- constructors ---
        public StructuralAssembly()
        {
            Assembly = new Assembly();
            Supports = new List<Support>();
            Loads = new List<Load>();
        }
        public StructuralAssembly(Assembly _assembly)
        {
            Assembly = _assembly;
            Supports = new List<Support>();
            Loads = new List<Load>();
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
            info = "<StructuralAssembly>"+
                " Supports:" + Supports.Count.ToString() +
                " Loads:" + Loads.Count.ToString();
            return info;
        }
        public new bool IsValid()
        {
            return Assembly != null;
        }
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
        public override string TypeName => "StructuralAssembly";
        public override string TypeDescription => "A model that gathers elements and has intersection points";
        public override string ToString()
        {
            return Value.ToString();
        }
    }

    public class Param_StructuralAssembly : GH_PersistentParam<GH_StructuralAssembly>
    {
        public Param_StructuralAssembly() : base(new GH_InstanceDescription("StructuralAssembly", "StructuralAssembly", "A model that gathers elements and has intersection points", CommonProps.category, CommonProps.subcate0)) { }

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

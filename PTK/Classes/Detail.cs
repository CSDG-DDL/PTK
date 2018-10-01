using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry;
using System.Linq;
using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;

namespace PTK
{
    public class Detail
    {
        // --- field ---
        public Node Node { get; private set; }
        public List<Element1D> Elements { get; private set; }
        public List<Vector3d> UnifiedVectors { get; private set; }
        public Dictionary<Element1D, int> ElementsPriorityMap { get; private set; } = new Dictionary<Element1D, int>();
        public DetailType Type { get; private set; }

        // --- constructors --- 
        public Detail()
        {
            Node = new Node();
            Elements = new List<Element1D>();
        }
        public Detail(Node _node)
        {
            Node = _node;
            Elements = new List<Element1D>();
        }
        public Detail(Node _node, List<Element1D> _elements)
        {
            Node = _node;
            Elements = _elements;
        }

        // --- methods ---
        public bool SetElements(List<Element1D> _elements, List<string> _priority)
        {
            List<Element1D> crossElements = _elements.FindAll(e => !IsNodeEndPointAtElement(e));
            List<Element1D> cornerElements = _elements.FindAll(e => IsNodeEndPointAtElement(e));

            if (crossElements.Count >= 2)
            {
                Type = DetailType.XType;
                if (!SortElementsByPriority(ref crossElements, _priority))
                {
                    return false;
                }
            }
            else if (crossElements.Count == 1)
            {
                Type = DetailType.TType;
            }
            else
            {
                Type = DetailType.LType;
            }
            if (cornerElements.Count >= 2)
            {
                if (!SortElementsByPriority(ref cornerElements, _priority))
                {
                    return false;
                }
            }

            string preTag = "";
            int priorityIndex = 0;
            foreach (Element1D e in crossElements)
            {
                if (preTag != e.Tag)
                {
                    preTag = e.Tag;
                    priorityIndex++;
                }
                ElementsPriorityMap[e] = priorityIndex;
            }
            preTag = "";    //When CrossElement and CornerElement are the same tag
            foreach (Element1D e in cornerElements)
            {
                if (preTag != e.Tag)
                {
                    preTag = e.Tag;
                    priorityIndex++;
                }
                ElementsPriorityMap[e] = priorityIndex;
            }
            return true;
        }

        public void GenerateUnifiedElementVectors()
        {
            UnifiedVectors = new List<Vector3d>();
            foreach (Element1D element in Elements)
            {
                double DistanceElemStart = Node.Point.DistanceTo(element.BaseCurve.PointAtStart);
                double DistanceElemEnd = Node.Point.DistanceTo(element.BaseCurve.PointAtEnd);
                element.BaseCurve.Reverse();
                if (DistanceElemStart < DistanceElemEnd)
                {
                    UnifiedVectors.Add(-element.BaseCurve.TangentAtStart);
                }
                else
                {
                    UnifiedVectors.Add(-element.BaseCurve.TangentAtEnd);
                }
            }
        }

        private bool SortElementsByPriority(ref List<Element1D> _elements, List<string> _priority)
        {
            //When rearranging by priority is not necessary
            if (_elements.Count <= 1 || _elements.ConvertAll(e => e.Tag).Distinct().Count() <= 1)
            {
                return true;
            }
            //Whether all Elements to be prioritized are input with priority
            if (_elements.ConvertAll(e => e.Tag).Except(_priority).Count() == 0)
            {
                _elements = _elements.OrderBy(e => _priority.IndexOf(e.Tag)).ToList();
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool IsNodeEndPointAtElement(Element1D _element)
        {
            if (Node.Equals(_element.BaseCurve.PointAtStart) ||
                Node.Equals(_element.BaseCurve.PointAtEnd) )
            {
                return true;
            }
            return false;
        }

        public double SearchNodeParamAtElement(Element1D _element)
        {
            _element.BaseCurve.ClosestPoint(Node.Point, out double p);
            return p;
        }

        public Detail DeepCopy()
        {
            return (Detail)base.MemberwiseClone();
        }
        public override string ToString()
        {
            string info;
            info = "<Detail>\n" +
                " Node:" + Node.Point.ToString() +
                " Elements:" + Elements.Count +
                " Type:" + Type.ToString();
            return info;
        }
        public bool IsValid()
        {
            return Node.IsValid() && Elements.Count != 0;
        }
    }

    public class GH_Detail : GH_Goo<Detail>
    {
        public GH_Detail() { }
        public GH_Detail(GH_Detail other) : base(other.Value) { this.Value = other.Value.DeepCopy(); }
        public GH_Detail(Detail detail) : base(detail) { this.Value = detail; }
        public override bool IsValid => base.m_value.IsValid();

        public override string TypeName => "Detail";

        public override string TypeDescription => "Set of Node and its adjacent element";

        public override IGH_Goo Duplicate()
        {
            return new GH_Detail(this);
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }

    public class Param_Detail : GH_PersistentParam<GH_Detail>
    {
        public Param_Detail() : base(new GH_InstanceDescription("Detail", "Detail", "Set of Node and its adjacent element", CommonProps.category, CommonProps.subcate0)) { }

        protected override System.Drawing.Bitmap Icon { get { return Properties.Resources.ParaDetail; } }  //Set icon image

        public override Guid ComponentGuid => new Guid("16494CE0-36A7-42D0-A033-4DFE936A05B2");

        protected override GH_GetterResult Prompt_Plural(ref List<GH_Detail> values)
        {
            return GH_GetterResult.success;
        }

        protected override GH_GetterResult Prompt_Singular(ref GH_Detail value)
        {
            return GH_GetterResult.success;
        }
    }




    public class ElementInDetail
    {
        public Element1D Element { get; private set; }
        public Vector3d UnifiedVector { get; private set; }


        public ElementInDetail(Element1D _element, Vector3d _unifiedVector)
        {
            Element = _element;
            UnifiedVector = _unifiedVector;
        }

        public ElementInDetail()
        {

        }
 
    }
    
}

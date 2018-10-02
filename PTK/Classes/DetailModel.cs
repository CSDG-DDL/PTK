//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Grasshopper.Kernel;
//using Grasshopper.Kernel.Types;

//namespace PTK
//{
//    public class DetailModel
//    {
//        public Assembly Assembly { get; private set; }
//        public List<Detail> Details { get; private set; }

//        public DetailModel()
//        {
//            Assembly = new Assembly();
//            Details = new List<Detail>();
//        }
//        public DetailModel(Assembly _assembly)
//        {
//            Assembly = _assembly;
//            Details = new List<Detail>();
//        }
//    }

//    public class PriorityModel
//    {
//        public Assembly Assembly { get; private set; }
//        public List<Detail> Details { get; private set; }
//        public List<string> Priority { get; private set; }

//        public PriorityModel()
//        {
//            Assembly = new Assembly();
//            Details = new List<Detail>();
//            Priority = new List<string>();
//        }
//        public PriorityModel(Assembly _assembly)
//        {
//            Assembly = _assembly;
//            Details = new List<Detail>();
//            Priority = new List<string>();
//        }

//        public void SetPriority(string _priorityText)
//        {
//            Priority = _priorityText.Split(',').ToList();
//        }

//        public bool SearchDetails()
//        {
//            foreach(Node n in Assembly.Nodes)
//            {
//                int ind = Assembly.Nodes.IndexOf(n);
//                //List of elements belonging to the specified node
//                List<Element1D> detailElems = Assembly.NodeMap.Where(p => p.Value.Contains(ind)).ToList().ConvertAll(p => p.Key);
//                Details.Add(new Detail(n));
//                if(!Details.Last().SetElements(detailElems, Priority))
//                {
//                    return false;
//                }
//            }
//            return true;
//        }

//        public PriorityModel DeepCopy()
//        {
//            return (PriorityModel)base.MemberwiseClone();
//        }
//        public override string ToString()
//        {
//            string info;
//            info = "<PriorityModel> Details:" + Details.Count.ToString();
//            return info;
//        }
//        public bool IsValid()
//        {
//            return Details.Count != 0;
//        }
//    }



//    public class GH_PriorityModel : GH_Goo<PriorityModel>
//    {
//        public GH_PriorityModel() { }
//        public GH_PriorityModel(GH_PriorityModel other) : base(other.Value) { this.Value = other.Value.DeepCopy(); }
//        public GH_PriorityModel(PriorityModel ass) : base(ass) { this.Value = ass; }
//        public override IGH_Goo Duplicate()
//        {
//            return new GH_PriorityModel(this);
//        }
//        public override bool IsValid => base.m_value.IsValid();
//        public override string TypeName => "PriorityModel";
//        public override string TypeDescription => "A model prioritized for mating members";
//        public override string ToString()
//        {
//            return Value.ToString();
//        }
//    }

//    public class Param_PriorityModel : GH_PersistentParam<GH_PriorityModel>
//    {
//        public Param_PriorityModel() : base(new GH_InstanceDescription("PriorityModel", "PriorityModel", "A model prioritized for mating members", CommonProps.category, CommonProps.subcate0)) { }

//        protected override System.Drawing.Bitmap Icon { get { return null; } }  //Set icon image

//        public override Guid ComponentGuid => new Guid("0A29F71C-B30D-4244-82CB-1A5ADCD38FC6");

//        protected override GH_GetterResult Prompt_Plural(ref List<GH_PriorityModel> values)
//        {
//            return GH_GetterResult.success;
//        }

//        protected override GH_GetterResult Prompt_Singular(ref GH_PriorityModel value)
//        {
//            return GH_GetterResult.success;
//        }
//    }
//}

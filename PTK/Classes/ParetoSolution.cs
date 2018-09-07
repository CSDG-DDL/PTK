using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Newtonsoft.Json;
using GH_IO.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace PTK
{
    [Serializable]
    public class ParetoSolution
    {
        #region fields
        public Dictionary<string,decimal> Positions { get; private set; }
        public Dictionary<string,decimal> Fitness { get; private set; }
        #endregion

        #region constructors
        public ParetoSolution()
        {
            Positions = new Dictionary<string, decimal>();
            Fitness = new Dictionary<string, decimal>();
        }
        #endregion

        #region methods
        public void AddPosition(string _name,decimal _val)
        {
            Positions[_name] = _val;
        }
        public void AddFitness(string _name, decimal _val)
        {
            Fitness[_name] = _val;
        }

        public string ToJSON()
        {
            Dictionary<string, decimal> tempMap = new Dictionary<string, decimal>(Positions);
            foreach(KeyValuePair<string,decimal> kvp in Fitness)
            {
                tempMap[kvp.Key] = kvp.Value;
            }
            string json = JsonConvert.SerializeObject(tempMap);
            return json;
        }

        public ParetoSolution DeepCopy()
        {
            return (ParetoSolution)base.MemberwiseClone();
        }
        public override string ToString()
        {
            string info = "<ParetoSolution>\nPositions => ";
            foreach(KeyValuePair<string, decimal> pos in Positions)
            {
                info += pos.Key + ":" + pos.Value.ToString() + ", ";
            }
            info += "\nFitness => ";
            foreach (KeyValuePair<string, decimal> fit in Fitness)
            {
                info += fit.Key + ":" + fit.Value.ToString() + ", ";
            }
            return info;
        }
        public bool IsValid()
        {
            return Positions.Count != 0 && Fitness.Count != 0;
        }
        #endregion

    }

    public class GH_ParetoSolution : GH_Goo<ParetoSolution>
    {
        public GH_ParetoSolution() { }
        public GH_ParetoSolution(GH_ParetoSolution other) : base(other.Value) { this.Value = other.Value.DeepCopy(); }
        public GH_ParetoSolution(ParetoSolution p) : base(p) { this.Value = p; }
        public override bool IsValid => base.m_value.IsValid();

        public override string TypeName => "ParetoSolution";

        public override string TypeDescription => "ParetoSolution";

        public override IGH_Goo Duplicate()
        {
            return new GH_ParetoSolution(this);
        }

        public override string ToString()
        {
            return Value.ToString();
        }
        public override bool Write(GH_IWriter writer)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            var stream = new MemoryStream();
            formatter.Serialize(stream, Value);
            writer.SetByteArray("pareto", 0, stream.ToArray());
            return base.Write(writer);
        }

        public override bool Read(GH_IReader reader)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            var stream = new MemoryStream();

            byte[] raw = reader.GetByteArray("pareto",0);

            stream.Write(raw, 0, raw.Length);
            stream.Position = 0;
            Value = formatter.Deserialize(stream) as ParetoSolution;
            return base.Read(reader);
        }

    }

    public class Param_ParetoSolution : GH_PersistentParam<GH_ParetoSolution>
    {
        public Param_ParetoSolution() : base(new GH_InstanceDescription("ParetoSolution", "Pareto", "A set of parameter values derived as one of the best solutions for multiobjective optimization.", CommonProps.category, CommonProps.subcate0)) { }

        protected override System.Drawing.Bitmap Icon { get { return Properties.Resources.Pareto; } }  //クラスにアイコンを付けたい場合はここ

        public override Guid ComponentGuid => new Guid("a782682d-9eaf-48c0-8275-41c9b2f0e9b2");

        protected override GH_GetterResult Prompt_Plural(ref List<GH_ParetoSolution> values)
        {
            return GH_GetterResult.success;
        }

        protected override GH_GetterResult Prompt_Singular(ref GH_ParetoSolution value)
        {
            return GH_GetterResult.success;
        }

    }

    public class DisassembleParetoSolution : GH_Component
    {
        public DisassembleParetoSolution()
          : base("DisassembleParetoSolution", "DisassPareto",
              "Description",
              CommonProps.category, CommonProps.subcate8)
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddParameter(new Param_ParetoSolution(), "ParetoSolution", "P", "ParetoSolution", GH_ParamAccess.item);
            pManager[0].Optional = true;
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("PositionNames", "PN", "PositionNames", GH_ParamAccess.list);
            pManager.AddTextParameter("FitnessNames", "FN", "FitnessNames", GH_ParamAccess.list);
            pManager.AddNumberParameter("Positions", "P", "Positions", GH_ParamAccess.list);
            pManager.AddNumberParameter("Fitnesss", "F", "Fitnesss", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            GH_ParetoSolution gParetoSolution = null;

            if(!DA.GetData(0,ref gParetoSolution)) { return; }
            ParetoSolution paretoSolution = gParetoSolution.Value;

            List<string> positionNames = paretoSolution.Positions.Keys.ToList();
            List<string> fitnessNames = paretoSolution.Fitness.Keys.ToList();
            List<decimal> positions = paretoSolution.Positions.Values.ToList();
            List<decimal> fitnesss = paretoSolution.Fitness.Values.ToList();

            DA.SetDataList(0, positionNames);
            DA.SetDataList(1, fitnessNames);
            DA.SetDataList(2, positions);
            DA.SetDataList(3, fitnesss);

        }

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return null;
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("00a684e6-0e70-4206-8f45-ebc9073f0efc"); }
        }
    }
}

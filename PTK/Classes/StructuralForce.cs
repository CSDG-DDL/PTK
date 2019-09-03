using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;

namespace PTK
{
    public class StructuralForce
    {
        // --- field ---
        public List<Force> forces { get; set; }
        public MaxCompression maxCompressionForce { get; set; }
        public MaxTension maxTensionForce { get; set; }
        public MaxShearDir1 maxShearDir1 { get; set; }
        public MaxShearDir2 maxShearDir2 { get; set; }
        public MaxBendingDir1 maxBendingDir1 { get; set; }
        public MaxBendingDir2 maxBendingDir2 { get; set; }
        public MaxTorsion maxTorsion { get; set; }

        // --- constructors --- 
        
        public StructuralForce()
        {
            // empty class

        }
        public StructuralForce(
            List<Force> _forces
            )
        {
            // only list of forces
            forces = _forces;
        }

        // --- methods ---
        public StructuralForce DeepCopy()
        {
            return (StructuralForce)base.MemberwiseClone();
        }
        public override string ToString()
        {
            string info;
            info = "<StructuralData> StructuralForce";
            return info;
        }
        public bool IsValid()
        {
            return true;
        }
    }

    public class GH_StructuralForce : GH_Goo<StructuralForce>
    {
        public GH_StructuralForce() { }
        public GH_StructuralForce(GH_StructuralForce other) : base(other.Value) { this.Value = other.Value.DeepCopy(); }
        public GH_StructuralForce(StructuralForce sec) : base(sec) { this.Value = sec; }
        public override bool IsValid => base.m_value.IsValid();

        public override string TypeName => "Force";

        public override string TypeDescription => "Force";

        public override IGH_Goo Duplicate()
        {
            return new GH_StructuralForce(this);
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }

    public class Param_StructuralForce : GH_PersistentParam<GH_StructuralForce>
    {
        public Param_StructuralForce() : base(new GH_InstanceDescription("StructuralForce", "StructuralForce", "StructuralForce", CommonProps.category, CommonProps.subcate0)) { }

        protected override System.Drawing.Bitmap Icon { get { return Properties.Resources.SF; } }  //Set icon image

        public override Guid ComponentGuid => new Guid("FB9C3075-110A-424E-AC7B-E515303D8A2F");

        protected override GH_GetterResult Prompt_Plural(ref List<GH_StructuralForce> values)
        {
            return GH_GetterResult.success;
        }

        protected override GH_GetterResult Prompt_Singular(ref GH_StructuralForce value)
        {
            return GH_GetterResult.success;
        }
    }


}
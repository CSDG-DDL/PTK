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
    public class StructuralData
    {
        // --- field ---
        public StructuralForce StructuralForces;
        public StructuralResult StructuralResults;

        // --- properties of the element without FEM ---
        public double effectiveLength1;
        public double effectiveLength2;

        public double slendernessRatio1;
        public double slendernessRatio2;

        public double eulerForce1;
        public double eulerForce2;

        public double slendernessRelative1;
        public double slendernessRelative2;

        public double instabilityFactor1;
        public double instabilityFactor2;

        public double BucklingStrength1;
        public double BucklingStrength2;

        // --- constructors --- 

        public StructuralData()
        {
            // empty class

        }

        // --- methods ---
        public StructuralData DeepCopy()
        {
            return (StructuralData)base.MemberwiseClone();
        }

        public bool IsValid()
        {
            return true;
        }
    }

        public class GH_StructuralData : GH_Goo<StructuralData>
        {
            public GH_StructuralData() { }
            public GH_StructuralData(GH_StructuralData other) : base(other.Value) { this.Value = other.Value.DeepCopy(); }
            public GH_StructuralData(StructuralData sec) : base(sec) { this.Value = sec; }
            public override bool IsValid => base.m_value.IsValid();

            public override string TypeName => "Force";

            public override string TypeDescription => "Force";

            public override IGH_Goo Duplicate()
            {
                return new GH_StructuralData(this);
            }

            public override string ToString()
            {
                return Value.ToString();
            }
        }

        public class Param_StructuralData : GH_PersistentParam<GH_StructuralData>
        {
            public Param_StructuralData() : base(new GH_InstanceDescription("StructuralData", "StructuralData", "StructuralData", CommonProps.category, CommonProps.subcate0)) { }

            protected override System.Drawing.Bitmap Icon { get { return Properties.Resources.Force; } }  //Set icon image

            public override Guid ComponentGuid => new Guid("FB9C3025-220A-424E-AC7B-E515301D8A2F");

            protected override GH_GetterResult Prompt_Plural(ref List<GH_StructuralData> values)
            {
                return GH_GetterResult.success;
            }

            protected override GH_GetterResult Prompt_Singular(ref GH_StructuralData value)
            {
                return GH_GetterResult.success;
            }
        }

 }
    

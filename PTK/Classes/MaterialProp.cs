using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;

namespace PTK
{
    public class MaterialProperty
    {
        // --- field ---
        public string Name { get; set; } = "N/A";
        public double Fmgk { get; set; }
        public double Ft0gk { get; set; }
        public double Ft90gk { get; set; }
        public double Fc0gk { get; set; }
        public double Fc90gk { get; set; }
        public double Fvgk { get; set; }
        public double Frgk { get; set; }
        public double EE0gmean { get; set; }
        public double EE0g05 { get; set; }
        public double EE90gmean { get; set; }
        public double EE90g05 { get; set; }
        public double GGgmean { get; set; }
        public double GGg05 { get; set; }
        public double GGrgmean { get; set; }
        public double GGrg05 { get; set; }
        public double Rhogk { get; set; }
        public double Rhogmean { get; set; }

        // --- constructors --- 
        public MaterialProperty() { }
        public MaterialProperty(string _name)
        {
            Name = _name;
        }
        public MaterialProperty(
            string _name,
            double _fmgk,
            double _ft0gk,
            double _ft90gk,
            double _fc0gk,
            double _fc90gk,
            double _fvgk,
            double _frgk,
            double _e0gmean,
            double _e0g05,
            double _e90gmean,
            double _e90g05,
            double _ggmean,
            double _gg05,
            double _grgmean,
            double _grg05,
            double _rhogk,      // density
            double _rhogmean    // density
        )
        {
            Name = _name; 
            Fmgk = _fmgk;
            Ft0gk = _ft0gk;
            Ft90gk = _ft90gk;
            Fc0gk = _fc0gk;
            Fc90gk = _fc90gk;
            Fvgk = _fvgk;
            Frgk = _frgk;
            EE0gmean = _e0gmean;
            EE0g05 = _e0g05;
            EE90gmean = _e90gmean;
            EE90g05 = _e90g05;
            GGgmean = _ggmean;
            GGg05 = _gg05;
            GGrgmean = _grgmean;
            GGrg05 = _grg05;
            Rhogk = _rhogk;          // density
            Rhogmean = _rhogmean;    // density
        }

        // --- methods ---
        public MaterialProperty DeepCopy()
        {
            return (MaterialProperty)base.MemberwiseClone();
        }
        public override string ToString()
        {
            string info;
            info = "<MaterialProperty> Name:" + Name;
            return info;
        }

        public bool IsValid()
        {
            return Name != "N/A";
        }
    }

    public class GH_MaterialProperty : GH_Goo<MaterialProperty>
    {
        public GH_MaterialProperty() { }
        public GH_MaterialProperty(GH_MaterialProperty other) : base(other.Value)
        {
            this.Value = other.Value.DeepCopy();
        }
        public GH_MaterialProperty(MaterialProperty MSProp) : base(MSProp)
        {
            this.Value = MSProp;
        }

        public override bool IsValid => base.m_value.IsValid();

        public override string TypeName => "GH_MaterialProperty";

        public override string TypeDescription => "Material properties for structural calculation";

        public override IGH_Goo Duplicate()
        {
            return new GH_MaterialProperty(this);
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }

    public class Param_MaterialProperty : GH_PersistentParam<GH_MaterialProperty>
    {
        public Param_MaterialProperty() : base(new GH_InstanceDescription("MaterialProperty", "MatStruct", "Material properties for structural calculation", CommonProps.category, CommonProps.subcate0)) { }

        protected override System.Drawing.Bitmap Icon { get { return Properties.Resources.ParaMaterial; } }  //Set icon image

        public override Guid ComponentGuid => new Guid("248D46D3-6995-4CE8-AAD6-D30E99E6C493");

        protected override GH_GetterResult Prompt_Plural(ref List<GH_MaterialProperty> values)
        {
            return GH_GetterResult.success;
        }

        protected override GH_GetterResult Prompt_Singular(ref GH_MaterialProperty value)
        {
            return GH_GetterResult.success;
        }
    }


}

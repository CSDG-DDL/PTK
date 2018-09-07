using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;

namespace PTK
{
    public class Support
    {
        public string Tag { get; private set; }
        public int LoadCase { get; private set; } = 0;
        public Plane FixingPlane { get; private set; }
        public List<bool> Conditions { get; private set; }

        public Support()
        {
            Tag = "N/A";
            FixingPlane = new Plane();
            Conditions = new List<bool>();
        }
        public Support(string _tag, int _loadCase, Plane _fixingPlane, List<bool> _conditions) 
        {
            Tag = _tag;
            LoadCase = _loadCase;
            FixingPlane = _fixingPlane;
            Conditions = _conditions;
        }

        public void UpdateConditions(List<bool> _conditions)
        {
            this.Conditions = _conditions;
        }

        public static bool[] StringToArray(string _boolStr)
        {
            List<bool> _returnArray = new List<bool>();
            char[] _tempChars;
            _tempChars = _boolStr.ToCharArray();

            foreach(char c in _tempChars)
            {
                if (c == '0')
                {
                    _returnArray.Add(false);
                }
                else
                {
                    _returnArray.Add(true);
                }
            }
            return _returnArray.ToArray();
        }
        public Support DeepCopy()
        {
            return (Support)base.MemberwiseClone();
        }
        public override string ToString()
        {
            string info;
            info = "<Support> LoadCase:" + LoadCase.ToString() +
                " FixingPlane:" + FixingPlane.Origin.ToString() +
                " Conditions:" + Conditions.ToArray().ToString();
            return info;
        }
        public bool IsValid()
        {
            return true;
        }
    }

    public class GH_Support : GH_Goo<Support>
    {
        public GH_Support() { }
        public GH_Support(GH_Support other) : base(other.Value) { this.Value = other.Value.DeepCopy(); }
        public GH_Support(Support ass) : base(ass) { this.Value = ass; }
        public override IGH_Goo Duplicate()
        {
            return new GH_Support(this);
        }
        public override bool IsValid => base.m_value.IsValid();
        public override string TypeName => "Support";
        public override string TypeDescription => "The plane of the fixed point and the constraint condition";
        public override string ToString()
        {
            return Value.ToString();
        }
    }

    public class Param_Support : GH_PersistentParam<GH_Support>
    {
        public Param_Support() : base(new GH_InstanceDescription("Support", "Spp", "The plane of the fixed point and the constraint condition", CommonProps.category, CommonProps.subcate0)) { }

        protected override System.Drawing.Bitmap Icon { get { return Properties.Resources.Support; } }  //Set icon image

        public override Guid ComponentGuid => new Guid("6426D65C-7B2A-4B08-9A58-5547B627C7B0");

        protected override GH_GetterResult Prompt_Plural(ref List<GH_Support> values)
        {
            return GH_GetterResult.success;
        }

        protected override GH_GetterResult Prompt_Singular(ref GH_Support value)
        {
            return GH_GetterResult.success;
        }
    }
}

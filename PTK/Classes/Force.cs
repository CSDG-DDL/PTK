﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;

namespace PTK
{
    public class Force
    {
        #region fields
        public int LoadCase { get; private set; } = 0;
        public double FX { get; private set; }
        public double FY { get; private set; }
        public double FZ { get; private set; }
        public double MX { get; private set; }
        public double MY { get; private set; }
        public double MZ { get; private set; }
        #endregion

        #region constructors
        public Force()
        {
            FX = 0.0;
            FY = 0.0;
            FZ = 0.0;
            MX = 0.0;
            MY = 0.0;
            MZ = 0.0;
        }
        public Force(int _loadCase, double _fx, double _fy, double _fz, double _mx, double _my, double _mz)
        {
            LoadCase = _loadCase;
            FX = _fx;
            FY = _fy;
            FZ = _fz;
            MX = _mx;
            MY = _my;
            MZ = _mz;
        }
        #endregion

        #region properties
        #endregion

        #region methods
        public Force DeepCopy()
        {
            return (Force)base.MemberwiseClone();
        }
        public override string ToString()
        {
            string info;
            info = "<Force> FX:" + FX.ToString() +
                " FY:" + FY.ToString() +
                " FZ:" + FZ.ToString() +
                " MX:" + MX.ToString() +
                " MY:" + MY.ToString() +
                " MZ:" + MZ.ToString();
            return info;
        }
        public bool IsValid()
        {
            return true;
        }
        #endregion
    }

    public class GH_Force : GH_Goo<Force>
    {
        public GH_Force() { }
        public GH_Force(GH_Force other) : base(other.Value) { this.Value = other.Value.DeepCopy(); }
        public GH_Force(Force sec) : base(sec) { this.Value = sec; }
        public override bool IsValid => base.m_value.IsValid();

        public override string TypeName => "Force";

        public override string TypeDescription => "Force";

        public override IGH_Goo Duplicate()
        {
            return new GH_Force(this);
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }

    public class Param_Force : GH_PersistentParam<GH_Force>
    {
        public Param_Force() : base(new GH_InstanceDescription("Force", "Force", "Force", CommonProps.category, CommonProps.subcate0)) { }

        protected override System.Drawing.Bitmap Icon { get { return null; } }  //クラスにアイコンを付けたい場合はここ

        public override Guid ComponentGuid => new Guid("FB9C3075-220A-424E-AC7B-E515303D8A2F");

        protected override GH_GetterResult Prompt_Plural(ref List<GH_Force> values)
        {
            return GH_GetterResult.success;
        }

        protected override GH_GetterResult Prompt_Singular(ref GH_Force value)
        {
            return GH_GetterResult.success;
        }
    }
}

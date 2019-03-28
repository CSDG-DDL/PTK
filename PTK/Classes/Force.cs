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
    public class Force
    {
        // --- field ---
        public double Max_Fx_compression { get; set; }
        public double Max_Fx_tension { get; set; }
        public double Max_Fy_shear { get; set; }
        public double Max_Fz_shear { get; set; }
        public double Max_Mx_torsion { get; set; }
        public double Max_My_bending { get; set; }
        public double Max_Mz_bending { get; set; }

        public Point3d Position_Max_Fx_compression { get; set; }
        public Point3d Position_Max_Fx_tension { get; set; }
        public Point3d Position_Max_Fy_shear { get; set; }
        public Point3d Position_Max_Fz_shear { get; set; }
        public Point3d Position_Max_Mx_torsion { get; set; }
        public Point3d Position_Max_My_bending { get; set; }
        public Point3d Position_Max_Mz_bending { get; set; }

        public int Loadcase_Max_Fx_compression { get; set; }
        public int Loadcase_Max_Fx_tension { get; set; }
        public int Loadcase_Max_Fy_shear { get; set; }
        public int Loadcase_Max_Fz_shear { get; set; }
        public int Loadcase_Max_Mx_torsion { get; set; }
        public int Loadcase_Max_My_bending { get; set; }
        public int Loadcase_Max_Mz_bending { get; set; }


        public List<double> FXc { get; set; }
        public List<double> FXt { get; set; }
        public List<double> FY { get; set; }
        public List<double> FZ { get; set; }
        public List<double> MX { get; set; }
        public List<double> MY { get; set; }
        public List<double> MZ { get; set; }

        // --- constructors --- 
        #region constructors
        public Force()
        {
            // empty class

        }
        public Force(
            List<double> _fxc,
            List<double> _fxt,
            List<double> _fy,
            List<double> _fz,
            List<double> _mx,
            List<double> _my,
            List<double> _mz
            )
        {
            // only list of forces
            FXc = _fxc;
            FXt = _fxc;
            FY = _fy;
            FZ = _fz;
            MX = _mx;
            MY = _my;
            MZ = _mz;
        }
        public Force(
            List<double> _fxc,
            List<double> _fxt,
            List<double> _fy,
            List<double> _fz,
            List<double> _mx,
            List<double> _my,
            List<double> _mz,
            int _loadcase_max_Fx_compression,
            int _loadcase_max_Fx_tension,
            int _loadcase_max_Fy_shear,
            int _loadcase_max_Fz_shear,
            int _loadcase_max_Mx_torsion,
            int _loadcase_max_My_bending,
            int _loadcase_max_Mz_bending
            )
        {
            // only list of forces
            FXc = _fxc;
            FXt = _fxc;
            FY = _fy;
            FZ = _fz;
            MX = _mx;
            MY = _my;
            MZ = _mz;
            Loadcase_Max_Fx_compression = _loadcase_max_Fx_compression;
            Loadcase_Max_Fx_tension = _loadcase_max_Fx_tension;
            Loadcase_Max_Fy_shear = _loadcase_max_Fy_shear;
            Loadcase_Max_Fz_shear = _loadcase_max_Fz_shear;
            Loadcase_Max_Mx_torsion = _loadcase_max_Mx_torsion;
            Loadcase_Max_My_bending = _loadcase_max_My_bending;
            Loadcase_Max_Mz_bending = _loadcase_max_Mz_bending;
        }

        #endregion

        // --- methods ---
        public Force DeepCopy()
        {
            return (Force)base.MemberwiseClone();
        }
        public override string ToString()
        {
            string info;
            info = "<Force>\n" +
                " FXc:" + Max_Fx_compression.ToString() +
                " FXt:" + Max_Fx_tension.ToString() +
                " FY:" + Max_Fy_shear.ToString() + "\n" +
                " MZ:" + Max_Fz_shear.ToString() +
                " MX:" + Max_Mx_torsion.ToString() +
                " MY:" + Max_My_bending.ToString() +
                " MZ:" + Max_Mz_bending.ToString();
            return info;
        }
        public bool IsValid()
        {
            return true;
        }
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

        protected override System.Drawing.Bitmap Icon { get { return Properties.Resources.ParaForce; } }  //Set icon image

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

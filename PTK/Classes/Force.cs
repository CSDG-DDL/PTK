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
        public Point3d positionPoint { get; set; }
        public double position { get; set; }
        public int karambaElementID { get; set; }
        public int loadcase { get; set; }

        public double FX { get; set; }
        public double FY { get; set; }
        public double FZ { get; set; }
        public double MX { get; set; }
        public double MY { get; set; }
        public double MZ { get; set; }

        // --- constructors --- 
        #region constructors
        public Force()
        {
            // empty class

        }
        public Force(
            double _fx,
            double _fy,
            double _fz,
            double _mx,
            double _my,
            double _mz
            )
        {
            // only list of forces
            FX = _fx;
            FY = _fy;
            FZ = _fz;
            MX = _mx;
            MY = _my;
            MZ = _mz;
        }
        public Force(
            double _fx,
            double _fy,
            double _fz,
            double _mx,
            double _my,
            double _mz,
            int _loadcase,
            Point3d _positionPoint,
            double _position
            )
        {
            // only list of forces
            FX = _fx;
            FY = _fy;
            FZ = _fz;
            MX = _mx;
            MY = _my;
            MZ = _mz;
            loadcase = _loadcase;
            positionPoint = _positionPoint;
            position = _position;
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
                " FX:" + FX.ToString() +
                " FY:" + FY.ToString() +
                " MZ:" + FZ.ToString() +
                " MX:" + MX.ToString() +
                " MY:" + MY.ToString() +
                " MZ:" + MZ.ToString();
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

        public override Guid ComponentGuid => new Guid("FB9C3075-220C-424E-AC7B-E515303D8A2F");

        protected override GH_GetterResult Prompt_Plural(ref List<GH_Force> values)
        {
            return GH_GetterResult.success;
        }

        protected override GH_GetterResult Prompt_Singular(ref GH_Force value)
        {
            return GH_GetterResult.success;
        }
    }


    public class MaxCompression: Force
    {
        private string forceType="Compression";
        private string forceUnits = "kN";


        public MaxCompression(
            double _fx,
            double _fy,
            double _fz,
            double _mx,
            double _my,
            double _mz,
            int _loadcase,
            Point3d _positionPoint,
            double _position
            ) : base(
             _fx,
             _fy,
             _fz,
             _mx,
             _my,
             _mz,
             _loadcase,
             _positionPoint,
             _position)
        {
        }

    }

    public class MaxTension : Force
    {
        private string forceType = "Tension";
        private string forceUnits = "kN";

        public MaxTension(
            double _fx,
            double _fy,
            double _fz,
            double _mx,
            double _my,
            double _mz,
            int _loadcase,
            Point3d _positionPoint,
            double _position
            ) : base(
             _fx,
             _fy,
             _fz,
             _mx,
             _my,
             _mz,
             _loadcase,
             _positionPoint,
             _position)
        {
        }

    }

    public class MaxShearDir1 : Force
    {
        private string forceType = "ShearDir1";
        private string forceUnits = "kN";

        public MaxShearDir1(
            double _fx,
            double _fy,
            double _fz,
            double _mx,
            double _my,
            double _mz,
            int _loadcase,
            Point3d _positionPoint,
            double _position
            ) : base(
             _fx,
             _fy,
             _fz,
             _mx,
             _my,
             _mz,
             _loadcase,
             _positionPoint,
             _position)
        {
        }

    }

    public class MaxShearDir2 : Force
    {
        private string forceType = "ShearDir2";
        private string forceUnits = "kN";

        public MaxShearDir2(
            double _fx,
            double _fy,
            double _fz,
            double _mx,
            double _my,
            double _mz,
            int _loadcase,
            Point3d _positionPoint,
            double _position
            ) : base(
             _fx,
             _fy,
             _fz,
             _mx,
             _my,
             _mz,
             _loadcase,
             _positionPoint,
             _position)
        {
        }

    }

    public class MaxBendingDir1 : Force
    {
        private string forceType = "BendingDir1";
        private string forceUnits = "kNm";

        public MaxBendingDir1(
            double _fx,
            double _fy,
            double _fz,
            double _mx,
            double _my,
            double _mz,
            int _loadcase,
            Point3d _positionPoint,
            double _position
            ) : base(
             _fx,
             _fy,
             _fz,
             _mx,
             _my,
             _mz,
             _loadcase,
             _positionPoint,
             _position)
        {
        }

    }

    public class MaxBendingDir2 : Force
    {
        private string forceType = "BendingDir2";
        private string forceUnits = "kNm";

        public MaxBendingDir2(
            double _fx,
            double _fy,
            double _fz,
            double _mx,
            double _my,
            double _mz,
            int _loadcase,
            Point3d _positionPoint,
            double _position
            ) : base(
             _fx,
             _fy,
             _fz,
             _mx,
             _my,
             _mz,
             _loadcase,
             _positionPoint,
             _position)
        {
        }

    }
    public class MaxTorsion : Force
    {
        private string forceType = "Torsion";
        private string forceUnits = "kNm";

        public MaxTorsion(
            double _fx,
            double _fy,
            double _fz,
            double _mx,
            double _my,
            double _mz,
            int _loadcase,
            Point3d _positionPoint,
            double _position
            ) : base(
             _fx,
             _fy,
             _fz,
             _mx,
             _my,
             _mz,
             _loadcase,
             _positionPoint,
             _position)
        {
        }

    }
}

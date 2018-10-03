using System;
using System.Collections.Generic;
using Rhino.Geometry;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;

namespace PTK
{
    public abstract class Load
    {
        // --- field ---
        public string Tag { get; private set; } = "N/A";
        public int LoadCase { get; private set; } = 0;

        // --- constructors --- 
        public Load() { }
        public Load(string _tag)
        {
            Tag = _tag;
        }
        public Load(string _tag, int _loadCase)
        {
            Tag = _tag;
            LoadCase = _loadCase;
        }

        // --- methods ---
        public abstract Load DeepCopy();
        public override string ToString()
        {
            string info;
            info = "<Load> Tag:" + Tag +
                " LoadCase:" + LoadCase;
            return info;
        }
        public bool IsValid()
        {
            return true;
        }
    }

    public class PointLoad : Load
    {
        // --- field ---
        public Point3d Point { get; private set; } = new Point3d();
        public Vector3d ForceVector { get; private set; } = new Vector3d();
        public Vector3d MomentVector { get; private set; } = new Vector3d();

        // --- constructors --- 
        public PointLoad() : base() { }
        public PointLoad(string _tag) : base(_tag) { }
        public PointLoad(string _tag, int _loadCase, Point3d _point, Vector3d _forceVector, Vector3d _momentVector) : base(_tag,_loadCase)
        {
            Point = _point;
            ForceVector = _forceVector;
            MomentVector = _momentVector;
        }

        // --- methods ---
        public override Load DeepCopy()
        {
            return (Load)MemberwiseClone();
        }
        public override string ToString()
        {
            string info;
            info = "<PointLoad>\n" +
                " Tag:" + Tag + "\n" +
                " LoadCase:" + LoadCase.ToString() + "\n" +
                " Point:" + Point.ToString() + "\n" +
                " ForceVector:" + ForceVector.ToString() + "\n" +
                " MomentVector:" + MomentVector.ToString() ;
            return info;
        }
    }

    public class GravityLoad : Load
    {
        // --- field ---
        public Vector3d GravityVector { get; private set; } = new Vector3d();

        // --- constructors --- 
        public GravityLoad() : base() { }
        public GravityLoad(string _tag) : base(_tag) { }
        public GravityLoad(string _tag, int _loadCase, Vector3d _gravityVector) : base(_tag,_loadCase)
        {
            GravityVector = _gravityVector;
        }

        // --- methods ---
        public override Load DeepCopy()
        {
            return (Load)MemberwiseClone();
        }
        public override string ToString()
        {
            string info;
            info = "<GravityLoad>\n" +
                " Tag:" + Tag + "\n" +
                " LoadCase:" + LoadCase.ToString() + "\n" +
                " GravityVector:" + GravityVector.ToString();
            return info;
        }
    }


    public class GH_Load : GH_Goo<Load>
    {
        public GH_Load() { }
        public GH_Load(GH_Load other) : base(other.Value) { this.Value = other.Value.DeepCopy(); }
        public GH_Load(Load sec) : base(sec) { this.Value = sec; }
        public override bool IsValid => base.m_value.IsValid();

        public override string TypeName => "Load";

        public override string TypeDescription => "Description";

        public override IGH_Goo Duplicate()
        {
            return new GH_Load(this);
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }

    public class Param_Load : GH_PersistentParam<GH_Load>
    {
        public Param_Load() : base(new GH_InstanceDescription("Load", "Load", "Description", CommonProps.category, CommonProps.subcate0)) { }

        protected override System.Drawing.Bitmap Icon { get { return Properties.Resources.ParaLoad; } }  //Set icon image

        public override Guid ComponentGuid => new Guid("01A7F933-62C3-4780-87EB-381F3344D370");

        protected override GH_GetterResult Prompt_Plural(ref List<GH_Load> values)
        {
            return GH_GetterResult.success;
        }

        protected override GH_GetterResult Prompt_Singular(ref GH_Load value)
        {
            return GH_GetterResult.success;
        }
    }
}


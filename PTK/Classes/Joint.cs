using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;

namespace PTK
{
    public class Joint
    {
        // --- field ---
        public int LoadCase { get; private set; } = 0;
        public Vector3d TranslateSpringAtStart { get; private set; }
        public Vector3d RotateSpringAtStart { get; private set; }
        public Vector3d TranslateSpringAtEnd { get; private set; }
        public Vector3d RotateSpringAtEnd { get; private set; }
        public List<bool> Conditions { get; private set; }

        // --- constructors --- 
        public Joint()
        {
        }
        public Joint(int _loadCase, Vector3d _transS, Vector3d _rotS, Vector3d _transE, Vector3d _rotE, List<bool> _conditions)
        {
            LoadCase = _loadCase;
            TranslateSpringAtStart = _transS;
            RotateSpringAtStart = _rotS;
            TranslateSpringAtEnd = _transE;
            RotateSpringAtEnd = _rotE;
            Conditions = _conditions;
        }

        // --- methods ---
        public Joint DeepCopy()
        {
            return (Joint)base.MemberwiseClone();
        }
        public override string ToString()
        {
            string info;
            info = "<Joint> LoadCase:" + LoadCase.ToString() +
                " TranslateSpringAtStart:" + TranslateSpringAtStart.ToString() +
                " RotateSpringAtStart:" + RotateSpringAtStart.ToString() +
                " TranslateSpringAtEnd:" + TranslateSpringAtEnd.ToString() +
                " RotateSpringAtEnd:" + RotateSpringAtEnd.ToString() +
                " Conditions:" + Conditions.ToString();
            return info;
        }
        public bool IsValid()
        {
            return true;
        }
    }

    public class GH_Joint : GH_Goo<Joint>
    {
        public GH_Joint() { }
        public GH_Joint(GH_Joint other) : base(other.Value) { this.Value = other.Value.DeepCopy(); }
        public GH_Joint(Joint sec) : base(sec) { this.Value = sec; }
        public override bool IsValid => base.m_value.IsValid();

        public override string TypeName => "Joint";

        public override string TypeDescription => "Joint";

        public override IGH_Goo Duplicate()
        {
            return new GH_Joint(this);
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }

    public class Param_Joint : GH_PersistentParam<GH_Joint>
    {
        public Param_Joint() : base(new GH_InstanceDescription("Joint", "Joint", "Joint", CommonProps.category, CommonProps.subcate0)) { }

        protected override System.Drawing.Bitmap Icon { get { return null; } }  //Set icon image

        public override Guid ComponentGuid => new Guid("45E370EA-0754-4F65-9F53-0DEBF9FAC7CB");

        protected override GH_GetterResult Prompt_Plural(ref List<GH_Joint> values)
        {
            return GH_GetterResult.success;
        }

        protected override GH_GetterResult Prompt_Singular(ref GH_Joint value)
        {
            return GH_GetterResult.success;
        }
    }
}

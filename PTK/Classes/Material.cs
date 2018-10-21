using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;

namespace PTK
{
    public class Material
    {
        // --- field ---
        public string Name { get; private set; } = "N/A";
        public MaterialProperty StructuralProp { get; private set; } = new MaterialProperty("MaterialProp");
        public Color Color { get; private set; } = new Color();

        // --- constructors --- 
        public Material() { }
        public Material(string _name)
        {
            Name = _name;
        }
        public Material(string _name, MaterialProperty _structuralProp, Color _color)
        {
            Name = _name;
            StructuralProp = _structuralProp;
            Color = _color;
        }

        // --- methods ---
        public Material DeepCopy()
        {
            return (Material)base.MemberwiseClone();
        }
        public override string ToString()
        {
            string info;
            info = "<Material>\n" +
                " Name:" + Name + "\n" +
                " StructuralProp.Name:" + StructuralProp.Name + "\n" +
                " Color:" + Color.ToString();
            return info;
        }

        public bool IsValid()
        {
            return Name != "N/A";
        }
    }

    public class GH_Material : GH_Goo<Material>
    {
        public GH_Material() { }
        public GH_Material(GH_Material other) : base(other.Value) { this.Value = other.Value.DeepCopy(); }
        public GH_Material(Material mat) : base(mat) { this.Value = mat; }
        public override bool IsValid => base.m_value.IsValid();

        public override string TypeName => "Material";

        public override string TypeDescription => "Material";

        public override IGH_Goo Duplicate()
        {
            return new GH_Material(this);
        }

        public override string ToString()
        {
            return Value.ToString(); ;
        }
    }

    public class Param_Material : GH_PersistentParam<GH_Material>
    {
        public Param_Material() : base(new GH_InstanceDescription("Material", "Mat", "Material name and property information", CommonProps.category, CommonProps.subcate0)) { }

        protected override System.Drawing.Bitmap Icon { get { return Properties.Resources.ParaMaterial; } }  //Set icon image

        public override Guid ComponentGuid => new Guid("62539F56-FB20-4342-AC8F-E7C1A2F7BAA2");

        protected override GH_GetterResult Prompt_Plural(ref List<GH_Material> values)
        {
            return GH_GetterResult.success;
        }

        protected override GH_GetterResult Prompt_Singular(ref GH_Material value)
        {
            return GH_GetterResult.success;
        }
    }

}
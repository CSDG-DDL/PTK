using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rhino.Geometry;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;


namespace PTK
{
    //public class Sub2DElement
    //{
    //    // --- field ---
    //    public string Name { get; private set; } = "N/A";
    //    public CrossSection CrossSection { get; private set; } = new RectangleCroSec("RectCroSec");
    //    public MaterialProperty MaterialProperty { get; private set; } = new MaterialProperty("Material");
    //    public Alignment Alignment { get; private set; } = new Alignment("Alignment");

    //    // --- constructors ---
    //    public Sub2DElement() { }
    //    public Sub2DElement(string _name, MaterialProperty _materialProperty, CrossSection _crossSection, Alignment _alignment)
    //    {
    //        Name = _name;
    //        MaterialProperty = _materialProperty;
    //        CrossSection = _crossSection;
    //        Alignment = _alignment;
    //    }

    //    // --- methods ---
    //    public Sub2DElement DeepCopy()
    //    {
    //        return (Sub2DElement)base.MemberwiseClone();
    //    }
    //    public override string ToString()
    //    {
    //        string info;
    //        info = "<SubElement>\n" +
    //            " Name:" + Name + "\n" +
    //            " CrossSection:" + CrossSection.Name + "\n" +
    //            " Material:" + MaterialProperty.Name + "\n" +
    //            " Alignment:" + Alignment.Name;
    //        return info;
    //    }
    //    public bool IsValid()
    //    {
    //        return Name != "N/A";
    //    }
    //}

    //public class GH_Sub2DElement : GH_Goo<Sub2DElement>
    //{
    //    public GH_Sub2DElement() { }
    //    public GH_Sub2DElement(GH_Sub2DElement other) : base(other.Value) { this.Value = other.Value.DeepCopy(); }
    //    public GH_Sub2DElement(Sub2DElement subelem) : base(subelem) { this.Value = subelem; }
    //    public override IGH_Goo Duplicate()
    //    {
    //        return new GH_Sub2DElement(this);
    //    }
    //    public override bool IsValid => base.m_value.IsValid();
    //    public override string TypeName => "SubElement";
    //    public override string TypeDescription => "A part of an Element";
    //    public override string ToString()
    //    {
    //        return Value.ToString();
    //    }
    //}

    //public class Param_Sub2DElement : GH_PersistentParam<GH_Sub2DElement>
    //{
    //    public Param_Sub2DElement() : base(new GH_InstanceDescription("SubElement", "SubElem",
    //        "A part of an Element", CommonProps.category, CommonProps.subcate0))
    //    { }
    //    protected override System.Drawing.Bitmap Icon { get { return null; } }
    //    public override Guid ComponentGuid => new Guid("8a12f26a-532b-4da0-80a0-d775f5648123");
    //    protected override GH_GetterResult Prompt_Plural(ref List<GH_Sub2DElement> values)
    //    {
    //        return GH_GetterResult.success;
    //    }
    //    protected override GH_GetterResult Prompt_Singular(ref GH_Sub2DElement value)
    //    {
    //        return GH_GetterResult.success;
    //    }
    //}
}

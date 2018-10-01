using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Linq;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;

namespace PTK
{
    public abstract class CrossSection
    {
        // --- field ---
        public string Name { get; private set; }
        // public MaterialProperty MaterialProperty { get; private set; }

        // --- constructors --- 
        public CrossSection()
        {
            Name = "N/A";
            // MaterialProperty = new MaterialProperty();
        }
        public CrossSection(string _name)
        {
            Name = _name;
            // MaterialProperty = new MaterialProperty();
        }
        public CrossSection(string _name, MaterialProperty _materialProperty)
        {
            Name = _name;
            // MaterialProperty = _materialProperty;
        }

        // --- methods ---
        public abstract double GetHeight();
        public abstract double GetWidth();
        public static void GetMaxHeightAndWidth(List<CrossSection> _secs,out double _height,out double _width)
        {
            _height = _secs.Max(s => s.GetHeight());
            _width = _secs.Max(s => s.GetWidth());
        }
        public abstract CrossSection DeepCopy();
        public override string ToString()
        {
            string info;
            info = "<CrossSection> Name:" + Name;  
              //  " Material Property:" + MaterialProperty.Name;
            return info;
        }
        public bool IsValid()
        {
            return true;
        }
    }

    public class RectangleCroSec : CrossSection
    {
        // --- field ---
        private double height = 100;
        private double width = 100;

        // --- constructors --- 
        public RectangleCroSec() : base()
        {
        }
       public RectangleCroSec(string _name, double _height, double _width) : base(_name)
        {
            SetHeight(_height);
            SetWidth(_width);
        }

        // --- properties ---
        private void SetHeight(double _height)
        {
            if (_height <= 0)
            {
                throw new ArgumentException("value <= 0");
            }
            height = _height;
        }
        public override double GetHeight()
        {
            return height;
        }
        private void SetWidth(double _width)
        {
            if (_width <= 0)
            {
                throw new ArgumentException("value <= 0");
            }
            width = _width;
        }
        public override double GetWidth()
        {
            return width;
        }

        // --- methods ---
        public override CrossSection DeepCopy()
        {
            return (CrossSection)base.MemberwiseClone();
        }
        public override string ToString()
        {
            string info;
            info = "<RectangleCroSec> Name:" + Name +
                " Height:" + height.ToString() +
                " Width:" + width.ToString(); 
                // " Material Property:" + MaterialProperty.Name;
            return info; 
        }
    }

    public class GH_CroSec : GH_Goo<CrossSection>
    {
        public GH_CroSec() { }
        public GH_CroSec(GH_CroSec other) : base(other.Value) { this.Value = other.Value.DeepCopy(); }
        public GH_CroSec(CrossSection sec) : base(sec) { this.Value = sec; }
        public override bool IsValid => base.m_value.IsValid();

        public override string TypeName => "CrossSection";

        public override string TypeDescription => "Cross Sectional shape of Element and its material";
        public override IGH_Goo Duplicate()
        {
            return new GH_CroSec(this);
        }
        public override string ToString()
        {
            return Value.ToString();
        }
    }

    public class Param_CroSec : GH_PersistentParam<GH_CroSec>
    {
        public Param_CroSec() : base(new GH_InstanceDescription("CrossSection", "CroSec", "Cross Sectional shape of Element and its material", CommonProps.category, CommonProps.subcate0)) { }
        protected override System.Drawing.Bitmap Icon { get { return Properties.Resources.ParaCrossSection; } }

        public override Guid ComponentGuid => new Guid("480DDCC7-02FB-497D-BF9F-4FAE3CE0687A");
        protected override GH_GetterResult Prompt_Plural(ref List<GH_CroSec> values)
        {
            return GH_GetterResult.success;
        }
        protected override GH_GetterResult Prompt_Singular(ref GH_CroSec value)
        {
            return GH_GetterResult.success;
        }
    }

}

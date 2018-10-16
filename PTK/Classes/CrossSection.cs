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
        public string Name { get; private set; } = "N/A";
        public MaterialProperty MaterialProperty { get; private set; } = new MaterialProperty("Not Named Material");
        public Alignment Alignment { get; private set; } = new Alignment("Not Named Alignment");

        // --- constructors --- 
        public CrossSection() { }
        public CrossSection(string _name)
        {
            Name = _name;
        }
        public CrossSection(string _name, MaterialProperty _material, Alignment _alignment)
        {
            Name = _name;
            MaterialProperty = _material;
            Alignment = _alignment;
        }

        // --- methods ---
        public abstract double GetHeight();
        public abstract double GetWidth();
        public static void GetMaxHeightAndWidth(List<CrossSection> _secs,out double _height,out double _width)
        {
            //It is a simple implementation and needs to be corrected later. Align is not taken into account.
            //簡易的な実装なので後で修正が必要。Alignが考慮されていない。
            _height = _secs.Max(s => s.GetHeight());
            _width = _secs.Max(s => s.GetWidth());
        }
        public abstract CrossSection DeepCopy();
        public override string ToString()
        {
            string info;
            info = "<CrossSection> Name:" + Name;  
            return info;
        }
        public bool IsValid()
        {
            return Name != "N/A";
        }
    }

    public class RectangleCroSec : CrossSection
    {
        // --- field ---
        public double Height { get; private set; } = 100;
        public double Width { get; private set; } = 100;

        // --- constructors --- 
        public RectangleCroSec() : base() { }
        public RectangleCroSec(string _name) : base(_name) { }
        public RectangleCroSec(string _name, MaterialProperty _material, double _height, double _width, Alignment _alignment) : base(_name, _material, _alignment)
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
            Height = _height;
        }
        public override double GetHeight()
        {
            return Height;
        }
        private void SetWidth(double _width)
        {
            if (_width <= 0)
            {
                throw new ArgumentException("value <= 0");
            }
            Width = _width;
        }
        public override double GetWidth()
        {
            return Width;
        }

        // --- methods ---
        public override CrossSection DeepCopy()
        {
            return (RectangleCroSec)base.MemberwiseClone();
        }
        public override string ToString()
        {
            string info;
            info = "<RectangleCroSec>\n"+
                " Name:" + Name + "\n" +
                " Height:" + Height.ToString() + "\n" +
                " Width:" + Width.ToString(); 
            return info; 
        }
    }

    public class CircularCroSec : CrossSection
    {
        // --- field ---
        public double Radius { get; private set; } = 100;

        // --- constructors --- 
        public CircularCroSec() : base() { }
        public CircularCroSec(string _name) : base(_name) { }
        public CircularCroSec(string _name, MaterialProperty _material, double _radius, Alignment _alignment) : base(_name, _material, _alignment)
        {
            SetRadius(_radius);
        }

        // --- properties ---
        private void SetRadius(double _radius)
        {
            if (_radius <= 0)
            {
                throw new ArgumentException("value <= 0");
            }
            Radius = _radius;
        }
        public double GetDiameter()
        {
            return Radius * 2;
        }
        public override double GetHeight()
        {
            return Radius * 2;
        }
        public override double GetWidth()
        {
            return Radius * 2;
        }

        // --- methods ---
        public override CrossSection DeepCopy()
        {
            return (CircularCroSec)base.MemberwiseClone();
        }
        public override string ToString()
        {
            string info;
            info = "<CircularCroSec>\n" +
                " Name:" + Name + "\n" +
                " Radius:" + Radius.ToString();
            return info;
        }
    }


    public class Composite : CrossSection
    {
        // --- field ---
        public List<CrossSection> SubCrossSections { get; private set; } = new List<CrossSection>();

        // --- constructors --- 
        public Composite() : base() { }
        public Composite(string _name) : base(_name) { }
        public Composite(string _name, MaterialProperty _material, List<CrossSection> _subCrossSections, Alignment _alignment) : base(_name, _material, _alignment)
        {
            SubCrossSections = _subCrossSections;
        }

        //public bool AddCrossSection(CrossSection _crossSection, Alignment _alignment)
        //{
        //    if (_crossSection.IsValid() && _alignment.IsValid())
        //    {
        //        SubCrossSections.Add(new Tuple<CrossSection, Alignment>(_crossSection, _alignment));
        //    }
        //    return false;
        //}
        // --- methods ---
        public List<Tuple<CrossSection, Alignment>> RecursionCrossSectionSearch()
        {
            //Alignmentの再帰に未対応　Alignmentの加算が必要
            List<Tuple<CrossSection, Alignment>> crossSections = new List<Tuple<CrossSection, Alignment>>(); 
            foreach (var s in SubCrossSections)
            {
                if(s is Composite comp)
                {
                    crossSections.AddRange(comp.RecursionCrossSectionSearch());
                }
                else
                {
                    crossSections.Add(new Tuple<CrossSection, Alignment>(s,s.Alignment));
                }
            }
            return crossSections;
        }
        public override double GetHeight()
        {
            GetHeightAndWidth(out double _width, out double _height);
            return _height;
        }
        public override double GetWidth()
        {
            GetHeightAndWidth(out double _width, out double _height);
            return _width;
        }
        //Re-examination is necessary because Align rotation etc. are not considered.
        //Alignの回転などを考慮していないので再検討が必要。
        private void GetHeightAndWidth(out double _width, out double _height)
        {
            double maxHeight = double.MinValue;
            double maxWidth = double.MinValue;
            double minHeight = double.MaxValue;
            double minWidth = double.MaxValue;
            foreach (var s in SubCrossSections)
            {
                double tempVal;

                // update maxHeight 
                tempVal = s.Alignment.OffsetZ + s.GetHeight() / 2;
                if (tempVal > maxHeight)
                {
                    maxHeight = tempVal;
                }

                // update minHeight 
                tempVal = s.Alignment.OffsetZ - s.GetHeight() / 2;
                if (tempVal < minHeight)
                {
                    minHeight = tempVal;
                }

                // update maxWidth 
                tempVal = s.Alignment.OffsetY + s.GetWidth() / 2;
                if (tempVal > maxWidth)
                {
                    maxWidth = tempVal;
                }

                // update minWidth 
                tempVal = s.Alignment.OffsetY - s.GetWidth() / 2;
                if (tempVal < minWidth)
                {
                    minWidth = tempVal;
                }

            }
            _height = maxHeight - minHeight;
            _width = maxWidth - minWidth;
        }

        public override CrossSection DeepCopy()
        {
            return (CrossSection)base.MemberwiseClone();
        }
        public override string ToString()
        {
            string info;
            info = "<CompositeCroSec>\n" +
                " Name:" + Name + "\n" +
                " SubCroSecs:" + SubCrossSections.Count.ToString();
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

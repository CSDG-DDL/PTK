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
    public class Composite
    {
        // --- field ---
        public string Name { get; private set; }
        public List<Sub2DElement> Sub2DElements { get; private set; }
        public Alignment Alignment { get; private set; }

        // --- constructors --- 
        public Composite()
        {
            Name = "N/A";
            Sub2DElements = new List<Sub2DElement>();
            Alignment = new Alignment();
        }
        public Composite(string _name, List<Sub2DElement> _sub2DElements, Alignment _alignment)
        {
            Name = _name;
            Sub2DElements = _sub2DElements;
            Alignment = _alignment;
        }

        // --- methods ---
        public void GetHeightAndWidth(out double _width, out double _height)
        {
            double maxHeight = double.MinValue;
            double maxWidth = double.MinValue;
            double minHeight = double.MaxValue;
            double minWidth = double.MaxValue;
            List<Sub2DElement> sub2DElements = this.Sub2DElements;
            foreach (Sub2DElement s in sub2DElements)
            {
                double tempVal;

                // update maxHeight 
                tempVal = s.Alignment.OffsetZ + s.CrossSection.GetHeight() / 2;
                if (tempVal > maxHeight)
                {
                    maxHeight = tempVal;
                }

                // update minHeight 
                tempVal = s.Alignment.OffsetZ - s.CrossSection.GetHeight() / 2;
                if (tempVal < minHeight)
                {
                    minHeight = tempVal;
                }

                // update maxWidth 
                tempVal = s.Alignment.OffsetY + s.CrossSection.GetWidth() / 2;
                if (tempVal > maxWidth)
                {
                    maxWidth = tempVal;
                }

                // update minWidth 
                tempVal = s.Alignment.OffsetY - s.CrossSection.GetWidth() / 2;
                if (tempVal < minWidth)
                {
                    minWidth = tempVal;
                }
                
            }

            _height = maxHeight - minHeight;
            _width = maxWidth - minWidth;
            
        }
        public Composite DeepCopy()
        {
            return (Composite)base.MemberwiseClone();
        }

        public override string ToString()
        {
            string info;
            info = "<Composite> Name:" + Name;
            // plus Subsections, etc.
            return info;
        }

        public bool IsValid()
        {
            return Name != "N/A";
        }
    }

    public class GH_Composite : GH_Goo<Composite>
    {
        public GH_Composite() { }
        public GH_Composite(GH_Composite other) : base(other.Value) { this.Value = other.Value.DeepCopy(); }
        public GH_Composite(Composite composite) : base(composite) { this.Value = composite; }
        public override IGH_Goo Duplicate()
        {
            return new GH_Composite(this);
        }
        public override bool IsValid => base.m_value.IsValid();
        public override string TypeName => "Composite";
        public override string TypeDescription => "Composite Cross-section";
        public override string ToString()
        {
            return Value.ToString();
        }
    }

    public class Param_Composite : GH_PersistentParam<GH_Composite>
    {

        public Param_Composite() : base(new GH_InstanceDescription(
            "Composite Cross-section", "Composite", "Composite Cross-section", 
            CommonProps.category, CommonProps.subcate0))
        { }
        protected override System.Drawing.Bitmap Icon { get { return Properties.Resources.Composite; } }
        public override Guid ComponentGuid => new Guid("c075b093-0f5a-4ff0-8faf-3de8bcd6fa7a");
        protected override GH_GetterResult Prompt_Plural(ref List<GH_Composite> values)
        {
            return GH_GetterResult.success;
        }
        protected override GH_GetterResult Prompt_Singular(ref GH_Composite value)
        {
            return GH_GetterResult.success;
        }

    }

}

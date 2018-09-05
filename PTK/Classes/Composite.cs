﻿using System;
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
        /////////////////////////////////////////////////////////////////////////////////
        // fields
        /////////////////////////////////////////////////////////////////////////////////
        public string Name { get; private set; }
        public List<Sub2DElement> SubElements { get; private set; }
        public Alignment Alignment { get; private set; }
        /////////////////////////////////////////////////////////////////////////////////
        // constructors
        /////////////////////////////////////////////////////////////////////////////////
        public Composite()
        {
            Name = null;
            SubElements = new List<Sub2DElement>();
            Alignment = new Alignment();
        }

        public Composite(string _name, List<Sub2DElement> _subElements, Alignment _alignment)
        {
            Name = _name;
            SubElements = _subElements;
            Alignment = _alignment;
        }
        /////////////////////////////////////////////////////////////////////////////////
        // properties
        /////////////////////////////////////////////////////////////////////////////////

        /////////////////////////////////////////////////////////////////////////////////
        // methods
        /////////////////////////////////////////////////////////////////////////////////

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
        /////////////////////////////////////////////////////////////////////////////////
        // fields
        /////////////////////////////////////////////////////////////////////////////////

        /////////////////////////////////////////////////////////////////////////////////
        // constructors
        /////////////////////////////////////////////////////////////////////////////////
        public GH_Composite() { }
        public GH_Composite(GH_Composite other) : base(other.Value)
        {
            this.Value = other.Value;
        }
        public GH_Composite(Composite composite) : base(composite)
        {
            this.Value = composite;
        }
        /////////////////////////////////////////////////////////////////////////////////
        // properties
        /////////////////////////////////////////////////////////////////////////////////

        /////////////////////////////////////////////////////////////////////////////////
        // methods
        /////////////////////////////////////////////////////////////////////////////////

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
        /////////////////////////////////////////////////////////////////////////////////
        // fields
        /////////////////////////////////////////////////////////////////////////////////

        /////////////////////////////////////////////////////////////////////////////////
        // constructors
        /////////////////////////////////////////////////////////////////////////////////

        public Param_Composite() : base(new GH_InstanceDescription(
            "Composite Cross-section", "Composite", "Composite Cross-section", 
            CommonProps.category, CommonProps.subcate0))
        { }

        /////////////////////////////////////////////////////////////////////////////////
        // properties
        /////////////////////////////////////////////////////////////////////////////////

        // set an icon here
        protected override System.Drawing.Bitmap Icon { get { return null; } }

        public override Guid ComponentGuid => new Guid("c075b093-0f5a-4ff0-8faf-3de8bcd6fa7a");

        /////////////////////////////////////////////////////////////////////////////////
        // methods
        /////////////////////////////////////////////////////////////////////////////////

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

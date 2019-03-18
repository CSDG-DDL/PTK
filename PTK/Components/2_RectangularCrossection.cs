using System;
using System.Collections.Generic;
using System.Windows.Forms;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace PTK
{
    public class PTK_RectangularCrossSection : GH_Component
    {
        public PTK_RectangularCrossSection()
          : base("Rectangular Cross Section", "RectSec",
              "CrossSection is being generated based on width, height, alignment and height-direction. GL26 as default material.",
              CommonProps.category, CommonProps.subcate2)
        {
            Message = CommonProps.initialMessage;
        }
        /// <summary>
        /// Overrides the exposure level in the components category 
        /// </summary>
        public override GH_Exposure Exposure
        {
            get
            { return GH_Exposure.tertiary; }
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Name", "N", "Add Cross Section Name", GH_ParamAccess.item, "Not Named RectCroSect");
            pManager.AddNumberParameter("Width", "W", "", GH_ParamAccess.item,100);  
            pManager.AddNumberParameter("Height", "H", "", GH_ParamAccess.item,100);
            pManager.AddParameter(new Param_MaterialProperty(), "Material properties", "M", "Add material properties", GH_ParamAccess.item);
            pManager.AddParameter(new Param_Alignment(), "Alignment", "A", "Local Alignment", GH_ParamAccess.item);

            pManager[0].Optional = true;
            pManager[1].Optional = true;
            pManager[2].Optional = true;
            pManager[3].Optional = true;
            pManager[4].Optional = true;
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Cross Section", "S", "Cross Section data to be connected in the Element", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // --- variables ---
            string name = null;
            double width = new double();
            double height = new double();
            GH_MaterialProperty gMaterial = null;
            MaterialProperty material = null;
            GH_Alignment gAlignment = null;
            Alignment alignment = null;

            // --- input --- 
            if (!DA.GetData(0, ref name)) { return; }
            if (!DA.GetData(1, ref width)) { return; }
            if (!DA.GetData(2, ref height)) { return; }
            if (!DA.GetData(3, ref gMaterial)) { material = new MaterialProperty("Not Named Material"); }
            else { material = gMaterial.Value; }
            if (!DA.GetData(4, ref gAlignment)) { alignment = new Alignment("Not Named Alignment"); ; }
            else { alignment = gAlignment.Value; }

            // --- solve ---
            GH_CroSec sec = new GH_CroSec(new RectangleCroSec(name, material, height, width, alignment));
            CompositeInput Composite = new CompositeInput(new RectangleCroSec(name, material, height, width, alignment));




            // --- output ---
            DA.SetData(0, Composite);
        }

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return PTK.Properties.Resources.RectangleCS;
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("59eb5896-0ccb-4e37-be5e-ba4ee7931ee1"); }
        }
    }
}
using System;
using System.Collections.Generic;

using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;

namespace PTK.Components
{
    public class ExtractCrossSection : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the ExtractCrossSection class.
        /// </summary>
        public ExtractCrossSection()
          : base("Deconstruct Subelement", "SE",
              "Deconstructs a SubeElement",
              CommonProps.category, CommonProps.subcate8)
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter( "Subelement", "SubElement", "Deconstruct a subelement", GH_ParamAccess.item);


        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddPlaneParameter("CroSecLocalPlaneCenter", "Ce", "Centric CrossSectionPlane of element", GH_ParamAccess.item);
            pManager.AddNumberParameter("Width", "W", "Subelement Width", GH_ParamAccess.item);
            pManager.AddNumberParameter("Height", "H", "Subelement Height", GH_ParamAccess.item);
            pManager.AddBrepParameter("SubElementGeometry", "SE", "Geometry of subelement",GH_ParamAccess.item);


        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            SubElement SubElement = null;



            if (!DA.GetData(0, ref SubElement)) { return; }



            DA.SetData(0, SubElement.CroSecLocalCenterPlane);
            DA.SetData(1, SubElement.Width);
            DA.SetData(2, SubElement.Height);
            DA.SetData(3, SubElement.GenerateElementGeometry());
        }


        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
                return PTK.Properties.Resources.ExtCompsite;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("299db316-121c-423e-8473-3f1ffe464947"); }
        }
    }
}
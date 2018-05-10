﻿using System;
using System.Collections.Generic;
using System.Windows.Forms;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace PTK
{
    public class PTK1_3_RectangularCrossection : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the PTK6 class.
        /// </summary>
        public PTK1_3_RectangularCrossection()
          : base("Rectangular CrossSection", "R CS",
              "CrossSection is being generated based on width, height, alignment and height-direction ",
              "PTK", "Materializer")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("CrossSectionName", "N", "Add Cross-SectionName", GH_ParamAccess.item,"Untitled");
            pManager.AddNumberParameter("Width", "W", "Width", GH_ParamAccess.item,100);  
            pManager.AddNumberParameter("Height", "H", "Height", GH_ParamAccess.item,100);
            

            pManager[0].Optional = true;
            pManager[1].Optional = true;
            pManager[2].Optional = true;

        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("CrossSection", "CS", "Crossection data to be connected in the materializer", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            #region variables
            string name = "N/A";
            double width = new double();
            double height = new double();
            Vector3d offset = new Vector3d(0, 0, 0);

            #endregion

            #region input
            if (!DA.GetData(0, ref name)) { return; }
            if (!DA.GetData(1, ref width)) { return; }
            if (!DA.GetData(2, ref height)) { return; }
            

            #endregion

            #region solve
            Section rectSec = new Section(name, width, height);
            string test = "";
            test += rectSec.SectionName + ", " + rectSec.Height.ToString();
            // MessageBox.Show(test);
            #endregion

            #region output
            DA.SetData(0, rectSec);
            #endregion
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
                return PTK.Properties.Resources.icontest4;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("59eb5896-0ccb-4e37-be5e-ba4ee7931ee1"); }
        }
    }
}
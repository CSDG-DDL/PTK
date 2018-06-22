﻿using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace PTK
{
    public class PTK_1_4_Alignment_Simple : GH_Component
    {
        public PTK_1_4_Alignment_Simple()
          : base("Simple Alignment", "Align",
              "Simple Alignment",
              CommonProps.category, CommonProps.subcat2)
        {
            Message = CommonProps.initialMessage;
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("Name", "N", "Alignment Name", GH_ParamAccess.item, 0.0);
            pManager.AddNumberParameter("offset Local y", "local y", "Offset length local y", GH_ParamAccess.item, 0.0);
            pManager.AddNumberParameter("offset Local z", "local z", "Offset length local z", GH_ParamAccess.item, 0.0);
            pManager.AddNumberParameter("angle", "angle", "Rotational angle in degree", GH_ParamAccess.item, 0.0);

            pManager[0].Optional = true;
            pManager[1].Optional = true;
            pManager[2].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Alignment", "Aln (PTK)", "AlignmentData to be added to materializer", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            #region variables
            double offsetY = new double();
            double offsetZ = new double();
            double rotationAngle = new double();
            #endregion

            #region input
            if (!DA.GetData(0, ref offsetY)) { return; }
            if (!DA.GetData(1, ref offsetZ)) { return; }
            if (!DA.GetData(2, ref rotationAngle)) { return; }
            #endregion

            #region solve
            Alignment Simple = new Alignment(offsetY, offsetZ, rotationAngle);

            #endregion

            #region output
            DA.SetData(0, Simple);
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
                return PTK.Properties.Resources.ico_align;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("a2017425-8288-4c78-9111-f9d9588a34df"); }
        }
    }
}
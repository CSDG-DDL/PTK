﻿using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace PTK.Components
{
    public class _12_DGPlaneRule_SurfaceNormal : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the _12_DGPlaneRule_SurfaceNormal class.
        /// </summary>
        public _12_DGPlaneRule_SurfaceNormal()
          : base("NormalVectorFromSurface", "S",
              "Gets normal vector of the node plane from a surface",
              CommonProps.category, CommonProps.subcate10)
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddSurfaceParameter("SurfaceGuide", "N", "Input surfaceGuide", GH_ParamAccess.item);
            pManager.AddTextParameter("Name of Alignmentelement", "A", "Optional. Add name of the element that aligns the plane", GH_ParamAccess.item);
            pManager[1].Optional = true;
        }


        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("PlaneRule", "PR", "PlaneRule", GH_ParamAccess.item);

        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {

            //Variables 
            Surface Surface = null;
            string AlignmentElementName = "";


            //Input 
            DA.GetData(0, ref Surface);
            DA.GetData(1, ref AlignmentElementName);


            //Solve 
            PlaneRules.SurfaceNormalPlane PlaneRule = new PlaneRules.SurfaceNormalPlane(Surface, AlignmentElementName);


            //Output
            DA.SetData(0, new PlaneRules.PlaneRule(PlaneRule.GenerateDetailingGroupPlane));


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
                return null;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("3263eda3-38e9-4861-911a-894c6536057f"); }
        }
    }
}
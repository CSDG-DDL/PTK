using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace PTK.Components
{
    public class _13_GlobalAlign_FromPlane : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the _13_GlobalAlign_FromPlane class.
        /// </summary>
        public _13_GlobalAlign_FromPlane()
          : base("AlignFromPlane", "Pl",
              "Align element Z-vectors to plane",
              CommonProps.category, CommonProps.subcate2)
        {   
        }
        /// <summary>
        /// Overrides the exposure level in the components category 
        /// </summary>
        public override GH_Exposure Exposure
        {
            get
            {return GH_Exposure.secondary; }
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddPlaneParameter("Plane", "Pl", "Aligns Z towards plane", GH_ParamAccess.item);
            pManager.AddNumberParameter("Offset Width(Y)", "OY", "Add negative or positive value to offset crosssection/composite from center-curve", GH_ParamAccess.item, 0);
            pManager.AddNumberParameter("Offset Height(Z)", "OZ", "Add negative or positive value to offset crosssection/composite from center-curve", GH_ParamAccess.item, 0);

        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Alignment", "A", "Add alignment to element", GH_ParamAccess.item);
        }


        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Plane plane = new Plane();
            double domain = 0.5;
            double OffsetY = 0;
            double offsetZ = 0;


            if (!DA.GetData(0, ref plane)) { return; }
            if (!DA.GetData(1, ref OffsetY)) { return; }
            if (!DA.GetData(2, ref offsetZ)) { return; }

            GlobalAlignmentRules.AlignmentFromPlane VectorAlign = new GlobalAlignmentRules.AlignmentFromPlane(plane);


            

            ElementAlign Alignment = new ElementAlign(VectorAlign.GenerateVector, OffsetY, offsetZ);

            


            DA.SetData(0, Alignment);



        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                return PTK.Properties.Resources.Alignment;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("bf19f26e-1a9f-402d-87ca-af0213dd1987"); }
        }
    }
}
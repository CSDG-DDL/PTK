using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace PTK.Components
{
    public class _13_GlobalAlign_FromPoints : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the _13_GlobalAlign_FromPoints class.
        /// </summary>
        public _13_GlobalAlign_FromPoints()
          : base("GlobalAlignFromPoints", "P",
              "Aligns element Z-vector from point(s). Closest point is used when several points are inserted.",
              CommonProps.category, CommonProps.subcate2)
        {
        }
        /// <summary>
        /// Overrides the exposure level in the components category 
        /// </summary>
        public override GH_Exposure Exposure
        {
            get
            { return GH_Exposure.secondary; }
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddPointParameter("Points", "P", "Add atractorpoints to align element here", GH_ParamAccess.list);
            pManager.AddNumberParameter("CurveDomain", "D", "Add domain if you want to spesify where the on the curve the distance is sampled", GH_ParamAccess.item, 0.5);
            pManager.AddNumberParameter("Offset Width(Y)", "OY", "Add negative or positive value to offset crosssection/composite from center-curve", GH_ParamAccess.item, 0);
            pManager.AddNumberParameter("Offset Height(Z)", "OZ", "Add negative or positive value to offset crosssection/composite from center-curve", GH_ParamAccess.item, 0);

        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("GlobalAlignmenrt", "A", "Add global alignment to element", GH_ParamAccess.item);
        }


        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<Point3d> atractorPoints = new List<Point3d>();
            double domain = 0.5;
            double OffsetY = 0;
            double offsetZ = 0;


            if (!DA.GetDataList(0, atractorPoints)) { return; }
            if (!DA.GetData(1, ref domain)) { return; }
            if (!DA.GetData(2, ref OffsetY)) { return; }
            if (!DA.GetData(3, ref offsetZ)) { return; }

            GlobalAlignmentRules.AlignmentFromPoints VectorAlign = new GlobalAlignmentRules.AlignmentFromPoints(atractorPoints, domain);



            GH_Alignment Alignment = new GH_Alignment(new Alignment("", OffsetY, offsetZ, VectorAlign.GenerateVector));

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
                // return Resources.IconForThisComponent;
                return null;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("205f540f-aaca-4fcd-8632-a0eb5c095c0b"); }
        }
    }
}
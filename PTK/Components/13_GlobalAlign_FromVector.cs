using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace PTK.Components
{
    public class _13_GlobalAlign_FromVector : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the _13_GlobalAlign_FromVector class.
        /// </summary>
        public _13_GlobalAlign_FromVector()
          : base("AlignFromVector", "V",
              "Aligns Z-vector from surface",
              CommonProps.category, CommonProps.subcate12)
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddVectorParameter("Vector", "V", "Add alignment vector here", GH_ParamAccess.item);
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
            Vector3d alignVector = new Vector3d();
            double OffsetY = 0;
            double offsetZ = 0;


            if (!DA.GetData(0, ref alignVector)) { return; }
            if (!DA.GetData(1, ref OffsetY)) { return; }
            if (!DA.GetData(2, ref offsetZ)) { return; }


            GlobalAlignmentRules.AlignmentFromVector VectorAlign = new GlobalAlignmentRules.AlignmentFromVector(alignVector);



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
            get { return new Guid("56f55553-217a-4261-87db-21cbb5856f31"); }
        }
    }
}
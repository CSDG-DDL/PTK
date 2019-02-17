//using System;
//using System.Collections.Generic;

//using Grasshopper.Kernel;
//using Rhino.Geometry;

//namespace PTK
//{
//    public class PTK_LocalAnalysis : GH_Component
//    {
//        /// <summary>
//        /// Initializes a new instance of the PTK_C_03 class.
//        /// </summary>
//        public PTK_LocalAnalysis()
//          : base("Local Analysis (PTK)", "Local Analysis",
//              "Local Analysis",
//              CommonProps.category, CommonProps.subcate5)
//        {
//            Message = CommonProps.initialMessage;
//        }

//        /// <summary>
//        /// Registers all the input parameters for this component.
//        /// </summary>
//        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
//        {
//            pManager.AddGenericParameter("PTK Assembly", "A (PTK)", "PTK DATA INPUT", GH_ParamAccess.item);

//        }

//        /// <summary>
//        /// Registers all the output parameters for this component.
//        /// </summary>
//        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
//        {
//            pManager.AddGenericParameter("PTK Local Analysis", "LA (PTK)", "PTK Local Analysis", GH_ParamAccess.item);

//        }

//        /// <summary>
//        /// This is the method that actually does the work.
//        /// </summary>
//        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
//        protected override void SolveInstance(IGH_DataAccess DA)
//        {
//        }

//        /// <summary>
//        /// Provides an Icon for the component.
//        /// </summary>
//        protected override System.Drawing.Bitmap Icon
//        {
//            get
//            {
//                //You can add image files to your project resources and access them like this:
//                // return Resources.IconForThisComponent;
//                return PTK.Properties.Resources.LocalAnalysis;
//            }
//        }

//        /// <summary>
//        /// Gets the unique ID for this component. Do not change this ID after release.
//        /// </summary>
//        public override Guid ComponentGuid
//        {
//            get { return new Guid("9b623fe7-191b-4163-800c-2cb85fef0c2b"); }
//        }
//    }
//}
using System;
using System.Collections.Generic;
using System.Linq;
using Grasshopper.Kernel;
using Rhino.Geometry;
using Google.OrTools.LinearSolver;

namespace PTK
{
    public class PTK_Optimization : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the PTK_C_03 class.
        /// </summary>
        public PTK_Optimization()
          : base("Otpimization (PTK)", "Optimization",
              "Optimization Analysis",
              CommonProps.category, CommonProps.subcate5)
        {
            Message = CommonProps.initialMessage;
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddIntegerParameter("n1", "n1", "number of dowels in the row", GH_ParamAccess.item,4);
            pManager.AddIntegerParameter("n2", "n2", "number of rows", GH_ParamAccess.item,6);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("report", "rep", "PTK Local Analysis", GH_ParamAccess.list);
            pManager.AddNumberParameter("FvRd", "FRd", "The force capacity form the group of dowels", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            int nx = 0;
            int ny = 0;

            DA.SetData(0,  nx);
            DA.SetData(1,  ny);

            List<string> outputs = new List<string>();
            
            

            //publish data
            DA.SetDataList(0, outputs);
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
                return PTK.Properties.Resources.LocalAnalysis;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("9b223fe7-191b-4161-800c-2cb85fef0c2b"); }
        }
    }
}
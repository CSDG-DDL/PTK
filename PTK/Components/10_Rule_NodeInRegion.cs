using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace PTK.Components
{
    public class PTK_Rule_NodeInRegion : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the _11_04_NodeInRegion class.
        /// </summary>
        public PTK_Rule_NodeInRegion()
          : base("NodeInRegion", "NodeInRegion",
              "Detail search checking if the details node is within one of the regions.",
              CommonProps.category, CommonProps.subcate10)
        {
        }
        /// <summary>
        /// Overrides the exposure level in the components category 
        /// </summary>
        public override GH_Exposure Exposure
        {
            get
            { return GH_Exposure.tertiary; }
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("Region(s)", "R", "Region(s) to test ", GH_ParamAccess.list);
            pManager.AddNumberParameter("Max Dist", "D", "Optional. Maximum distance between the node and regions plane. Set to 0 for strict evaluation", GH_ParamAccess.item, 9999999);

        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("SearchCriteria", "SC", "Search Criteria for DetailSearch", GH_ParamAccess.item);

        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            //Variables
            List<Curve> InputCurves = new List<Curve>();
            double MaxDist = 99999999;

            //Input
            DA.GetDataList(0, InputCurves);
            DA.GetData(1, ref MaxDist);

            //Solve
            Rules.NodeHitRegion Rule = new Rules.NodeHitRegion(InputCurves, MaxDist);

            //Output
            DA.SetData(0, new Rules.Rule(new CheckGroupDelegate(Rule.check)));   //Sending a new checkgroupDelegate through a new rule object




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
                return Properties.Resources.RegionRule;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("101e862b-9aea-487e-be17-0f3b9474ae0f"); }
        }
    }
}
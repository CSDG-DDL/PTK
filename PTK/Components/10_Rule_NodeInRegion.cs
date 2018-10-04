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
          : base("NodeInRegion", "Nickname",
              "Description",
              CommonProps.category, CommonProps.subcate10)
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("Region", "R", "Region to test ", GH_ParamAccess.list);


        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Rule", "R", "DetailingGroupRule", GH_ParamAccess.item);

        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            //Variables
            List<Curve> InputCurves = new List<Curve>();


            //Input
            DA.GetDataList(0, InputCurves);

            //Solve
            Rules.NodeHitRegion Rule = new Rules.NodeHitRegion(InputCurves);

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
                return Properties.Resources.SearchDetail;
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
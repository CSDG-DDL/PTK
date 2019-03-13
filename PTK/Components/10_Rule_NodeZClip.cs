using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace PTK.Components
{
    public class PTK_Rule_NodeZClip : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the _11_04_NodeZClip class.
        /// </summary>
        public PTK_Rule_NodeZClip()
          : base("NodeZclip", "NodeZClip",
              "Uses Z value of the node within given range. Useful for finding supports.",
              CommonProps.category, CommonProps.subcate10)
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("Min Z","Min", "Minium Z value",GH_ParamAccess.item, -1);
            pManager.AddNumberParameter("Max Z","Max","Maximum Z value",GH_ParamAccess.item, 9999999);

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
            double MinZ = -1;
            double MaxZ = 99999999;

            //Input
            DA.GetData(0, ref MinZ);
            DA.GetData(1, ref MaxZ);


            //Solve
            Rules.NodeZClip Rule = new Rules.NodeZClip(MinZ,MaxZ);

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
            get { return new Guid("886e17c0-4787-4cb7-bfbb-3112ec9de815"); }
        }
    }
}
﻿using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace PTK.Components
{
    public class _10_Rule_ForceBendingMY : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the _10_Rule_ForceBendingMY class.
        /// </summary>
        public _10_Rule_ForceBendingMY()
          : base("ElementForceBending MY", "MY",
              "Detail search based on bending moment (MY) in the element's detail",
              CommonProps.category, CommonProps.subcate10)
        {
        }
        /// <summary>
        /// Overrides the exposure level in the components category 
        /// </summary>
        public override GH_Exposure Exposure
        {
            get
            { return GH_Exposure.quarternary; }
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("Min Bending moment", "<", "Minimum tending moment in element", GH_ParamAccess.item, 0);
            pManager.AddNumberParameter("Max Bending moment", ">", "Maximum Bending moment in element", GH_ParamAccess.item, 10000000);
            pManager.AddBooleanParameter("All Elements", "A", "True: All elements must be within the domain. False: Only one element must be inside the domain", GH_ParamAccess.item, true);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("SearchCriteria", "SC", "Search Criteria for DetailSearch", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {

            //Variables 
            double minAmount = 0;
            double maxAmount = 10000000;
            bool all = false;


            //Input 
            DA.GetData(0, ref minAmount);
            DA.GetData(1, ref maxAmount);
            DA.GetData(2, ref all);

            //CHANGE
            Rules.ForceMode Mode = Rules.ForceMode.BendingDir1;
            //CHANGE

            //Solve 

            Rules.ElementForce Rule = new Rules.ElementForce(Mode, minAmount, maxAmount, all);


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
            get { return new Guid("e715006d-8907-49b1-9b84-ca5afe591ff8"); }
        }
    }
}
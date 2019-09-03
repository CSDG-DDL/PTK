using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace PTK.Components
{
    public class PTK_Rule_ElementAmount : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the _10_Rule_ElementLength class.
        /// </summary>
        public PTK_Rule_ElementAmount()
          : base("ElementAmountRule", "A",
              "Detail search based on amount of elements at the detail",
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
            pManager.AddIntegerParameter("Min Amount", "<", "Minimum element amount allowed", GH_ParamAccess.item, -1);
            pManager.AddIntegerParameter("Max Amount", ">", "Maximum element amount allowed", GH_ParamAccess.item, 10000000);
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
            int minAmount = -1;
            int maxAmount = 10000000;


            //Input 
            DA.GetData(0, ref minAmount);
            DA.GetData(1, ref maxAmount);


            //Solve 
            Rules.ElementAmount Rule = new Rules.ElementAmount(minAmount, maxAmount);


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
                return Properties.Resources.AmountRule;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("88804a53-0d2b-4387-b338-4f4d7fe49ed3"); }
        }
    }
}
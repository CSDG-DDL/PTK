using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace PTK.Components
{
    public class PTK_Rule_ElementLength : GH_Component
    {

        /// <summary>
        /// Initializes a new instance of the _10_Rule_ElementLength class.
        /// </summary>
        public PTK_Rule_ElementLength()
          : base("ElementLengthRule", "L",
              "Detail search based on the length of the elements",
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
            pManager.AddNumberParameter("Min Length", "<", "Minimum element length allowed", GH_ParamAccess.item,-1);
            pManager.AddNumberParameter("Max Length", ">", "Maximum element length allowed", GH_ParamAccess.item,10000000);   
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
            double minLength = -1;
            double maxlength = 10000000;

            
            //Input 
            DA.GetData(0, ref minLength);
            DA.GetData(1, ref maxlength);

            
            //Solve 
            Rules.ElementLength Rule = new Rules.ElementLength(minLength, maxlength);


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
            get { return new Guid("d982bbed-ef20-44d1-8003-8918b3bd7543"); }
        }
    }
}
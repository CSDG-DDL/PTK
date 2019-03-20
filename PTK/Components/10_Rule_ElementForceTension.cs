using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace PTK.Components
{
    public class _10_Rule_ElementForceTension : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the _10_RuleElementCompressionForce class.
        /// </summary>
        public _10_Rule_ElementForceTension()
          : base("Detail shape", "S",
              "Makes you select details based on typical shapes",
              CommonProps.category, CommonProps.subcate10)
        {
        }
        /// <summary>
        /// Overrides the exposure level in the components category 
        /// </summary>
        public override GH_Exposure Exposure
        {
            get
            { return GH_Exposure.hidden; }
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddIntervalParameter("Min/max comp force", "CF", "Ddsd", GH_ParamAccess.item);
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
            Interval forceinterval = new Interval();
            








            //Input
            DA.GetData(0, ref forceinterval);


            //Solve
            Rules.ElementForce Rule = new Rules.ElementForce(forceinterval); 
                
         

            //Output
            DA.SetData(0, new Rules.Rule(new CheckGroupDelegate(Rule.check)));





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
            get { return new Guid("f099152b-1290-4d2f-a950-ad522fe027c2"); }
        }
    }
}
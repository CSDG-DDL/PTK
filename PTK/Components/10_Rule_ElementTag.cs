using System;
using System.Collections.Generic;
using Grasshopper.Documentation;
using Grasshopper.Kernel;
using Karamba.Utilities.UIWidgets;
using Rhino.Geometry;

namespace PTK.Components
{
    public class _10_Rule_ElementTag : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the _11_03_ElementTag class.
        /// </summary>
        public _10_Rule_ElementTag()
          : base("ElementTagRule", "T",
              "Detail search checking the tag of its elements. Several tags can be inputted for different modes.",
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
            pManager.AddTextParameter("Tags Are", "T", "Input the tags the element must contain", GH_ParamAccess.list, "N/A");
            pManager.AddIntegerParameter("Mode", "M", "Mode 0 - One of - The detail must contain one of the inputted tags. " +
                                                         "Mode 1 - At least -  The detail must contain all the inputted tags, but can also contain other tags. " +
                                                         "Mode 2 - Distinct - The detail must contain all the inputted tags and no other tags. " +
                                                         "Mode 3 - Strict - The detai must contain all the inputted tags and the exact amount. ", GH_ParamAccess.item,0);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("DetailDescription", "DD", "Outputed detailDescription", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {

            //Variables
            List<string> tagsAre = new List<string>();
            int strict = 4;

            //Input
            DA.GetDataList(0, tagsAre);
            DA.GetData(1, ref strict);

            //Solve
            Rules.ElementTag Rule = new Rules.ElementTag(tagsAre, strict);

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
                return Properties.Resources.TagRule;
            }
        }


        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("bd55f4e2-4775-4a03-b2e6-6209d3e8917e"); }
        }
    }
}
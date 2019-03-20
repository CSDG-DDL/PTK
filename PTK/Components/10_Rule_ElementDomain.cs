using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace PTK.Components
{
    public class _10_Rule_ElementDomain : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the _10_Rule_ElementDomain class.
        /// </summary>
        public _10_Rule_ElementDomain()
          : base("Detail typology", "T",
              "Detail search based on typical detail toplogies or shapes, like an L, T or X detail.",
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
            pManager.AddIntegerParameter("Mode", "M",    "Select type of detail: \n"+
                                                         "Mode 0 - Two elements connected in an end-point, L-node. \n" +
                                                         "Mode 1 - One element connected in the end, one element connected on the element, T-node\n " +
                                                         "Mode 2 - Two elements connected on the element, X-node\n" +
                                                         "Mode 3 - Nodes with single elements conencted, End-node\n " +
                                                         "Mode 4 - 3 or more elements connected in the end, *Star-Node \n" +
                                                         "Mode 5 - Detail is planar \n" +
                                                         "Mode 6 - Detail is orthogonal \n", GH_ParamAccess.item, 0);
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
            List<Interval> intervals = new List<Interval>();
            int mode = 0;

            






            //Input
            DA.GetData(0, ref mode);


            //Solve
            Rules.ElementOnNodeDomains Rule = new Rules.ElementOnNodeDomains(mode);

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
                return Properties.Resources.SearchDetail;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("fc932f92-bddc-495c-9526-18885c8ee40c"); }
        }
    }
}
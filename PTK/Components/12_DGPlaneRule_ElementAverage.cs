using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace PTK.Components
{
    public class _12_DGPlaneRule_ElementAverage : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the _12_DGPlaneRule_ElementAverage class.
        /// </summary>
        public _12_DGPlaneRule_ElementAverage()
          : base("DetailPlaneNormalFromElementAverage", "DetNorElem",
              "Sets the detail z-axis to surface normal at the closest point to the detail node. Optional Align X-axis along an element.",
              CommonProps.category, CommonProps.subcate10)
        {
        }

        /// <summary>
        /// Overrides the exposure level in the components category 
        /// </summary>
        public override GH_Exposure Exposure
        {
            get
            { return GH_Exposure.secondary; }
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Name of AlignmentElement for x-axis", "X", "Optional. Name of an element to align the detail plane x-axis along", GH_ParamAccess.item);
            pManager[0].Optional = true;
        }


        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("PlaneRule", "PR", "PlaneRule", GH_ParamAccess.item);

        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {

            //Variables 
            string AlignmentElementName = "";


            //Input 
            DA.GetData(0, ref AlignmentElementName);


            //Solve 
            PlaneRules.ElementAverage PlaneRule = new PlaneRules.ElementAverage(AlignmentElementName);


            //Output
            DA.SetData(0, new PlaneRules.PlaneRule(PlaneRule.GenerateDetailingGroupPlane));


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
                return PTK.Properties.Resources.DPNEleAve;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("a3cd6ca7-e44c-403e-9631-5d5ffb32972a"); }
        }
    }
}
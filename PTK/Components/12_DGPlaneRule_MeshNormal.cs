using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace PTK.Components
{
    public class _12_DGPlaneRule_MeshNormal : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the _12_DGPlaneRule_MeshNormal class.
        /// </summary>
        public _12_DGPlaneRule_MeshNormal()
          : base("DetailPlaneNormalFromMesh", "DetNorMsh",
              "Sets the detail z-axis to Mesh normal at the closest point to the detail node. Optional Align X-axis along an element.",
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
            pManager.AddMeshParameter("MeshGuide", "M", "Input Mesh Guide", GH_ParamAccess.item);
            pManager.AddTextParameter("Name of AlignmentElement for x-axis", "X", "Optional. Name of an element to align the detail plane x-axis along", GH_ParamAccess.item);
            pManager[1].Optional = true;
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
            Mesh Mesh = null;
            string AlignmentElementName = "";


            //Input 
            DA.GetData(0, ref Mesh);
            DA.GetData(1, ref AlignmentElementName);


            //Solve 
            PlaneRules.MeshNormalPlane PlaneRule = new PlaneRules.MeshNormalPlane(Mesh, AlignmentElementName);


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
                return PTK.Properties.Resources.Alignment;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("54ec09ea-7dd8-409d-a6af-59aa55c30817"); }
        }
    }
}
using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace PTK.Components
{
    public class _13_GlobalAlign_FromSurfaces : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the _13_GlobalAlign_FromSurfaces class.
        /// </summary>
        public _13_GlobalAlign_FromSurfaces()
          : base("GlobalAlignFromSurface", "S",
              "Aligns the element Z-vector to a surface normal at closest point from middle of element",
              CommonProps.category, CommonProps.subcate2)
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
            pManager.AddSurfaceParameter("Surfaces", "S", "Add alignment surfaces here", GH_ParamAccess.list);
            pManager.AddNumberParameter("Max Distance", "D", "Maximum allowed distance to let surface deside alignment. If too large, Z-alignment is used", GH_ParamAccess.item, 1000);
            pManager.AddNumberParameter("Offset Width(Y)", "OY", "Add negative or positive value to offset crosssection/composite from center-curve", GH_ParamAccess.item,0);
            pManager.AddNumberParameter("Offset Height(Z)", "OZ", "Add negative or positive value to offset crosssection/composite from center-curve", GH_ParamAccess.item,0);

        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("GlobalAlignmenrt", "A", "Add global alignment to element", GH_ParamAccess.item);
        }

               
        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<Surface> AlignmentSurfaces = new List<Surface>();
            double MaxDistance = 0; 
            double OffsetY = 0;
            double offsetZ = 0;


            if (!DA.GetDataList(0, AlignmentSurfaces)) { return; }
            if (!DA.GetData(1, ref MaxDistance)) { return; }
            if (!DA.GetData(2, ref OffsetY)) { return; }
            if (!DA.GetData(3, ref offsetZ)) { return; }

            GlobalAlignmentRules.AlignmentFromSurfaces SurfaceAlignment = new GlobalAlignmentRules.AlignmentFromSurfaces(AlignmentSurfaces, 100);

            

            GH_Alignment Alignment = new GH_Alignment(new Alignment("", OffsetY, offsetZ, SurfaceAlignment.GenerateVector));

            DA.SetData(0, Alignment);



        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                return PTK.Properties.Resources.Alignment;

            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("166794ca-5d22-4197-aca5-9da47c0bacad"); }
        }
    }
}
using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace PTK.Components
{
    public class ExtractCrossSection : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the ExtractCrossSection class.
        /// </summary>
        public ExtractCrossSection()
          : base("ExtractCrossSection", "CS",
              "Description",
              CommonProps.category, CommonProps.subcate8)
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddParameter(new Param_CroSec(), "CrossSection", "CS", "Add cross-section from element extraction", GH_ParamAccess.item);


        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddNumberParameter("Width", "W", "Extract Width(s)", GH_ParamAccess.list);
            pManager.AddNumberParameter("Height", "H", "Extract Height(s)", GH_ParamAccess.list);
            pManager.AddNumberParameter("OffsetY", "OY", "Extract OffsetY", GH_ParamAccess.list);
            pManager.AddNumberParameter("OffsetZ", "OZ", "Extract OffsetZ", GH_ParamAccess.list);



        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            GH_CroSec GHCrossection = null;
            


            if (!DA.GetData(0, ref GHCrossection)) { return; }
            CrossSection CrossSection = GHCrossection.Value;

            
            List<double> Width = new List<double>();
            List<double> Height = new List<double>();
            List<double> OffsetY = new List<double>();
            List<double> OffsetZ = new List<double>();





            if (CrossSection is Composite comp)
            {
                List<Tuple<CrossSection, Alignment>> secs = comp.RecursionCrossSectionSearch();
                foreach (var s in secs)
                {

                    Width.Add(s.Item1.GetWidth());
                    Height.Add(s.Item1.GetHeight());
                    OffsetY.Add(s.Item1.Alignment.OffsetY);
                    OffsetZ.Add(s.Item1.Alignment.OffsetZ);

                }
            }
            else
            {
                Width.Add(CrossSection.GetWidth());
                Height.Add(CrossSection.GetHeight());
                OffsetY.Add(CrossSection.Alignment.OffsetY);
                OffsetZ.Add(CrossSection.Alignment.OffsetZ);
                

            }

            DA.SetDataList(0, Width);
            DA.SetDataList(1, Height);
            DA.SetDataList(2, OffsetY);
            DA.SetDataList(3, OffsetZ);


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
                return PTK.Properties.Resources.RectangleCS;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("299db316-121c-423e-8473-3f1ffe464947"); }
        }
    }
}
using System;
using System.Collections.Generic;
using Grasshopper.Documentation;
using Grasshopper.Kernel;
using Karamba.Utilities.UIWidgets;
using Rhino.Geometry;

namespace PTK.Components
{
    public class _10_Rule_ElementAngle : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the _11_03_ElementTag class.
        /// </summary>
        public _10_Rule_ElementAngle()
          : base("Element Angle", "EA",
              "Checks the angle of the all neighbouring elements. Returns details with elements inside the range",
              CommonProps.category, CommonProps.subcate10)
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {

            pManager.AddIntegerParameter("Minimum Angle", "Min", "The minimum angle between two elements", GH_ParamAccess.item, 0);
            pManager.AddIntegerParameter("Maximum Angle", "Max", "The maximum angle allowed between two elements",GH_ParamAccess.item, 360);
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
            int minimumAngle = 0;
            int maximumAngle = 360;
            //plane plane = Plane.WorldXY;


            //Inputs
            DA.GetData(0, ref minimumAngle);
            DA.GetData(1, ref maximumAngle);
            //    DA.GetData(2,ref mode);
            //    DA.GetData(3, ref plane);

            
            //Solve 
            Rules.ElementAngle Rule = new Rules.ElementAngle(minimumAngle, maximumAngle);
            

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
            get { return new Guid("7dbd4d6c-34e1-4b54-98bc-d08d5178ae32"); }
        }
    }
}
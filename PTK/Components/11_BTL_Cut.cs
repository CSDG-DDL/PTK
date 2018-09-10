using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace PTK
{
    public class _11_BTL_Cut : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the _11_BTL_Cut class.
        /// </summary>
        public _11_BTL_Cut()
          : base("_11_BTL_Cut", "Nickname",
              "Description",
              CommonProps.category, CommonProps.subcate11)
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Element", "E", "", GH_ParamAccess.item);
            pManager.AddPlaneParameter("P", "CutPlane", "", GH_ParamAccess.item);
            
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("B", "BTL-Cut", "", GH_ParamAccess.item);
            
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {

            ElementInDetail temp = new ElementInDetail(); 
            Element1D Element = new Element1D();
            Plane Plane = new Plane();

            DA.GetData(0, ref temp);
            DA.GetData(1, ref Plane);

            Element = temp.Element;
            List<Element1D> test = new List<Element1D>();

            

            BTLCut cut = new BTLCut(Plane);
             


            // Making Object with delegate and ID
            OrderedTimberProcess Order = new OrderedTimberProcess(Element, new PerformTimberProcessDelegate(cut.DelegateProcess));


            DA.SetData(0, Order);

            
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
                return Properties.Resources.Cut;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("9ef81ec8-a7e3-4ced-8ed2-fb05a2fd5ef7"); }
        }
    }
}
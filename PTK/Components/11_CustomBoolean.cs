using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace PTK.Components
{
    public class _11_CustomBoolean : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the _11_CustomBoolean class.
        /// </summary>
        public _11_CustomBoolean()
          : base("CustomSubtraction", "CS",
              "Add a Brep here to subtract material from the element. NB!! This subtractive operation does not export to BTL!!!!",
              CommonProps.category, CommonProps.subcate11)
        {
            Message = CommonProps.initialMessage;
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddParameter(new Param_Element1D(), "Element", "E", "Element", GH_ParamAccess.item);
            pManager.AddBrepParameter("SubtractionVolume", "B", "Add subtraction volume to if BTL is not apropriate. NB! NB! This operation will not be included in the BTL!!!", GH_ParamAccess.item);

            pManager[0].Optional = true;
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("BTL-Cut", "B", "", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // --- variables ---
            GH_Element1D gElement = null;
            Brep brepshape = new Brep();
      

            // --- input --- 
            if (!DA.GetData(0, ref gElement)) { return; }
            Element1D element = gElement.Value;
            if (!DA.GetData(1, ref brepshape)) { return; }





            // --- solve ---
            CustomBrep BrepShape = new CustomBrep(brepshape);

            // Making Object with delegate and ID
            OrderedTimberProcess Order = new OrderedTimberProcess(element, new PerformTimberProcessDelegate(BrepShape.DelegateProcess));

            // --- output ---
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
                return null;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("dab4779e-7601-4f2f-939f-3ae9293221ba"); }
        }
    }
}
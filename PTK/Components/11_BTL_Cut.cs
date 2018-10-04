using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace PTK
{
    public class PTK_BTL_Cut : GH_Component
    {
        public PTK_BTL_Cut()
          : base("BTL Cut", "Cut",
              "Define the Cut process",
              CommonProps.category, CommonProps.subcate11)
        {
            Message = CommonProps.initialMessage;
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddParameter(new Param_Element1D(), "Element", "E", "Element", GH_ParamAccess.item);
            pManager.AddPlaneParameter("Cut Plane", "P", "Cut Plane", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("BTL-Cut", "B", "", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // --- variables ---
            GH_Element1D gElement = null;
            Plane Plane = new Plane();

            // --- input --- 
            if (!DA.GetData(0, ref gElement)) { return; }
            Element1D element = gElement.Value;
            if (!DA.GetData(1, ref Plane)) { return; }

            // --- solve ---
            BTLCut cut = new BTLCut(Plane);

            // Making Object with delegate and ID
            OrderedTimberProcess Order = new OrderedTimberProcess(element, new PerformTimberProcessDelegate(cut.DelegateProcess));

            // --- output ---
            DA.SetData(0, Order);
        }

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return Properties.Resources.Cut;
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("9ef81ec8-a7e3-4ced-8ed2-fb05a2fd5ef7"); }
        }
    }
}
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
            pManager.AddGenericParameter("Element", "E", "", GH_ParamAccess.item);
            pManager.AddPlaneParameter("CutPlane", "P", "", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("BTL-Cut", "B", "", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // --- variables ---
            ElementInDetail temp = new ElementInDetail(); 
            Element1D Element = new Element1D();
            Plane Plane = new Plane();

            // --- input --- 
            if (!DA.GetData(0, ref temp)) { return; }
            Element = temp.Element;
            if (!DA.GetData(1, ref Plane)) { return; }

            // --- solve ---

            BTLCut cut = new BTLCut(Plane);

            // Making Object with delegate and ID
            OrderedTimberProcess Order = new OrderedTimberProcess(Element, new PerformTimberProcessDelegate(cut.DelegateProcess));

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
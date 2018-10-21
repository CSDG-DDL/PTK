using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace PTK.Components
{
    public class _11_BTL_Tenon : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the _11_BTL_Tenon class.
        /// </summary>
        public _11_BTL_Tenon()
          : base("Tenon", "T",
              "Define Tenon process",
              CommonProps.category, CommonProps.subcate11)
        {
            Message = CommonProps.initialMessage;
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddParameter(new Param_Element1D(), "Element", "E", "Element", GH_ParamAccess.item);
            pManager.AddPlaneParameter("TenonPlane", "P", "Add TenonPlane, X-axis equals length-direction", GH_ParamAccess.item);
            pManager.AddNumberParameter("Length", "L", "Add Length-parameter", GH_ParamAccess.item);
            pManager.AddNumberParameter("Width", "W", "Add Width-parameter", GH_ParamAccess.item);
            pManager.AddNumberParameter("Height", "H", "Add Height-parameter", GH_ParamAccess.item);
            pManager.AddBooleanParameter("ExtendTop?", "T", "Extend tenon upwards?", GH_ParamAccess.item, false);
            pManager.AddBooleanParameter("ExtendBottom?", "B", "Extend tenon Downwards?", GH_ParamAccess.item, false);
            pManager.AddBooleanParameter("Chamfer?", "C", "Chamfer end tip of tenon?", GH_ParamAccess.item, false);
            pManager.AddNumberParameter("Shaperadius", "R", "Add shaperadius. (Active if Shapemode is set to Radius)", GH_ParamAccess.item, 20);
            pManager.AddIntegerParameter("Shapemode", "M", "Choose Shapemode. 0=Automatic, 1=Square, 2=Round, 3=rounded by tool radius, 4=Radius-based",GH_ParamAccess.item, 0);
            pManager[5].Optional = true;
            pManager[6].Optional = true;
            pManager[7].Optional = true;
            pManager[8].Optional = true;
            pManager[9].Optional = true;

        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("BTL-Tenon", "T", "", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // --- variables ---
            GH_Element1D gElement = null;


            Plane TenonPlane = new Plane();
            double Length = 30;
            double Width = 30;
            double Height =30;
            bool LengthLimitedTop = true;
            bool LengthLimitedBottom = true;
            bool Chamfer = false ;
            double Shaperadius = 0 ;
            int Shapetype = 0;


            // --- input --- 
            if (!DA.GetData(0, ref gElement)) { return; }
            Element1D element = gElement.Value;

            if (!DA.GetData(1, ref TenonPlane)) { return; }
            if (!DA.GetData(2, ref Length)) { return; }
            if (!DA.GetData(3, ref Width)) { return; }
            if (!DA.GetData(4, ref Height)) { return; }
            DA.GetData(5, ref LengthLimitedTop);
            DA.GetData(6, ref LengthLimitedBottom);
            DA.GetData(7, ref Chamfer);
            DA.GetData(8, ref Shaperadius);
            DA.GetData(9, ref Shapetype);


            BooleanType top = BooleanType.no;
            BooleanType bottom = BooleanType.no;
            BooleanType chamfer = BooleanType.no;

            if (!LengthLimitedTop) { top = BooleanType.yes; }
            if (!LengthLimitedBottom) { bottom = BooleanType.yes; }
            if (Chamfer) { chamfer = BooleanType.yes; }


            // --- solve ---
            BTLTenon tenon = new BTLTenon(TenonPlane, Width, Length, Height, top, bottom, chamfer, Shapetype, Shaperadius);

            // Making Object with delegate and ID
            OrderedTimberProcess Order = new OrderedTimberProcess(element, new PerformTimberProcessDelegate(tenon.DelegateProcess));

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
            get { return new Guid("fedb6ee7-dad6-406f-8611-10b9eef05f16"); }
        }
    }
}
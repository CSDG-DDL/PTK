using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace PTK.Components
{
    public class _11_BTL_DoveTailMortise : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the _11_BTL_DoveTailMortise class.
        /// </summary>
        public _11_BTL_DoveTailMortise()
          : base("DovetailMortise", "DM",
              "Makes a Dovetailmortise",
              CommonProps.category, CommonProps.subcate11)
        {
            Message = CommonProps.initialMessage;
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddParameter(new Param_Element1D(), "Element", "E", "Element", GH_ParamAccess.item);
            pManager.AddPlaneParameter("MortisePlane", "P", "Add TenonPlane, X-axis equals length-direction", GH_ParamAccess.item);
            pManager.AddNumberParameter("Length", "L", "Add Length-parameter", GH_ParamAccess.item);
            pManager.AddNumberParameter("Width", "W", "Add Width-parameter", GH_ParamAccess.item);
            pManager.AddNumberParameter("Depth", "H", "Add Height-parameter", GH_ParamAccess.item);
            pManager.AddBooleanParameter("ExtendTop?", "T", "Extend tenon upwards?", GH_ParamAccess.item, false);
            pManager.AddBooleanParameter("ExtendBottom?", "B", "Extend tenon Downwards?", GH_ParamAccess.item, false);
            pManager.AddNumberParameter("Shaperadius", "R", "Add shaperadius. (Active if Shapemode is set to Radius)", GH_ParamAccess.item, 20);
            pManager.AddIntegerParameter("Shapemode", "M", "Choose Shapemode. 0=Automatic, 1=Square, 2=Round, 3=rounded by tool radius, 4=Radius-based", GH_ParamAccess.item, 0);
            pManager.AddNumberParameter("FlankAngle", "FA", "The angle (min 5, max 35) of the tool that makes the dovetail", GH_ParamAccess.item,15);
            pManager.AddNumberParameter("ConeAngle", "CA", "The angle(max 30) of the cone (reduction of size from the startpoint", GH_ParamAccess.item,0);
            pManager.AddBooleanParameter("Flank active?", "A", "If not active, the flank angle is zero", GH_ParamAccess.item, true);
            pManager.AddBooleanParameter("FlipDirection?", "F", "set True if you want to flip direction of the Mortise", GH_ParamAccess.item, false);
            pManager[5].Optional = true;
            pManager[6].Optional = true;
            pManager[7].Optional = true;
            pManager[8].Optional = true;

        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("BTL-DovetailMortise", "DM", "", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // --- variables ---
            GH_Element1D gElement = null;


            Plane DoveTailMortisePlane = new Plane();
            double Length = 30;
            double Width = 30;
            double Depth = 30;
            bool LengthLimitedTop = true;
            bool LengthLimitedBottom = true;
            double Shaperadius = 0;
            int Shapetype = 0;
            double FlankAngle = 0;
            double ConeAngle = 0;
            bool FlankActive = false;

            bool FlipPlane = false;



            // --- input --- 
            if (!DA.GetData(0, ref gElement)) { return; }
            Element1D element = gElement.Value;

            if (!DA.GetData(1, ref DoveTailMortisePlane)) { return; }
            if (!DA.GetData(2, ref Length)) { return; }
            if (!DA.GetData(3, ref Width)) { return; }
            if (!DA.GetData(4, ref Depth)) { return; }
            DA.GetData(5, ref LengthLimitedTop);
            DA.GetData(6, ref LengthLimitedBottom);
            DA.GetData(7, ref Shaperadius);
            DA.GetData(8, ref Shapetype);
            DA.GetData(9, ref FlankAngle);
            DA.GetData(10, ref ConeAngle);
            DA.GetData(11, ref FlankActive);
            DA.GetData(12, ref FlipPlane);


            BooleanType top = BooleanType.no;
            BooleanType bottom = BooleanType.no;
            BooleanType chamfer = BooleanType.no;

            if (!LengthLimitedTop) { top = BooleanType.yes; }
            if (!LengthLimitedBottom) { bottom = BooleanType.yes; }



            // --- solve ---
            BTLDovetailMortise DovetailMortise = new BTLDovetailMortise(DoveTailMortisePlane, Width, Length, Depth, top, bottom, Shapetype, Shaperadius, FlipPlane, ConeAngle, FlankAngle, FlankActive);
            

            // Making Object with delegate and ID
            OrderedTimberProcess Order = new OrderedTimberProcess(element, new PerformTimberProcessDelegate(DovetailMortise.DelegateProcess));

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
            get { return new Guid("95e4df22-3bd5-4b04-80fb-83dd1d305e20"); }
        }
    }
}
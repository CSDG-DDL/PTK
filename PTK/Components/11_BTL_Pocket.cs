using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace PTK.Components
{
    public class _11_BTL_Pocket : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the _11_BTL_Pocket class.
        /// </summary>
        public _11_BTL_Pocket()
            : base("Drill", "D",
              "Define the Drill process",
              CommonProps.category, CommonProps.subcate11)
        {
            Message = CommonProps.initialMessage;
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddParameter(new Param_Element1D(), "Element", "E", "Element", GH_ParamAccess.item);
            pManager.AddCurveParameter("Paralellogram", "", "", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("BTL-Drill", "D", "", GH_ParamAccess.item);
            pManager.AddBooleanParameter("", "", "", GH_ParamAccess.item);
            pManager.AddLineParameter("X", "", "", GH_ParamAccess.item);
            pManager.AddLineParameter("Y", "", "", GH_ParamAccess.item);
            pManager.AddPointParameter("PT", "", "", GH_ParamAccess.item);
            pManager.AddPlaneParameter("PL", "", "", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {

            // --- variables ---
            GH_Element1D gElement = null;
            Curve Parallellogram = null;

            


            // --- input --- 
            if (!DA.GetData(0, ref gElement)) { return; }
            Element1D element = gElement.Value;
            DA.GetData(1, ref Parallellogram);

            ////////////////////////////////////////////////////////////////
            ///THIS IS A TEMPORARY GENERATION OF AN ASSEMBLY! START

            List<Element1D> elems = new List<Element1D>();
            elems.Add(element);


            BTLPocket BTLPocket = new BTLPocket(Parallellogram, false, 90, 90, 90, 90);

            OrderedTimberProcess Order = new OrderedTimberProcess(element, new PerformTimberProcessDelegate(BTLPocket.DelegateProcess));
            List<OrderedTimberProcess> Orders = new List<OrderedTimberProcess>();
            Orders.Add(Order);

            BuildingProject GrasshopperProject = new BuildingProject(new ProjectType());
            GrasshopperProject.PrepairElements(elems, Orders);
            GrasshopperProject.ManufactureProject(ManufactureMode.BOTH);



            ///THIS IS A TEMPORARY GENERATION OF AN ASSEMBLY! END
            ////////////////////////////////////////////////////////////////

            
            DA.SetData(2, BTLPocket.X);
            DA.SetData(3, BTLPocket.Y);
            DA.SetData(4, BTLPocket.pt);
            DA.SetData(5, BTLPocket.refPlanepublic);










            bool check = BTLPocket.CheckParallogramValidity(Parallellogram);


            // --- output ---
            DA.SetData(0, Order);
            DA.SetData(1, check);
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
            get { return new Guid("bcf6f04d-b933-4966-b8ef-6434b15915be"); }
        }
    }
}
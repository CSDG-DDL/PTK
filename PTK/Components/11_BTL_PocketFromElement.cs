using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace PTK.Components
{
    public class _11_BTL_PocketFromElement : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the _11_BTL_PocketFromElement class.
        /// </summary>
        public _11_BTL_PocketFromElement()
            : base("PocketElement", "P",
              "Define the Pocket process from other element",
              CommonProps.category, CommonProps.subcate11)
        {
            Message = CommonProps.initialMessage;
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddParameter(new Param_Element1D(), "ElementToPocket", "E", "Insert the element that should be pocketed", GH_ParamAccess.item);
            pManager.AddParameter(new Param_Element1D(), "ElementThatPockets", "E", "Insert the element that is used to determine the pocket", GH_ParamAccess.item);
            pManager.AddNumberParameter("Tolerance ", "tb", "Extra cutout for tolerances", GH_ParamAccess.item, 0);
            

        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("BTL-Pocket", "D", "", GH_ParamAccess.item);

        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {

            // --- variables ---
            GH_Element1D gElement = null;
            GH_Element1D gElementOther = null;
            double Tolerance = 0;
            



            //Fix angles in btlx
            //Fix opposite direction of angles
            //Add extra component



            // --- input --- 
            if (!DA.GetData(0, ref gElement)) { return; }
            Element1D element = gElement.Value;

            if (!DA.GetData(1, ref gElementOther)) { return; }
            Element1D elementOther = gElementOther.Value;

            if (!DA.GetData(2, ref Tolerance)) { return; }
            






            BTLPocket BTLPocket = new BTLPocket(elementOther,Tolerance);

            OrderedTimberProcess Order = new OrderedTimberProcess(element, new PerformTimberProcessDelegate(BTLPocket.DelegateProcess));


            /*
            ////////////////////////////////////////////////////////////////
            ///THIS IS A TEMPORARY GENERATION OF AN ASSEMBLY! START
            List<Element1D> elems = new List<Element1D>();
            elems.Add(element);

            List<OrderedTimberProcess> Orders = new List<OrderedTimberProcess>();
            Orders.Add(Order);

            
            BuildingProject GrasshopperProject = new BuildingProject(new ProjectType());
            GrasshopperProject.PrepairElements(elems, Orders);
            GrasshopperProject.ManufactureProject(ManufactureMode.BOTH);



            

            
            DA.SetData(2, BTLPocket.X);
            DA.SetData(3, BTLPocket.Y);
            DA.SetData(4, BTLPocket.pt);
            DA.SetData(5, BTLPocket.refPlanepublic);
            ///THIS IS A TEMPORARY GENERATION OF AN ASSEMBLY! END
            ////////////////////////////////////////////////////////////////
            */



           

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
            get { return new Guid("7074242f-5d75-4226-b039-afe299da4aae"); }
        }
    }
}
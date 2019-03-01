using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace PTK.Components
{
    public class _11_BTL_Drill : GH_Component
    {
        Line PublicLine = new Line();
        double PublicRadius = 0;
        /// <summary>
        /// Initializes a new instance of the _11_BTL_Drill class.
        /// </summary>
        public _11_BTL_Drill()
          : base("Drill", "D",
              "Define the Drill process",
              CommonProps.category, CommonProps.subcate11)
        {
            Message = CommonProps.initialMessage;
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddParameter(new Param_Element1D(), "Element", "E", "Element", GH_ParamAccess.item);
            pManager.AddLineParameter("Drill Axis", "A", "Add drill axis (line)", GH_ParamAccess.item);
            pManager.AddNumberParameter("Drill Radius", "R", "Add Drill Radius", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("BTL-Drill", "D", "", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // --- variables ---
            GH_Element1D gElement = null;
            Line Line = new Line();
            double Radius = 0;
            

            // --- input --- 
            if (!DA.GetData(0, ref gElement)) { return; }
            Element1D element = gElement.Value;
            if (!DA.GetData(1, ref Line)) { return; }
            if (!DA.GetData(2, ref Radius)) { return; }

            // --- solve ---
            BTLDrill drill = new BTLDrill(Line, Radius);

            PublicLine = Line;
            PublicRadius = Radius;

            // Making Object with delegate and ID
            OrderedTimberProcess Order = new OrderedTimberProcess(element, new PerformTimberProcessDelegate(drill.DelegateProcess));
            
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
            get { return new Guid("02f5f402-a02c-4b74-89e8-d59b0323a12f"); }
        }

        public override void ExpireSolution(bool recompute)
        {

            base.ExpireSolution(recompute);
        }


        public override void DrawViewportMeshes(IGH_PreviewArgs args)
        {

            if (PublicLine.IsValid)
            {


                Plane tempPlane = new Plane(PublicLine.From, PublicLine.Direction);
                Circle tempCircle = new Circle(tempPlane, PublicRadius);
                Cylinder tempCylinder = new Cylinder(tempCircle, PublicLine.Length);

                args.Display.DepthMode = Rhino.Display.DepthMode.AlwaysInFront;
                args.Display.DrawBrepShaded(tempCylinder.ToBrep(false,false), new Rhino.Display.DisplayMaterial(System.Drawing.Color.Red));


            }


        }

    }
}
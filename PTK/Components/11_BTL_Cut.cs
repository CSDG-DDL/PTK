﻿using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace PTK
{
    public class PTK_BTL_Cut : GH_Component
    {
        List<Plane> publicPlane = new List<Plane>();
        

        public PTK_BTL_Cut()
          : base("Cut", "Cut",
              "Define the Cut process",
              CommonProps.category, CommonProps.subcate11)
        {
            Message = CommonProps.initialMessage;
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddParameter(new Param_Element1D(), "Element", "E", "Element", GH_ParamAccess.item);
            pManager.AddPlaneParameter("Cut Plane", "P", "Cut Plane", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Flip Plane?", "F", "True flip plane", GH_ParamAccess.item, false);

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
            Plane Plane = new Plane();
            bool Flip = false;

            // --- input --- 
            if (!DA.GetData(0, ref gElement)) { return; }
            Element1D element = gElement.Value;
            if (!DA.GetData(1, ref Plane)) { return; }

            DA.GetData(2, ref Flip);

            if (Flip)
            {
                Plane = new Plane(Plane.Origin, -Plane.ZAxis);
            }

            Curve lineCurve = element.BaseCurve;

            var intersection = Rhino.Geometry.Intersect.Intersection.CurvePlane(lineCurve, Plane, CommonProps.tolerances);
            



            

            Plane templane = Plane;

            if (intersection==null)
            {
                throw new Exception("Your cut does not intersect!");
            }
            else
            {
                templane.Origin = intersection[0].PointA;
            }
            publicPlane.Add(templane);






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

        public override void ExpireSolution(bool recompute)
        {

            base.ExpireSolution(recompute);
        }

        public override void DrawViewportMeshes(IGH_PreviewArgs args)
        {



            if (publicPlane.Count>1)
            {
                foreach (Plane pp in publicPlane)
                {
                    args.Display.DepthMode = Rhino.Display.DepthMode.AlwaysInFront;

                    PlaneArrow Arrow = new PlaneArrow(pp, 50);
                    args.Display.DrawLineArrow(Arrow.NegLine, System.Drawing.Color.Red, 5, 10);
                    args.Display.DrawLineArrow(Arrow.PosLine, System.Drawing.Color.Green, 5, 10);
                    args.Display.DrawBrepShaded(Arrow.SurfacePlane.ToBrep(), new Rhino.Display.DisplayMaterial(System.Drawing.Color.Red));
                }
                
            }
            

        }



        public override Guid ComponentGuid
        {
            get { return new Guid("9ef81ec8-a7e3-4ced-8ed2-fb05a2fd5ef7"); }
        }
    }
}
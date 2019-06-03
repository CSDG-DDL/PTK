using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace PTK
{
    public class PTK_BTL_Cut : GH_Component
    {
        List<Plane> publicPlane = new List<Plane>();
        List<Plane> missingPlane = new List<Plane>();
        bool intersected = false;
        bool run = false;


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

            
            
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("BTL-Cut", "B", "", GH_ParamAccess.list);
            pManager[0].Optional = true;
            pManager[0].DataMapping = GH_DataMapping.Flatten;
            

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

            Curve BaseCurve = element.BaseCurve;

            int miss = 0;

            for (int i=0; i < element.EdgeCurves.Count; i++)
            {
                Curve edgeCurve = element.EdgeCurves[i];
                var edgeIntersection = Rhino.Geometry.Intersect.Intersection.CurvePlane(edgeCurve, Plane, CommonProps.tolerances);

                if (edgeIntersection == null)
                {
                    miss++;
                    //intersected = false;
                    //}
                    //else
                    //{
                    //intersected = true;
                    //}

                    //if (edgeIntersection != null)
                    //{
                    //    intersected = true;
                }

            }

            if (miss == 4)
            {
                intersected = false;
            }
            else
            {
                intersected = true;
            }
            Plane templane = Plane;
            
            //Resets after use of counter
            miss = 0;

                if (intersected == false)
            {
                double param = 0;
                Line templine = new Line(BaseCurve.PointAtStart, BaseCurve.PointAtEnd);
                Rhino.Geometry.Intersect.Intersection.LinePlane(templine, Plane, out param);
                templane.Origin = templine.PointAt(param);

                missingPlane.Add(templane);
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Your cut does not intersect at some elements! The missing planes are marked Grey");

            }

            else
            {
                double lenght = (BaseCurve.PointAtStart - BaseCurve.PointAtEnd).Length;
                var extendedCurve = BaseCurve.Extend(CurveEnd.Both, lenght, CurveExtensionStyle.Line);

                var intersection = Rhino.Geometry.Intersect.Intersection.CurvePlane(extendedCurve, Plane, CommonProps.tolerances);
                templane.Origin = intersection[0].PointA;
                publicPlane.Add(templane);
            }

            // reset after use of bool
            

            OrderedTimberProcess Order = null;

            if (!intersected)
            {
                

                List<OrderedTimberProcess> TempList = new List<OrderedTimberProcess>();
                DA.SetDataList(0, TempList);
            }
            else
            {             
                // --- solve ---
                BTLCut cut = new BTLCut(Plane);

                // Making Object with delegate and ID
                Order = new OrderedTimberProcess(element, new PerformTimberProcessDelegate(cut.DelegateProcess));
                List<OrderedTimberProcess> TempList = new List<OrderedTimberProcess>();
                TempList.Add(Order);
                DA.SetDataList(0, TempList);

            }

            // --- output ---

            
            

            intersected = false;
            run = true; 
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
            publicPlane.Clear();
            missingPlane.Clear();
            base.ExpireSolution(recompute);

        }

        public override void DrawViewportMeshes(IGH_PreviewArgs args)
        {


            if (publicPlane.Count >=1)
            {
                foreach (Plane pp in publicPlane)
                {
                    args.Display.DepthMode = Rhino.Display.DepthMode.AlwaysInFront;

                    PlaneArrow Arrow = new PlaneArrow(pp, 50);
                    args.Display.DrawLineArrow(Arrow.NegLine, System.Drawing.Color.Red, 5, 10);
                    args.Display.DrawLineArrow(Arrow.PosLine, System.Drawing.Color.Green,5, 10);
                    args.Display.DrawBrepShaded(Arrow.SurfacePlane.ToBrep(), new Rhino.Display.DisplayMaterial(System.Drawing.Color.Red));
                }
                
            }
            else if (run)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Your cut actually does not intersect at any elements!");
            }



            if (missingPlane.Count >=1)
            {
                foreach (Plane pp in missingPlane)
                {
                    args.Display.DepthMode = Rhino.Display.DepthMode.AlwaysInFront;

                    PlaneArrow Arrow2 = new PlaneArrow(pp, 50);
                    args.Display.DrawLineArrow(Arrow2.NegLine, System.Drawing.Color.Red, 5, 10);
                    args.Display.DrawLineArrow(Arrow2.PosLine, System.Drawing.Color.Green, 5, 10);
                    args.Display.DrawBrepShaded(Arrow2.SurfacePlane.ToBrep(), new Rhino.Display.DisplayMaterial(System.Drawing.Color.SlateGray));
                }

            }


        }



        public override Guid ComponentGuid
        {
            get { return new Guid("9ef81ec8-a7e3-4ced-8ed2-fb05a2fd5ef7"); }
        }
    }
}
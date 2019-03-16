using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace PTK.Components
{
    public class PTK_PreviewElement : GH_Component
    {

        List<Brep> PrevElements = new List<Brep>();


        public PTK_PreviewElement()
          : base("Preview Element", "PrevElem",
              "Preview Element",
              CommonProps.category, CommonProps.subcate2)
        {
            Message = CommonProps.initialMessage;
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddParameter(new Param_Element1D(), "Elements", "E", "Add elements here", GH_ParamAccess.item);
            pManager[0].Optional = true;
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Model", "M", "3d model", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // --- variables ---
            GH_Element1D gElement = null;
            Dictionary<Brep, Color> tmpModels = new Dictionary<Brep, Color>();

            // --- input --- 
            if (!DA.GetData(0, ref gElement)) { return; }
            Element1D element = gElement.Value;

            // --- solve ---
            Dictionary<Curve, Color> sectionCurves = new Dictionary<Curve, Color>();


            //if(element.CrossSection is Composite comp)
            //{
            //    Vector3d localY = element.CroSecLocalPlane.XAxis;
            //    Vector3d localZ = element.CroSecLocalPlane.YAxis;
            //    Point3d originSection = element.CroSecLocalPlane.Origin;
            //    List<Tuple<CrossSection, Alignment>> secs = comp.RecursionCrossSectionSearch();
            //    foreach(var s in secs)
            //    {
            //        Point3d originSubSection = originSection + s.Item2.OffsetY * localY + s.Item2.OffsetZ * localZ;

            //        Plane localPlaneSubSection = new Plane(originSubSection, localY, localZ);

            //        sectionCurves[new Rectangle3d(
            //                    localPlaneSubSection,
            //                    new Interval(-s.Item1.GetWidth()/2, s.Item1.GetWidth()/2),
            //                    new Interval(-s.Item1.GetHeight()/2, s.Item1.GetHeight()/2)).ToNurbsCurve()]
            //                    =Color.GhostWhite;
            //    }
            //}
            //else
            //{
            //    Plane templane = new Plane(element.CroSecLocalPlane);
            //    templane.Rotate(element.CrossSection.Alignment.RotationAngle, templane.ZAxis, templane.Origin);


            //    Vector3d localY = templane.XAxis;
            //    Vector3d localZ = templane.YAxis;
            //    Point3d originSection = templane.Origin;

            //    Point3d originSubSection = originSection + element.CrossSection.Alignment.OffsetY * localY + element.CrossSection.Alignment.OffsetZ * localZ;

            //    Plane localPlaneSubSection = new Plane(originSubSection, localY, localZ);

            //    sectionCurves[new Rectangle3d(
            //                    localPlaneSubSection,
            //                    new Interval(-element.CrossSection.GetWidth() / 2, element.CrossSection.GetWidth() / 2),
            //                    new Interval(-element.CrossSection.GetHeight() / 2, element.CrossSection.GetHeight() / 2)).ToNurbsCurve()]
            //                    = Color.GhostWhite;
            //}




            //foreach (Curve s in sectionCurves.Keys)
            //{
            //    Curve c = element.BaseCurve;
            //    if (c.IsLinear())
            //    {
            //        Line l = new Line(c.PointAtStart, c.PointAtEnd);
            //        Brep brep = Extrusion.CreateExtrusion(s, l.Direction).ToBrep();
            //        //brep = brep.CapPlanarHoles(CommonProps.tolerances);
            //        tmpModels[brep] = sectionCurves[s];
            //    }
            //    else
            //    {
            //        Brep[] breps = Brep.CreateFromSweep(c, s, true, CommonProps.tolerances);
            //        foreach(var brep in breps)
            //        {
            //            tmpModels[brep] = sectionCurves[s];
            //        }
            //    }
            //}


            List<Brep> SubElements = new List<Brep>();

            foreach (SubElement S in element.Composite.Subelements)
            {
                SubElements.Add(S.GenerateElementGeometry());
            }






            PrevElements = SubElements;


            
            

            DA.SetDataList(0, SubElements);
        }


        





        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return Properties.Resources.PreElement;
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("7da0c2a7-ccb0-4f9e-b383-43b74bf56375"); }
        }

        public override void ExpireSolution(bool recompute)
        {
            PrevElements.Clear();
            base.ExpireSolution(recompute);
        }

        //public override BoundingBox ClippingBox => models.Keys.ToList()[0].GetBoundingBox(false);
        public override void DrawViewportMeshes(IGH_PreviewArgs args)
        {
            foreach(var m in PrevElements)
            {
                //args.Display.DrawObject(m.Key, new Rhino.Display.DisplayMaterial(m.Value, 0.5));
                args.Display.DrawBrepShaded(m, new Rhino.Display.DisplayMaterial(Color.SaddleBrown));
            }
            //base.DrawViewportMeshes(args);
        }
        public override void DrawViewportWires(IGH_PreviewArgs args)
        {
            //base.DrawViewportWires(args);
        }
    }
}
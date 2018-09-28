using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace PTK.Components
{
    public class PTK_6_PreviewGeometry : GH_Component
    {
        public PTK_6_PreviewGeometry()
          : base("Preview Geometry", "PrevGeom",
              "Preview Assembly",
              CommonProps.category, CommonProps.subcate6)
        {
            Message = CommonProps.initialMessage;
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddParameter(new Param_Assembly(), "Assembly", "A", "connect an Assembly here", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddBrepParameter("Model", "M", "3d model", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // --- variables ---
            GH_Assembly gAssembly = null;
            Assembly assembly = null;
            // StructuralAssembly structuralAssembly = null;
            List<Node> nodes = new List<Node>();
            List<Element> elems = new List<Element>();
            List<CrossSection> secs = new List<CrossSection>();
            List<Brep> brepGeom = new List<Brep>();

            // --- input --- 
            if (!DA.GetData(0, ref gAssembly))
            {
                return;
            }
            else
            {
                assembly = gAssembly.Value;
            }

            // --- solve ---
            List<Curve> sectionCurves = new List<Curve>();
            /*
            List<CrossSection> crossSections = new List<CrossSection>();
            foreach (Sub2DElement subElement in element.Sub2DElements)
            {
                Vector3d localY = element.CroSecLocalPlane.XAxis;
                Vector3d localZ = element.CroSecLocalPlane.YAxis;

                Point3d originElement = element.CroSecLocalPlane.Origin;
                Point3d originSubElement = originElement + subElement.Alignment.OffsetY * localY + subElement.Alignment.OffsetZ * localZ;

                Plane localPlaneSubElement = new Plane(originSubElement,
                    element.CroSecLocalPlane.XAxis,
                    element.CroSecLocalPlane.YAxis);

                sectionCurves.Add(new Rectangle3d(
                            localPlaneSubElement,
                            new Interval(-subElement.CrossSection.GetWidth() / 2, subElement.CrossSection.GetWidth() / 2),
                            new Interval(-subElement.CrossSection.GetHeight() / 2, subElement.CrossSection.GetHeight() / 2)).ToNurbsCurve());
            }

            foreach (Curve s in sectionCurves)
            {
                Brep[] breps = Brep.CreateFromSweep(element.BaseCurve, s, true, Rhino.RhinoDoc.ActiveDoc.ModelAbsoluteTolerance);
                models.AddRange(breps);
            }
            */

            // --- output ---

        }

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return Properties.Resources.Assemble;
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("bd28cf4d-1b9a-41cc-abca-e29bb12f09e9"); }
        }
    }
}
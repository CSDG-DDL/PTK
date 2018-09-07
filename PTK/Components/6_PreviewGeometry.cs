using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace PTK.Components
{
    public class PTK_6_PreviewGeometry : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the _6_PreviewGeometry class.
        /// </summary>
        public PTK_6_PreviewGeometry()
          : base("Preview Geometry", "PrevGeom",
              "Preview Assembly",
              CommonProps.category, CommonProps.subcate6)
        {
            Message = CommonProps.initialMessage;
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddParameter(new Param_Assembly(), "Assembly", "A", "connect an Assembly here", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddBrepParameter("Model", "M", "3d model", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            /////////////////////////////////////////////////////////////////////////////////
            // variables
            /////////////////////////////////////////////////////////////////////////////////
            GH_Assembly gAssembly = null;
            Assembly assembly = null;
            // StructuralAssembly structuralAssembly = null;
            List<Node> nodes = new List<Node>();
            List<Element> elems = new List<Element>();
            List<CrossSection> secs = new List<CrossSection>();
            List<Brep> brepGeom = new List<Brep>();

            /////////////////////////////////////////////////////////////////////////////////
            // input
            /////////////////////////////////////////////////////////////////////////////////
            if (!DA.GetData(0, ref gAssembly))
            {
                return;
            }
            else
            {
                assembly = gAssembly.Value;
            }

            /////////////////////////////////////////////////////////////////////////////////
            // solve
            /////////////////////////////////////////////////////////////////////////////////


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

            /////////////////////////////////////////////////////////////////////////////////
            // output
            /////////////////////////////////////////////////////////////////////////////////
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
                return Properties.Resources.Assemble;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("bd28cf4d-1b9a-41cc-abca-e29bb12f09e9"); }
        }
    }
}
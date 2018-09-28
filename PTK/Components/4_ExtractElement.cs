using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace PTK.Components
{
    public class ExtractElement : GH_Component
    {
        public ExtractElement()
          : base("ExtractElement", "Nickname",
              "Description",
              CommonProps.category, CommonProps.subcate4)
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("E", "Element", "", GH_ParamAccess.list);

        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddIntegerParameter("ID", "", "", GH_ParamAccess.list);
            pManager.AddNumberParameter("Width", "", "", GH_ParamAccess.list);
            pManager.AddNumberParameter("Height", "", "", GH_ParamAccess.list);
            pManager.AddNumberParameter("Length", "", "", GH_ParamAccess.list);
            pManager.AddPlaneParameter("XY-plane", "", "", GH_ParamAccess.list);
            pManager.AddPlaneParameter("XZ-plane", "", "", GH_ParamAccess.list);
            pManager.AddPlaneParameter("YZ-plane", "", "", GH_ParamAccess.list);
            pManager.AddBrepParameter("BrepGeometry", "", "", GH_ParamAccess.list);
            pManager.AddCurveParameter("Curve", "", "", GH_ParamAccess.list);
            pManager.AddVectorParameter("Unified Vecsssstor", "", "", GH_ParamAccess.list);

        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // --- variables ---
            List<ElementInDetail> ElementWrapper = new List<ElementInDetail>();
            List<Element1D> Elements = new List<Element1D>();



            DA.GetDataList(0, ElementWrapper);

            List<int> id = new List<int>();
            List<double> width = new List<double>();
            List<double> height = new List<double>();
            List<double> length = new List<double>();
            List<Plane> xyPlane = new List<Plane>();
            List<Plane> xzPlane = new List<Plane>();
            List<Plane> yzPlane = new List<Plane>();
            List<Brep> brep = new List<Brep>();
            List<Curve> curves = new List<Curve>();
            List<Vector3d> unifiedVectors = new List<Vector3d>();






            foreach (ElementInDetail Wrap in ElementWrapper)
            {


                Element1D elem = Wrap.Element;

                /*
                width.Add(elem.SubElement.CrossSections[0].Name
                height.Add(elem.Section.Height);
                length.Add(elem.Length);
                xyPlane.Add(elem.XYPlane);
                xzPlane.Add(elem.XZPlane);
                yzPlane.Add(elem.YZPlane);
                brep.Add(elem.ElementGeometry);
                */
                curves.Add(elem.BaseCurve);


                unifiedVectors.Add(Wrap.UnifiedVector);

            }

            DA.SetDataList(0, id);
            DA.SetDataList(1, width);
            DA.SetDataList(2, height);
            DA.SetDataList(3, length);
            DA.SetDataList(4, xyPlane);
            DA.SetDataList(5, xzPlane);
            DA.SetDataList(6, yzPlane);
            DA.SetDataList(7, brep);
            DA.SetDataList(8, curves);
            DA.SetDataList(9, unifiedVectors);


        }

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return Properties.Resources.Element;
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("bb290a34-d8f7-4ed4-a0e4-84d76f6fe2a3"); }
        }
    }
}
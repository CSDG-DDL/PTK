using Grasshopper;
using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace PTK
{
    public class PTK_DisassembleElement : GH_Component
    {
        public PTK_DisassembleElement()
          : base("Disassemble Element", "X Element",
              "Disassemble Element (PTK)",
              CommonProps.category, CommonProps.subcate8)
        {
            Message = CommonProps.initialMessage;
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddParameter(new Param_Element1D(), "Element", "E", "PTK ELEM", GH_ParamAccess.item);
            pManager[0].Optional = true;
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Tag", "Tag", "Tag", GH_ParamAccess.item);
            pManager.AddCurveParameter("Curve", "Curve", "Curve", GH_ParamAccess.item);
            pManager.AddPointParameter("Point At Start", "Ps", "Point At Start", GH_ParamAccess.item);
            pManager.AddPointParameter("Point At End", "Pe", "Point At End", GH_ParamAccess.item);
            pManager.AddPlaneParameter("YZ Plane", "Pl", "returns local yz plane", GH_ParamAccess.item);
            pManager.RegisterParam(new Param_CroSec(), "CrossSections", "S", "CrossSections", GH_ParamAccess.list);
            pManager.RegisterParam(new Param_Alignment(), "Alignment", "A", "Alignment", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Intersect Other", "I", "Is Intersect With Other", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // --- variables ---
            GH_Element1D gElem = null;

            // --- input --- 
            if (!DA.GetData(0, ref gElem)) { return; }
            Element1D elem = gElem.Value;

            // --- solve ---
            string tag = elem.Tag;
            Curve curve = elem.BaseCurve;
            Point3d ps = elem.PointAtStart;
            Point3d pe = elem.PointAtEnd;
            Plane plane = elem.CroSecLocalPlane;

            // elem.Composite.SubElements.
            List<GH_CroSec> secs = elem.CrossSections.ConvertAll(s => new GH_CroSec(s));
            GH_Alignment align = new GH_Alignment(elem.Align);
            bool intersect = elem.IsIntersectWithOther;

            // --- output ---
            DA.SetData(0, tag);
            DA.SetData(1, curve);
            DA.SetData(2, ps);
            DA.SetData(3, pe);
            DA.SetData(4, plane);
            DA.SetDataList(5, secs);
            DA.SetData(6, align);
            DA.SetData(7, intersect);
        }


        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return PTK.Properties.Resources.Element;
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("891a0366-cf2f-4642-b92b-4a93d0389330"); }
        }

    }

}
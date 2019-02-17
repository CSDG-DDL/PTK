using System;
using System.Collections.Generic;

using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;

namespace PTK
{
    public class PTK_ExtractElement : GH_Component
    {
        public PTK_ExtractElement()
          : base("Extract Element", "ExtractElement",
              "Extract Element",
              CommonProps.category, CommonProps.subcate8)
        {
            Message = CommonProps.initialMessage;
        }

        
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddParameter(new Param_Element1D(), "Element", "E", "Element", GH_ParamAccess.item);
            pManager[0].Optional = true;
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Tag", "T", "Tag", GH_ParamAccess.item);
            pManager.AddCurveParameter("Bace Curve", "C", "Bace Curve", GH_ParamAccess.item);
            pManager.AddPointParameter("Point At Start", "Ps", "Point At Start", GH_ParamAccess.item);
            pManager.AddPointParameter("Point At End", "Pe", "Point At End", GH_ParamAccess.item);
            pManager.AddPlaneParameter("CroSecLocalPlane", "Pl", "returns CroSec Local Plane", GH_ParamAccess.item);
            pManager.RegisterParam(new Param_CroSec(), "CrossSections", "S", "CrossSections", GH_ParamAccess.item);
            pManager.RegisterParam(new Param_Alignment(), "Alignment", "A", "Alignment", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Intersect Other", "I", "Is Intersect With Other", GH_ParamAccess.item);
            pManager.RegisterParam(new Param_Force(), "Forces", "F", "forces in the element", GH_ParamAccess.item);
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
            

            GH_CroSec sec = new GH_CroSec(elem.CrossSection);
            GH_Alignment align = new GH_Alignment(elem.Alignment);
            bool intersect = elem.IsIntersectWithOther;
            GH_Force forc = new GH_Force(elem.Forces);
            

            // --- output ---
            DA.SetData(0, tag);
            DA.SetData(1, curve);
            DA.SetData(2, ps);
            DA.SetData(3, pe);
            DA.SetData(4, plane);
            DA.SetData(5, sec);
            DA.SetData(6, align);
            DA.SetData(7, intersect);
            DA.SetData(8, forc );
        }


        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return PTK.Properties.Resources.ExtElement;
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("891a0366-cf2f-4642-b92b-4a93d0389330"); }
        }

    }

}
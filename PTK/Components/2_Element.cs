﻿    using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PTK
{
    public class PTK_Element : GH_Component
    {

        public PTK_Element()
          : base("Element", "Element",
              "creates a beam element.",
              CommonProps.category, CommonProps.subcate2)
        {
            Message = CommonProps.initialMessage;
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Tag", "T", "Add a tag to the element here.", GH_ParamAccess.item, "Not Named Element");
            pManager.AddCurveParameter("Base Curve", "C", "Add curves that shall be materalized", GH_ParamAccess.item);
            pManager.AddParameter(new Param_CroSec(), "CrossSection", "CS", "CrossSection", GH_ParamAccess.item);
            pManager.AddParameter(new Param_Alignment(), "Alignment", "A", "Global Alignment", GH_ParamAccess.item);
            pManager.AddParameter(new Param_Force(), "Forces", "F", "Add forces", GH_ParamAccess.item);
            pManager.AddParameter(new Param_Joint(), "Joint", "J", "Add joint", GH_ParamAccess.list);
            pManager.AddIntegerParameter("Structural Priority", "P", "Add integer value to set the priority of the member", GH_ParamAccess.item, 0);
            pManager.AddBooleanParameter("Intersection Nodes?", "I?", "Whether the element intersects other members at other than the end point", GH_ParamAccess.item, true);

            pManager[0].Optional = true;
            pManager[1].Optional = true;
            pManager[2].Optional = true;
            pManager[3].Optional = true;
            pManager[4].Optional = true;
            pManager[5].Optional = true;
            pManager[6].Optional = true;
            pManager[7].Optional = true;
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.RegisterParam(new Param_Element1D(), "Element", "E", "PTK Elements", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            //test

            // --- variables ---
            string tag = null;
            Curve curve = null;
            GH_CroSec gCroSec = null;
            CrossSection crossSection = null;
            GH_Alignment gAlignment = null;
            Alignment alignment = null;
            GH_Force gForces = null;
            Force forces = null;
            List<GH_Joint> gJoints = new List<GH_Joint>();
            List<Joint> joints = null;
            int priority = new int();
            bool intersect = true;

            // --- input --- 
            if (!DA.GetData(0, ref tag)) { return; }
            if (!DA.GetData(1, ref curve)) { return; }
            if (!DA.GetData(2, ref gCroSec))
            {
                crossSection = new RectangleCroSec("DefaultCrossSection");
                
            }
            else
            {
                crossSection = gCroSec.Value;
            }
            
            if (!DA.GetData(3, ref gAlignment))
            {
                GlobalAlignmentRules.AlignmentFromVector VectorAlign = new GlobalAlignmentRules.AlignmentFromVector(new Vector3d(0,0,1));

                alignment = new Alignment("", 0, 0, VectorAlign.GenerateVector);

            }
            else
            {
                alignment = gAlignment.Value;
            }
            //Generating Alignment
            alignment.GenerateVectorFromDelegate(curve);

            if (!DA.GetData(4, ref gForces))
            {
                forces = new Force();
            }
            else
            {
                forces = gForces.Value;
            }
            if (!DA.GetDataList(5, gJoints))
            {
                joints = new List<Joint>();
            }
            else
            {
                joints = gJoints.ConvertAll(j => j.Value);
            }
            if (!DA.GetData(6, ref priority)) { return; }
            if (!DA.GetData(7, ref intersect)) { return; }








            // --- solve ---
            GH_Element1D elem = new GH_Element1D(new Element1D(tag, curve, crossSection, alignment, forces, joints, priority, intersect));

            // --- output ---
            DA.SetData(0, elem);
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
            get { return new Guid("0f259d4d-3cf2-4337-9545-c392178e1fe1"); }
        }
    }
}

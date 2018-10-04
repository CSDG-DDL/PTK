using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace PTK
{
    public class PTK_Force : GH_Component
    {
        public PTK_Force()
          : base("Force", "Force",
              "Adding forces here if data allready is provided ",
              CommonProps.category, CommonProps.subcate2)
        {
            Message = CommonProps.initialMessage;
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddIntegerParameter("Load Case", "LC", "Load Case", GH_ParamAccess.item, 0);
            pManager.AddNumberParameter("FX", "FX", "Add FX", GH_ParamAccess.item, 0.0);
            pManager.AddNumberParameter("FY", "FY", "Add FY", GH_ParamAccess.item, 0.0);
            pManager.AddNumberParameter("FZ", "FZ", "Add FZ", GH_ParamAccess.item, 0.0);
            pManager.AddNumberParameter("MX", "MX", "Add MX", GH_ParamAccess.item, 0.0);
            pManager.AddNumberParameter("MY", "MY", "Add MY", GH_ParamAccess.item, 0.0);
            pManager.AddNumberParameter("MZ", "MZ", "Add MZ", GH_ParamAccess.item, 0.0);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.RegisterParam(new Param_Force(), "Force", "F", "Forces to be added to Materializer", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // --- variables ---
            int loadCase = new int();
            double fx = new double();
            double fy = new double();
            double fz = new double();
            double mx = new double();
            double my = new double();
            double mz = new double();

            // --- input --- 
            if (!DA.GetData(0, ref loadCase)) { return; }
            if (!DA.GetData(1, ref fx)) { return; }
            if (!DA.GetData(2, ref fy)) { return; }
            if (!DA.GetData(3, ref fz)) { return; }
            if (!DA.GetData(4, ref mx)) { return; }
            if (!DA.GetData(5, ref my)) { return; }
            if (!DA.GetData(6, ref mz)) { return; }

            // --- solve ---
            GH_Force force = new GH_Force(new Force(loadCase, fx, fy, fz, mx, my, mz));

            // --- output ---
            DA.SetData(0, force);
        }

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return PTK.Properties.Resources.Force;
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("76a606c9-f75b-4c7f-a30e-02baf83adb53"); }
        }
    }
}
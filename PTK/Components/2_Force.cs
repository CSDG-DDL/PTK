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
            pManager.AddNumberParameter("FXc", "FXc", "Add FX compression", GH_ParamAccess.list, new List<double>() { 0 });
            pManager.AddNumberParameter("FXt", "FXt", "Add FX tension", GH_ParamAccess.list, new List<double>() { 0  });
            pManager.AddNumberParameter("FY", "FY", "Add FY", GH_ParamAccess.list, new List<double>() { 0 });
            pManager.AddNumberParameter("FZ", "FZ", "Add FZ", GH_ParamAccess.list, new List<double>() { 0 });
            pManager.AddNumberParameter("MX", "MX", "Add MX", GH_ParamAccess.list, new List<double>() { 0 });
            pManager.AddNumberParameter("MY", "MY", "Add MY", GH_ParamAccess.list, new List<double>() { 0 });
            pManager.AddNumberParameter("MZ", "MZ", "Add MZ", GH_ParamAccess.list, new List<double>() { 0 });
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.RegisterParam(new Param_Force(), "Force", "F", "Forces to be added to Materializer", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // --- variables ---
            int loadCase = new int();
            var fxc = new List<double>();
            var fxt = new List<double>();
            var fy = new List<double>();
            var fz = new List<double>();
            var mx = new List<double>();
            var my = new List<double>();
            var mz = new List<double>();

            // --- input --- 
            if (!DA.GetData(0, ref loadCase)) { return; }
            if (!DA.GetDataList(1,  fxc)) { return; }
            if (!DA.GetDataList(2,  fxt)) { return; }
            if (!DA.GetDataList(3,  fy)) { return; }
            if (!DA.GetDataList(4,  fz)) { return; }
            if (!DA.GetDataList(5,  mx)) { return; }
            if (!DA.GetDataList(6,  my)) { return; }
            if (!DA.GetDataList(7,  mz)) { return; }

            // --- solve ---
            GH_Force force = new GH_Force(new Force( fxc, fxt , fy, fz, mx, my, mz));

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
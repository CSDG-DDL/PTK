using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace PTK
{
    public class PTK_2_Force : GH_Component
    {
        public PTK_2_Force()
          : base("Force", "Force",
              "Adding forces here if data allready is provided ",
              CommonProps.category, CommonProps.subcate2)
        {
            Message = CommonProps.initialMessage;
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("FX", "FX", "Add FX", GH_ParamAccess.tree, 0);   //Should be tree, cause more than force pr element
            pManager.AddNumberParameter("FY", "FY", "Add FY", GH_ParamAccess.tree, 0);
            pManager.AddNumberParameter("FZ", "FZ", "Add FZ", GH_ParamAccess.tree, 0);
            pManager.AddNumberParameter("MX", "MX", "Add MX", GH_ParamAccess.tree, 0);
            pManager.AddNumberParameter("MY", "MY", "Add MY", GH_ParamAccess.tree, 0);
            pManager.AddNumberParameter("MZ", "MZ", "Add MZ", GH_ParamAccess.tree, 0);

        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.RegisterParam(new Param_Force(), "Force", "F", "Forces to be added to Materializer", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
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
using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace PTK
{
    public class PTK_4_DetailModel : GH_Component
    {

        public PTK_4_DetailModel()
          : base("Detail Model", "Detail Model",
              "Detail Model",
              CommonProps.category, CommonProps.subcate4)
        {
            Message = CommonProps.initialMessage;
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("PTK Assembly", "A (PTK)", "PTK DATA INPUT", GH_ParamAccess.item);
            pManager.AddGenericParameter("SEL NODE", "SEL NODE", "PTK LOGIC OF MAKING DETAILS", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("PTK LOGIC", "LOGIC (PTK)", "LOGIC (PTK)", GH_ParamAccess.item);

        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // --- variables ---

            // --- input --- 

            // --- solve ---

            // --- output ---
        }

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return PTK.Properties.Resources.Detail;
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("d8643dc4-2a9f-4573-920f-4e808275b29b"); }
        }
    }
}
using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace PTK
{
    public class PTK_5_FeasibleCheck : GH_Component
    {
        public PTK_5_FeasibleCheck()
          : base("FEASIBILITY CHECK", "IsFeasible",
              "FEASIBILITY CHECK",
              CommonProps.category, CommonProps.subcate5)
        {
            Message = CommonProps.initialMessage;
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("PTK Assembly", "A (PTK)", "PTK DATA INPUT", GH_ParamAccess.item);

        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddBooleanParameter("FEASIBLE?", "FEASIBLE?", "FEASIBILITY CHECK RESULT", GH_ParamAccess.item);
            
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
                return PTK.Properties.Resources.LocalAnalysis;

            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("9f6403f8-874c-41f8-87ef-6b908c862f2a"); }
        }
    }
}
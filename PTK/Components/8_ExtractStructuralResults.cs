using System;
using System.Collections.Generic;

using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;


namespace PTK
{
    public class PTK_ExtractStructuralResults : GH_Component
    {
        public PTK_ExtractStructuralResults()
          : base("Deconstruct Force", "DF",
              "Deconstructs a force into into values",
              CommonProps.category, CommonProps.subcate8)
        {
            Message = CommonProps.initialMessage;
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddParameter(new Param_StructuralData(), "StructuralData", "SD", "StructuralData", GH_ParamAccess.item);
            pManager[0].Optional = true;
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new Param_Force(), "StructuralData", "SD", "StructuralData", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // --- variables ---
            GH_StructuralData gSD = null;

            // --- input --- 
            if (!DA.GetData(0, ref gSD)) { return; }
            StructuralData sd = gSD.Value;

            // --- solve ---

            
            // --- output ---
            DA.SetData(0, sd.StructuralForces);
            DA.SetData(1, sd.StructuralResults);
           

        }

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return PTK.Properties.Resources.ExtNode;
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("dd123cf2-521c-44a4-1221-f0335469c0dd"); }
        }
    }
}
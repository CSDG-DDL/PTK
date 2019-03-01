using System;
using System.Collections.Generic;

using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;


namespace PTK
{
    public class PTK_ExtractForce : GH_Component
    {
        public PTK_ExtractForce()
          : base("Extract Force", "Extract Force",
              "Extract Force",
              CommonProps.category, CommonProps.subcate8)
        {
            Message = CommonProps.initialMessage;
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddParameter(new Param_Force(), "Force", "F", "Force", GH_ParamAccess.item);
            pManager[0].Optional = true;
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddNumberParameter("Force List", "FL", "List of forces in the element", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // --- variables ---
            GH_Force gForce = null;

            // --- input --- 
            if (!DA.GetData(0, ref gForce)) { return; }
            Force force = gForce.Value;

            // --- solve ---
            List<double> forceList = new List<double>();
            forceList.Add(force.Max_Fx_compression);
            forceList.Add(force.Max_Fx_tension);
            forceList.Add(force.Max_Fy_shear);
            forceList.Add(force.Max_Fz_shear);

            forceList.Add(force.Max_Mx_torsion);
            forceList.Add(force.Max_My_bending);
            forceList.Add(force.Max_Mz_bending);
            
            // --- output ---
            DA.SetDataList(0, forceList);
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
            get { return new Guid("dd123cf2-521c-44a4-8448-f0335469c0dd"); }
        }
    }
}
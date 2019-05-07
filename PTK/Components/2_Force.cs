using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace PTK
{
    /*
    public class PTK_Force : GH_Component
    {
        
        public PTK_Force( )
          : base("Force", "Force",
              "Adding forces here if data allready is provided ",
              CommonProps.category, CommonProps.subcate5)
        {
            Message = CommonProps.initialMessage;
        }
        /// <summary>
        /// Overrides the exposure level in the components category 
        /// </summary>
        public override GH_Exposure Exposure
        {
            get
            { return GH_Exposure.secondary; }
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddIntegerParameter("Load Case", "LC", "Load Case", GH_ParamAccess.item, 0);
            pManager.AddNumberParameter("FXc", "FXc", "Add FX compression", GH_ParamAccess.item, 0);
            pManager.AddNumberParameter("FXt", "FXt", "Add FX tension", GH_ParamAccess.item, 0);
            pManager.AddNumberParameter("FY", "FY", "Add FY", GH_ParamAccess.item , 0);
            pManager.AddNumberParameter("FZ", "FZ", "Add FZ", GH_ParamAccess.item, 0);
            pManager.AddNumberParameter("MX", "MX", "Add MX", GH_ParamAccess.item, 0);
            pManager.AddNumberParameter("MY", "MY", "Add MY", GH_ParamAccess.item, 0);
            pManager.AddNumberParameter("MZ", "MZ", "Add MZ", GH_ParamAccess.item, 0);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // --- variables ---
            int loadCase = 0;
            double fxc = 0;
            double fxt = 0;
            double fy = 0;
            double fz = 0;
            double mx = 0;
            double my = 0;
            double mz = 0;

            // --- input --- 
            if (!DA.GetData(0, ref loadCase)) { return; }
            if (!DA.GetData(1, ref fxc)) { return; }
            if (!DA.GetData(2, ref fxt)) { return; }
            if (!DA.GetData(3, ref fy)) { return; }
            if (!DA.GetData(4, ref fz)) { return; }
            if (!DA.GetData(5, ref mx)) { return; }
            if (!DA.GetData(6, ref my)) { return; }
            if (!DA.GetData(7, ref mz)) { return; }

            // --- solve ---
            StructuralData structuralData = new StructuralData();
            structuralData.maxCompressionForce.FX = fxc;
            structuralData.maxTensionForce.FX = fxt;
            structuralData.maxShearDir1.FY = fy;
            structuralData.maxShearDir2.FZ = fz;
            structuralData.maxBendingDir1.MY = my;
            structuralData.maxBendingDir2.MZ = mz;
            structuralData.maxTorsion.MX = mx;

            // --- output ---
            DA.SetData(0, new GH_StructuralData(structuralData) );
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
    
     */
}
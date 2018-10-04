using feb;
using GH_IO.Serialization;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Karamba;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;

namespace PTK
{
    public class PTK_KarambaExport : GH_Component
    {
        public PTK_KarambaExport()
          : base("Karamba Analysis", "Karamba Analysis",
              "Creates Model information of Karamba",
              CommonProps.category, CommonProps.subcate5)
        {
            Message = CommonProps.initialMessage;
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddParameter(new Param_StructuralAssembly(), "Structural Assembly", "SA", "Structural Assembly", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.RegisterParam(new Karamba.Models.Param_Model(), "Karamba Model", "KM", "Karamba Model", GH_ParamAccess.item);
            pManager.RegisterParam(new Karamba.Models.Param_Model(), "Analyzed Assembly", "AA", "", GH_ParamAccess.item);
            pManager.AddNumberParameter("Displacement", "D", "Maximum displacement in [m]", GH_ParamAccess.list);
            pManager.AddNumberParameter("Gravity force", "G", "Resulting force of gravity [kN] of each load-case of the model", GH_ParamAccess.list);
            pManager.AddNumberParameter("Strain Energy", "E", "Internal elastic energy in [kNm of each load cases of the model", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // --- variables ---
            GH_StructuralAssembly gStrAssembly = null;
            StructuralAssembly structuralAssembly = null;
            List<double> maxDisps;
            List<double> gravityForces;
            List<double> elasticEnergy;
            string warning;

            // --- input --- 
            if (!DA.GetData(0, ref gStrAssembly)) { return; }
            structuralAssembly = gStrAssembly.Value;
            

            // --- solve ---
            var karambaModel = PTK.KarambaConversion.BuildModel(structuralAssembly);
            Karamba.Models.Model analyzedModel;

            //Karamba.Algorithms.Component_ThIAnalyze.solve(
            //    karambaModel,
            //    out maxDisps,
            //    out gravityForces,
            //    out elasticEnergy,
            //    out warning,
            //    out analyzedModel
            //);

            // --- output ---
            DA.SetData(0, new Karamba.Models.GH_Model(karambaModel));
            //DA.SetData(1, new Karamba.Models.GH_Model(analyzedModel));
            //DA.SetDataList(2, maxDisps);
            //DA.SetDataList(3, gravityForces);
            //DA.SetDataList(4, elasticEnergy);
        }

        
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return PTK.Properties.Resources.Karamba;
            }
        }

        
        public override Guid ComponentGuid
        {
            get { return new Guid("7c6860fa-ee7b-4580-9f04-7bab9e325d8b"); }
        }
    }
}

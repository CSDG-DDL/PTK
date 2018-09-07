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
    public class PTK_5_KarambaExport : GH_Component
    {
        public PTK_5_KarambaExport()
          : base("Karamba Analysis", "Karamba Analysis",
              "Creates Model information of Karamba",
              CommonProps.category, CommonProps.subcate5)
        {
            Message = CommonProps.initialMessage;

        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddParameter(new Param_Assembly(), "Structural Assembly", "SA", "Structural Assembly", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.RegisterParam(new Karamba.Models.Param_Model(), "Analyzed Model", "Analyzed Model", "", GH_ParamAccess.item);
            pManager.AddNumberParameter("Displacement", "D", "Maximum displacement in [m]", GH_ParamAccess.list);
            pManager.AddNumberParameter("Gravity force", "G", "Resulting force of gravity [kN] of each load-case of the model", GH_ParamAccess.list);
            pManager.AddNumberParameter("Strain Energy", "E", "Internal elastic energy in [kNm of each load cases of the model", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            #region variables
            GH_Assembly gAssembly = null;
            StructuralAssembly structuralAssembly = null;
            List<double> maxDisps;
            List<double> gravityForces;
            List<double> elasticEnergy;
            string warning;
            #endregion

            #region input
            if (!DA.GetData(0, ref gAssembly)) { return; }
            if (gAssembly.Value is StructuralAssembly)
            {
                structuralAssembly = (StructuralAssembly)gAssembly.Value;
            }
            else
            {
                return;
            }
            #endregion

            #region solve
            var karambaModel = PTK.KarambaConversion.BuildModel(structuralAssembly);

            Karamba.Algorithms.Component_ThIAnalyze.solve(
                karambaModel,
                out maxDisps,
                out gravityForces,
                out elasticEnergy,
                out warning,
                out karambaModel
            );

            //feb.Deform deform = new feb.Deform(karambaModel.febmodel);
            //feb.Response response = new feb.Response(deform);

            //response.updateNodalDisplacements();
            //response.updateMemberForces();
            #endregion

            #region output
            DA.SetData(0, new Karamba.Models.GH_Model(karambaModel));
            DA.SetDataList(1, maxDisps);
            DA.SetDataList(2, gravityForces);
            DA.SetDataList(3, elasticEnergy);
            #endregion
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

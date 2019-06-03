using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Karamba;
using Rhino.Geometry;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;


namespace PTK
{
    public class PTK_KarambaLoad : GH_Component
    {

        public PTK_KarambaLoad()
          : base("KarambaLoad", "KarmbaLoad",
                "Change any Karamba load to reindeer load",
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
            pManager.AddGenericParameter("KarambaLoad", "KL", "Karamba load to be changed to reindeer load", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Load Case", "LC", "Load case", GH_ParamAccess.item, 0);
            
            pManager[0].Optional = true;
            pManager[1].Optional = true;

        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.RegisterParam(new Param_Load(), "Reindeer Load", "L", "Load data to be send to Assembler(PTK)", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // --- variables ---
            Karamba.Loads.GH_Load GH_kLoad = new Karamba.Loads.GH_Load();
            int lcase = new int();

            // --- input --- 
            if (!DA.GetData(0, ref GH_kLoad)) { return; }
            if (!DA.GetData(1, ref lcase)) { return; }

            // --- solve ---
            KarambaLoad reindeerLoad = new KarambaLoad();
            reindeerLoad.karambaLoad = GH_kLoad;
            GH_Load GH_load = new GH_Load(reindeerLoad);

            // --- output ---
            DA.SetData(0, GH_load);
        }

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return Properties.Resources.KarambaLoad;
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("58181589-b12d-4cd4-850f-c0f38030bd1b"); }
        }
    }
}

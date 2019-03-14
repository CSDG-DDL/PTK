using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace PTK
{
    public class PTK_PointLoad : GH_Component
    {
        public PTK_PointLoad()
            : base("PointLoad", "PointLoad",
                "Add load here",
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
            pManager.AddTextParameter("Tag", "T", "Tag", GH_ParamAccess.item, "Not Named PointLoad");
            pManager.AddIntegerParameter("Load Case", "LC", "Load case", GH_ParamAccess.item, 0);
            pManager.AddPointParameter("Point", "P", "Point to which load will be assigned", GH_ParamAccess.item, new Point3d() );
            pManager.AddVectorParameter("Force Vector", "FV", "in [kN]. Vector which describe the diretion and value in kN", GH_ParamAccess.item, new Vector3d(0, 0, -1));
            pManager.AddVectorParameter("Moment Vector", "MV", "in [kNm]. Vector which describe the diretion and value in kNm", GH_ParamAccess.item, new Vector3d());

            pManager[0].Optional = true;
            pManager[1].Optional = true;
            pManager[2].Optional = true;
            pManager[3].Optional = true;
            pManager[4].Optional = true;
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.RegisterParam(new Param_Load(), "Point Load", "L", "Load data to be send to Assembler(PTK)", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // --- variables ---
            string Tag = null;
            int lcase = 0;
            Point3d point = new Point3d();
            Vector3d fvector = new Vector3d();
            Vector3d mvector = new Vector3d();

            // --- input --- 
            if (!DA.GetData(0, ref Tag)) { return; }
            if (!DA.GetData(1, ref lcase)) { return; }
            if (!DA.GetData(2, ref point)) { return; }
            if (!DA.GetData(3, ref fvector)) { return; }
            if (!DA.GetData(4, ref mvector)) { return; }

            // --- solve ---
            GH_Load load = new GH_Load(new PointLoad(Tag, lcase, point, fvector, mvector));

            // --- output ---
            DA.SetData(0, load);
        }

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return PTK.Properties.Resources.PointLoad;
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("965bef7b-feea-46d1-abe9-f686d28b9c41"); }
        }
    }



}

using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace PTK
{
    public class PTK_3_PointLoad : GH_Component
    {
        public PTK_3_PointLoad()
            : base("PointLoad", "PointLoad",
                "Add load here",
                CommonProps.category, CommonProps.subcate3)
        {
            Message = CommonProps.initialMessage;
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Tag", "T", "Tag", GH_ParamAccess.item, "N/A");
            pManager.AddIntegerParameter("Load Case", "LC", "Load case", GH_ParamAccess.item, 0);
            pManager.AddPointParameter("Point", "P", "Point to which load will be assigned", GH_ParamAccess.item );
            pManager.AddVectorParameter("Force Vector","F","in [kN]. Vector which describe the diretion and value in kN", GH_ParamAccess.item, new Vector3d(0, 0, -1));
            pManager.AddVectorParameter("Moment Vector", "M", "in [kN]. Vector which describe the diretion and value in kN", GH_ParamAccess.item, new Vector3d(0, 0, 0));

            pManager[0].Optional = true;
            pManager[1].Optional = true;
            pManager[3].Optional = true;
            pManager[4].Optional = true;
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.RegisterParam(new Param_Load(), "Point Load", "L", "Load data to be send to Assembler(PTK)", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            #region variables
            string Tag = null;
            int lcase = 0;
            Point3d point = new Point3d();
            Vector3d fvector = new Vector3d();
            Vector3d mvector = new Vector3d();
            #endregion

            #region input
            DA.GetData(0, ref Tag);
            if (!DA.GetData(1, ref lcase)) { return; }
            if (!DA.GetData(2, ref point)) { return; }
            if (!DA.GetData(3, ref fvector)) { return; }
            if (!DA.GetData(4, ref mvector)) { return; }
            #endregion

            #region solve
            GH_Load load = new GH_Load(new PointLoad(Tag, lcase, point, fvector, mvector));
            #endregion

            #region output
            DA.SetData(0, load);
            #endregion
        }

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return PTK.Properties.Resources.Load;
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("965bef7b-feea-46d1-abe9-f686d28b9c41"); }
        }
    }



}

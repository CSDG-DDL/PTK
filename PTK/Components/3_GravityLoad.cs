﻿using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace PTK.Components
{
    public class PTK_3_GravityLoad : GH_Component
    {

        public PTK_3_GravityLoad()
          : base("GtravityLoad", "GravityLoad",
                "Add load here",
                CommonProps.category, CommonProps.subcate3)
        {
            Message = CommonProps.initialMessage;
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Tag", "T", "Tag", GH_ParamAccess.item, "N/A");
            pManager.AddIntegerParameter("Load Case", "LC", "Load case", GH_ParamAccess.item, 0);
            pManager.AddVectorParameter("Gravity Vector", "G", "in [kN]. Vector which describe the diretion and value in kN", GH_ParamAccess.item,new Vector3d(0,0,-1));

            pManager[0].Optional = true;
            pManager[1].Optional = true;
            pManager[2].Optional = true;
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.RegisterParam(new Param_Load(), "Gravity Load", "L", "Load data to be send to Assembler(PTK)", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            #region variables
            string Tag = null;
            int lcase = 0;
            Vector3d gvector = new Vector3d();
            #endregion

            #region input
            DA.GetData(0, ref Tag);
            if (!DA.GetData(1, ref lcase)) { return; }
            if (!DA.GetData(2, ref gvector)) { return; }
            #endregion

            #region solve
            GH_Load load = new GH_Load(new GravityLoad(Tag, lcase, gvector));
            #endregion

            #region output
            DA.SetData(0, load);
            #endregion
        }

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return null;
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("58181589-b98d-4cd4-850f-c0f38030bd1b"); }
        }
    }
}
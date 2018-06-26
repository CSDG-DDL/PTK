﻿using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace PTK.Components
{
    public class PTK_3_2_StructuralElement : GH_Component
    {
        public PTK_3_2_StructuralElement()
          : base("Structural Element", "Str Element",
              "creates a beam element.",
              CommonProps.category, CommonProps.subcate2)
        {
            Message = CommonProps.initialMessage;
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddParameter(new Param_Element1D(), "Element", "S", "Add the cross-section componentt here", GH_ParamAccess.item);
            pManager.AddParameter(new Param_Force(), "Forces", "F", "Add the cross-section componentt here", GH_ParamAccess.list);
            pManager[1].Optional = true;
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.RegisterParam(new Param_StructuralElement(), "Structural Element", "SE", "Structural Element", GH_ParamAccess.item);
        }


        protected override void SolveInstance(IGH_DataAccess DA)
        {
            #region variables
            Element1D elem = null;
            List<Force> forces = null;
            #endregion

            #region input
            if (!DA.GetData(0, ref elem)) { return; }
            if (!DA.GetDataList(1, forces))
            {
                forces = new List<Force>();
            }
            #endregion

            #region solve
            GH_StructuralElement strElem = new GH_StructuralElement(new StructuralElement(elem, forces));
            #endregion

            #region output
            DA.SetData(0, strElem);
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
            get { return new Guid("493f1111-4e43-4497-aacc-41cb29a1baf0"); }
        }
    }
}
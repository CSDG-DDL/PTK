using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace PTK.Components
{
    public class PTK_2_StructuralElement : GH_Component
    {
        public PTK_2_StructuralElement()
          : base("Structural Element", "Str Element",
              "creates a beam element.",
              CommonProps.category, CommonProps.subcate2)
        {
            Message = CommonProps.initialMessage;
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddParameter(new Param_Element1D(), "Element", "E", "Add the cross-section componentt here", GH_ParamAccess.item);
            pManager.AddParameter(new Param_Force(), "Forces", "F", "Add the cross-section componentt here", GH_ParamAccess.list);
            pManager.AddParameter(new Param_Force(), "Joints", "J", "Add the cross-section componentt here", GH_ParamAccess.list);
            pManager[1].Optional = true;
            pManager[2].Optional = true;
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.RegisterParam(new Param_StructuralElement(), "Structural Element", "SE", "Structural Element", GH_ParamAccess.item);
        }


        protected override void SolveInstance(IGH_DataAccess DA)
        {
            #region variables
            GH_Element1D gElem = null;
            Element1D elem = null;
            List<GH_Force> gForces = new List<GH_Force>();
            List<Force> forces = null;
            List<GH_Joint> gJoints = new List<GH_Joint>();
            List<Joint> joints = null;
            #endregion

            #region input
            if (!DA.GetData(0, ref gElem))
            {
                elem = new Element1D();
            }
            else
            {
                elem = gElem.Value;
            }
            if (!DA.GetDataList(1, gForces))
            {
                forces = new List<Force>();
            }
            else
            {
                forces = gForces.ConvertAll(f => f.Value);
            }
            if (!DA.GetDataList(2, gJoints))
            {
                joints = new List<Joint>();
            }
            else
            {
                joints = gJoints.ConvertAll(j => j.Value);
            }
            #endregion

            #region solve
            GH_StructuralElement strElem = new GH_StructuralElement(new StructuralElement(elem, forces, joints));
            #endregion

            #region output
            DA.SetData(0, strElem);
            #endregion
        }

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return Properties.Resources.Element;
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("493f1111-4e43-4497-aacc-41cb29a1baf0"); }
        }
    }
}
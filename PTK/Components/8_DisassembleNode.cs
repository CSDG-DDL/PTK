using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

// using System.Numerics;

namespace PTK
{
    public class PTK_8_DisassembleNode : GH_Component
    {
        public PTK_8_DisassembleNode()
          : base("Disassemble Node", "X Node",
              "Disassemble Node (PTK)",
              CommonProps.category, CommonProps.subcate8)
        {
            Message = CommonProps.initialMessage;
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddParameter(new Param_Node(), "Node", "N", "PTK NODE", GH_ParamAccess.item);
            pManager[0].Optional = true;
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddPointParameter("Point", "P", "Node point", GH_ParamAccess.item);
            //pManager.AddVectorParameter("Displacement Vectors", "V", "Displacement", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            #region variables
            GH_Node gNode = null;
            #endregion

            #region input
            if (!DA.GetData(0, ref gNode)) { return; }
            Node node = gNode.Value;
            #endregion

            #region solve
            Point3d p = node.Point;
            //List<Vector3d> vs = node.DisplacementVectors;
            #endregion

            #region output
            DA.SetData(0, p);
            //DA.SetDataList(1, vs);
            #endregion
        }

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return PTK.Properties.Resources.Node;
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("dd8adcf2-521c-44a4-8448-f0335469c0dd"); }
        }
    }
}
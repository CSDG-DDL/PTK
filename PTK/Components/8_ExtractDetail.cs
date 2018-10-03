using System;
using System.Collections.Generic;

using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;


namespace PTK
{
    public class PTK_ExtractDetail : GH_Component
    {
        public PTK_ExtractDetail()
          : base("Extract Detail", "Extract Detail",
              "Extract Detail",
              CommonProps.category, CommonProps.subcate8)
        {
            Message = CommonProps.initialMessage;
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddParameter(new Param_Detail(), "Detail", "D", "Detail", GH_ParamAccess.item);
            pManager[0].Optional = true;
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.RegisterParam(new Param_Node(), "Node", "N", "Node", GH_ParamAccess.item);
            pManager.RegisterParam(new Param_Element1D(), "Elements", "E", "Elements", GH_ParamAccess.list);
            pManager.AddVectorParameter("Vectors", "V", "UnifiedVectors", GH_ParamAccess.list);
            pManager.AddTextParameter("Type", "T", "Detail Type", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // --- variables ---
            GH_Detail gDetail = null;

            // --- input --- 
            if (!DA.GetData(0, ref gDetail)) { return; }
            Detail detail = gDetail.Value;

            // --- solve ---
            GH_Node node = new GH_Node(detail.Node);
            List<GH_Element1D> elements = detail.Elements.ConvertAll(e => new GH_Element1D(e));
            List<Vector3d> vectors = detail.Elements.ConvertAll(e => detail.ElementsUnifiedVectorsMap[e]);
            string type = detail.Type.ToString();

            // --- output ---
            DA.SetData(0, node);
            DA.SetDataList(1, elements);
            DA.SetDataList(2, vectors);
            DA.SetData(3, type);
        }

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return PTK.Properties.Resources.ExtDetail;
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("dd8adcf2-521c-44a4-8448-f0335469c0dd"); }
        }
    }
}
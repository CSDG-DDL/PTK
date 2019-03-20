using System;
using System.Collections.Generic;

using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;


namespace PTK
{
    public class PTK_ExtractNode : GH_Component
    {
        public PTK_ExtractNode()
          : base("Deconstruct Node", "DN",
              "Deconstructs a node into its properties. 0.5 Version has limited options",
              CommonProps.category, CommonProps.subcate8)
        {
            Message = CommonProps.initialMessage;
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddParameter(new Param_Node(), "Node", "N", "Node", GH_ParamAccess.item);
            pManager[0].Optional = true;
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddPointParameter("Point", "P", "Node point", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // --- variables ---
            GH_Node gNode = null;

            // --- input --- 
            if (!DA.GetData(0, ref gNode)) { return; }
            Node node = gNode.Value;

            // --- solve ---
            Point3d p = node.Point;
            

            // --- output ---
            DA.SetData(0, p);
        }

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return PTK.Properties.Resources.ExtNode;
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("dd8adcf2-521c-44a4-8448-f0335469c0dd"); }
        }
    }
}
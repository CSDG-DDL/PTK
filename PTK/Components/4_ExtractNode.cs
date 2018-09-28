using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace PTK.Components
{
    public class ExtractNode : GH_Component
    {
        public ExtractNode()
          : base("ExtractNode", "Nickname",
              "Description",
              CommonProps.category, CommonProps.subcate4)
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("N", "Node", "", GH_ParamAccess.list);

        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddIntegerParameter("ID", "", "", GH_ParamAccess.list);
            pManager.AddPointParameter("Point", "", "", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {

            List<Node> Nodes = new List<Node>();

            DA.GetDataList(0, Nodes);

            List<int> id = new List<int>();
            List<Point3d> pt = new List<Point3d>();



            foreach (Node node in Nodes)
            {
                

                pt.Add(node.Point);


            }

            DA.SetDataList(0, id);
            DA.SetDataList(1, pt);



        }

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return Properties.Resources.Node;
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("274e1a9e-68f1-46d4-9cdf-6786be3412bf"); }
        }
    }
}
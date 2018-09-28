using System;
using System.Collections.Generic;

using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;

using Rhino.Geometry;
using Rhino.Display;

namespace PTK.Components
{
    public class PTK_PreviewPriority : GH_Component
    {

        private DataTree<TextDot> dotTree = new DataTree<TextDot>();

        public PTK_PreviewPriority()
          : base("PreviewPriority", "PreviewPriority",
              "PreviewPriority",
              CommonProps.category, CommonProps.subcate6)
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddParameter(new Param_PriorityModel(), "PriorityModel", "PM", "PriorityModel", GH_ParamAccess.item);
            pManager.AddNumberParameter("Distance", "D", "Distance from corner to display numbers", GH_ParamAccess.item,10);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.Register_GenericParam("Dot", "D", "TextDot", GH_ParamAccess.tree);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            GH_PriorityModel gPriorityModel = null;
            double distance = 0.0;

            if (!DA.GetData(0, ref gPriorityModel)) { return; }
            DA.GetData(1,ref distance);

            PriorityModel priorityModel = gPriorityModel.Value;


            dotTree.Clear();
            int path = 0;
            foreach (Detail d in priorityModel.Details)
            {
                List<TextDot> dots = new List<TextDot>();
                foreach(KeyValuePair<Element1D,int> kvp in d.ElementsPriorityMap)
                {
                    Element1D elem = kvp.Key;
                    double param = d.SearchNodeParamAtElement(elem);
                    double length = 0.0;
                    //double length = elem.BaseCurve.GetLength(Math.Abs(param - elem.BaseCurve.GetLength()) / elem.BaseCurve.GetLength());
                    if (param == 0.0)
                    {
                        length = distance;
                    }
                    else
                    {
                        length = elem.BaseCurve.GetLength(new Interval(0.0,param)) - distance;
                    }
                    Point3d dotPoint = elem.BaseCurve.PointAtLength(length);
                    dots.Add(new TextDot(elem.Tag + ":" + kvp.Value.ToString(), dotPoint));
                }
                dotTree.AddRange(dots, new GH_Path(path));
                path++;
            }
            DA.SetDataTree(0, dotTree);
        }

        public override void DrawViewportMeshes(IGH_PreviewArgs args)
        {
            base.DrawViewportMeshes(args);

            foreach (List<TextDot> dots in dotTree.Branches)
            {
                foreach(TextDot dot in dots)
                {
                    args.Display.DrawDot(dot.Point, dot.Text);
                }
            }
        }

        public override void DrawViewportWires(IGH_PreviewArgs args)
        {
            base.DrawViewportWires(args);
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
            get { return new Guid("98d74da2-324e-4749-9ba0-80c29a1ad297"); }
        }
    }
}
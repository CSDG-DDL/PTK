using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace PTK.Components
{
    public class PTK_PreviewElement : GH_Component
    {

        List<Brep> PrevElements = new List<Brep>();


        public PTK_PreviewElement()
          : base("Preview Element", "PrevElem",
              "Preview Element",
              CommonProps.category, CommonProps.subcate2)
        {
            Message = CommonProps.initialMessage;
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddParameter(new Param_Element1D(), "Elements", "E", "Add elements here", GH_ParamAccess.item);
            pManager[0].Optional = true;
            pManager.AddBooleanParameter("Show Simplified?", "D", "If true, the simplified element is outputed. If false, subElements are outputed", GH_ParamAccess.item,false);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Model", "M", "3d model", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // --- variables ---
            GH_Element1D gElement = null;
            bool check = true;
            Dictionary<Brep, Color> tmpModels = new Dictionary<Brep, Color>();

            // --- input --- 
            if (!DA.GetData(0, ref gElement)) { return; }
            if (!DA.GetData(1, ref check)) { return; }
            Element1D element = gElement.Value;



            List<Brep> SubElements = new List<Brep>();


            if (check)
            {
                SubElements.Add(element.GenerateSimplifiedGeometry());
            }
            else
            {
                foreach (SubElement S in element.Composite.Subelements)
                {
                    SubElements.Add(S.GenerateElementGeometry());
                }
            }
            






            PrevElements.AddRange(SubElements);


            
            

            DA.SetDataList(0, SubElements);
        }


        





        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return Properties.Resources.PreElement;
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("7da0c2a7-ccb0-4f9e-b383-43b74bf56375"); }
        }

        public override void ExpireSolution(bool recompute)
        {
            PrevElements.Clear();
            base.ExpireSolution(recompute);
        }

        //public override BoundingBox ClippingBox => models.Keys.ToList()[0].GetBoundingBox(false);
        public override void DrawViewportMeshes(IGH_PreviewArgs args)
        {
            foreach(var m in PrevElements)
            {
                //args.Display.DrawObject(m.Key, new Rhino.Display.DisplayMaterial(m.Value, 0.5));
                args.Display.DrawBrepShaded(m, new Rhino.Display.DisplayMaterial(Color.SaddleBrown,0.5));
            }
            //base.DrawViewportMeshes(args);
        }
        public override void DrawViewportWires(IGH_PreviewArgs args)
        {
            //base.DrawViewportWires(args);
        }
    }
}
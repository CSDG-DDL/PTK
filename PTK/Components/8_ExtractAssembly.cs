using System;
using System.Collections.Generic;

using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;

namespace PTK
{
    public class PTK_ExtractAssembly : GH_Component
    {
        public PTK_ExtractAssembly()
          : base("Deconstruct Assembly", "DA",
              "Deconstructs an assembly into elements, nodes and details",
              CommonProps.category, CommonProps.subcate8)
        {
            Message = CommonProps.initialMessage;
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddParameter(new Param_Assembly(), "Assembly", "A", "Assembly", GH_ParamAccess.item);
            pManager[0].Optional = true;
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.RegisterParam(new Param_Element1D(), "Elements", "E", "Elements", GH_ParamAccess.list);
            pManager.RegisterParam(new Param_Node(), "Nodes", "N", "Nodes", GH_ParamAccess.list);
            pManager.RegisterParam(new Param_Detail(), "Details", "D", "Details", GH_ParamAccess.list);
            pManager.AddTextParameter("Tags", "T", "Tag list held by Elements included in Assemble", GH_ParamAccess.list);
            pManager.RegisterParam(new Param_CroSec(), "CrossSection", "S", "CrossSection list held by Elements included in Assemble", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // --- variables ---
            GH_Assembly gAssembly = null;
            Assembly assembly = null;

            // --- input --- 
            if (!DA.GetData(0, ref gAssembly)) { return; }
            assembly = gAssembly.Value;

            // --- solve ---
            List<GH_Element1D> elems = assembly.Elements.ConvertAll(e => new GH_Element1D(e));
            List<GH_Node> nodes = assembly.Nodes.ConvertAll(n => new GH_Node(n));
            List<GH_Detail> details = assembly.Details.ConvertAll(d => new GH_Detail(d));
            List<string> tags = assembly.Tags;
            List<GH_CroSec> sections = assembly.CrossSections.ConvertAll(s => new GH_CroSec(s));

            // --- output ---
            DA.SetDataList(0, elems);
            DA.SetDataList(1, nodes);
            DA.SetDataList(2, details);
            DA.SetDataList(3, tags);
            DA.SetDataList(4, sections);
        }

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return PTK.Properties.Resources.ExtAssemble;
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("807ac401-b08a-4702-8328-84b152af5724"); }
        }
    }
}
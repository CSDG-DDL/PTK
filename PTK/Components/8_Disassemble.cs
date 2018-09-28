using System;
using System.Collections.Generic;

using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;

namespace PTK
{
    public class PTK_U_Disassemble : GH_Component
    {
        public PTK_U_Disassemble()
          : base("Disassemble", "Disassemble",
              "Disassemble PTK Assemble Model",
              CommonProps.category, CommonProps.subcate8)
        {
            Message = CommonProps.initialMessage;
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Assembly", "A", "Assembly", GH_ParamAccess.item);
            pManager[0].Optional = true;
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.RegisterParam(new Param_Element1D(), "Elements", "E", "PTK Elements", GH_ParamAccess.list);
            pManager.RegisterParam(new Param_Node(), "Nodes", "N", "PTK Nodes", GH_ParamAccess.list);
            pManager.AddTextParameter("Tags", "T", "Tag list held by Elements included in Assemble", GH_ParamAccess.list);
            pManager.RegisterParam(new Param_MaterialProperty(), "Material properties", "MP", "Material property list held by Elements included in Assemble", GH_ParamAccess.list);
            pManager.RegisterParam(new Param_CroSec(), "CrossSection", "S", "CrossSection list held by Elements included in Assemble", GH_ParamAccess.list);
            pManager.AddIntegerParameter("NodeIDs Connnected Element", "EtoN", "NodeIDs to which the Element is connected", GH_ParamAccess.tree);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // --- variables ---
            GH_Assembly gAssembly = null;
            Assembly assembly = null;
            DataTree<int> nodeMap = new DataTree<int>();

            // --- input --- 
            if (!DA.GetData(0, ref gAssembly)) { return; }
            assembly = gAssembly.Value;

            // --- solve ---
            List<GH_Element1D> elems = assembly.Elements.ConvertAll(e => new GH_Element1D(e));
            List<GH_Node> nodes = assembly.Nodes.ConvertAll(n => new GH_Node(n));
            List<string> tags = assembly.Tags;
            List<GH_MaterialProperty> materialProperties = assembly.MaterialProperties.ConvertAll(m => new GH_MaterialProperty(m));
            List<GH_CroSec> sections = assembly.CrossSections.ConvertAll(s => new GH_CroSec(s));

            int path = 0;
            foreach (List<int> ids in assembly.NodeMap.Values)
            {
                nodeMap.AddRange(ids,new GH_Path(path));
                path++;
            }

            // --- output ---
            DA.SetDataList(0, elems);
            DA.SetDataList(1, nodes);
            DA.SetDataList(2, tags);
            DA.SetDataList(3, materialProperties);
            DA.SetDataList(4, sections);
            DA.SetDataTree(5, nodeMap);
        }

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return PTK.Properties.Resources.Disassemble;
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("807ac401-b08a-4702-8328-84b152af5724"); }
        }
    }
}
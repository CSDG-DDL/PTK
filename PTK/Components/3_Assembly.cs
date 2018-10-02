using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;

using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PTK
{
    public class PTK_Assembly : GH_Component
    {

        public PTK_Assembly()
          : base("Assembly", "Assembly",
              "Assembly",
              CommonProps.category, CommonProps.subcate3)
        {
            Message = CommonProps.initialMessage;
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddParameter(new Param_Element1D(), "Elements", "E", "Add elements here", GH_ParamAccess.list);
            pManager.AddGenericParameter("DetailingGroupDefinitions", "DG", "Add detailingroups here", GH_ParamAccess.list);
            pManager[0].Optional = true;
            pManager[1].Optional = true;
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.RegisterParam(new Param_Assembly(), "Assembly", "A", "Assembled project data", GH_ParamAccess.item);
            pManager.RegisterParam(new Param_Node(), "Nodes", "N", "Nodes included in the Assembly", GH_ParamAccess.list);
            pManager.AddTextParameter("Tags", "T", "Tag list held by Elements included in Assemble", GH_ParamAccess.list);
            pManager.RegisterParam(new Param_MaterialProperty(), "Material properties", "M", "Material property list held by Elements included in Assemble", GH_ParamAccess.list);
            pManager.RegisterParam(new Param_CroSec(), "CrossSection", "S", "CrossSection list held by Elements included in Assemble", GH_ParamAccess.list);
        }


        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // --- variables ---
            Assembly assembly = new Assembly();
            List<GH_Element1D> gElems = new List<GH_Element1D>();
            List<Element1D> elems = null;
            List<DetailingGroupRulesDefinition> DetailinGroupDefinitions = new List<DetailingGroupRulesDefinition>();

            // --- input --- 
            if (!DA.GetDataList(0, gElems))
            {
                elems = new List<Element1D>();
            }
            else
            {
                elems = gElems.ConvertAll(e => e.Value);
            }
            if (!DA.GetDataList(1, DetailinGroupDefinitions))
            {
                DetailinGroupDefinitions = new List<DetailingGroupRulesDefinition>();
            }
            

            // --- solve ---
            elems.ForEach(e => assembly.AddElement(e));
            assembly.GenerateDetails();
            
            foreach(DetailingGroupRulesDefinition DG in DetailinGroupDefinitions)
            {
                assembly.AddDetailingGroup(DG.GenerateDetailingGroup(assembly.Details)); 
            }

            // --- output ---
            List<GH_Node> nodes = assembly.Nodes.ConvertAll(n => new GH_Node(n));
            List<string> tags = assembly.Tags;
            List<GH_MaterialProperty> materialProperties = assembly.MaterialProperties.ConvertAll(m => new GH_MaterialProperty(m));
            List<GH_CroSec> sections = assembly.CrossSections.ConvertAll(s => new GH_CroSec(s));
            
            DA.SetData(0, new GH_Assembly(assembly));
            DA.SetDataList(1, nodes);
            DA.SetDataList(2, tags);
            DA.SetDataList(3, materialProperties);
            DA.SetDataList(4, sections);
        }

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return PTK.Properties.Resources.Assemble;
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("d16b2f49-a170-4d47-ae63-f17a4907fed1"); }
        }
    }
}

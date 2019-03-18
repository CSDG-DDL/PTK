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
          : base("Reindeer Assembly", "Assembly",
              "Assemble all elements in this component.",
              CommonProps.category, CommonProps.subcate3)
        {
            Message = CommonProps.initialMessage;
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddParameter(new Param_Element1D(), "Elements", "E", "Add elements here", GH_ParamAccess.list);
            pManager[0].Optional = true;
            pManager[0].DataMapping = GH_DataMapping.Flatten;

        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.RegisterParam(new Param_Assembly(), "Assembly", "A", "Assembled project data", GH_ParamAccess.item);
        }


        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // --- variables ---
            List<GH_Element1D> gElems = new List<GH_Element1D>();
            List<Element1D> elems = null;


            // --- input --- 
            if (!DA.GetDataList(0, gElems))
            {
                elems = new List<Element1D>();
            }
            else
            {
                elems = gElems.ConvertAll(e => e.Value);
            }

            

            // --- solve ---
            Assembly assembly = new Assembly();

            elems.ForEach(e => assembly.AddElement(e));
                                     
            
            
            DA.SetData(0, new GH_Assembly(assembly));

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

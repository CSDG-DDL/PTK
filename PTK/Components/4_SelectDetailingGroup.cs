using System;
using System.Collections.Generic;
using System.Drawing;
using Grasshopper.Kernel;
using Rhino.Geometry;
using System.Linq;
using Grasshopper;


namespace PTK.Components
{
    public class PTK_SelectDetailingGroup : GH_Component
    {
        public PTK_SelectDetailingGroup()
          : base("SelectDetailingGroup", "DG",
              "Use the group name to select a detailing group",
              CommonProps.category, CommonProps.subcate3)
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddParameter(new Param_Assembly(), "Assembly", "A", "Assembly", GH_ParamAccess.item);
            pManager.AddTextParameter("DetailingGroupName", "DN", "DetailingGroupName", GH_ParamAccess.item, "DetailingGroupName");  
            pManager.AddIntegerParameter("Sorting rule", "SR", "0=Structural, 1=Alphabetical, 2=ElementLength", GH_ParamAccess.item, 0);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.RegisterParam(new Param_Detail(), "Details", "D", "Details", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // --- variables ---
            GH_Assembly ghAssembly = new GH_Assembly();
            Assembly assembly = new Assembly();
            string name = "";
            int priorityKey = 0;

            // --- input --- 
            if (!DA.GetData(0, ref ghAssembly)) { return; }
            assembly = ghAssembly.Value;
            if (!DA.GetData(1, ref name)) { return; }
            if (!DA.GetData(2, ref priorityKey)) { return; }

            //Until now, the slider is a hypothetical object.
            // This command makes it 'real' and adds it to the canvas.

            // --- solve ---
            if (assembly.DetailingGroups.Any(t => t.Name == name))
            {
                List<Detail> details = assembly.DetailingGroups.Find(t => t.Name == name).Details;

                foreach (Detail detail in details)
                {
                    detail.GenerateUnifiedElementVectors();
                    detail.SortElement(priorityKey);
                }

                // --- output ---
                DA.SetDataList(0, details.ConvertAll(d => new GH_Detail(d)));
            }
        }

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return Properties.Resources.SearchDetail;
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("018213a3-efa0-45b0-b444-33259ad18f81"); }
        }
    }
}
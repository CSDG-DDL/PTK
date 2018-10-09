using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;

using Rhino.Geometry;

namespace PTK.Components
{
    public class PTK_Composite : GH_Component
    {
        public PTK_Composite()
          : base("Composite CrossSection", "Composite",
              "creates a sub element",
              CommonProps.category, CommonProps.subcate2)
        {
            Message = CommonProps.initialMessage;
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Name", "N", "Add name to the sub-element.", GH_ParamAccess.item, "Not Named Composite");
            pManager.AddParameter(new Param_CroSec(), "Cross-sections", "S", "Sub CrossSections", GH_ParamAccess.list);
            pManager.AddParameter(new Param_Alignment(), "Alignments", "A", "Alignmnet", GH_ParamAccess.list);
            pManager[0].Optional = true;
            pManager[1].Optional = true;
            pManager[2].Optional = true;
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.RegisterParam(new Param_CroSec(), "Cross Section", "S", "Cross Section data to be connected in the materializer", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // --- variables ---
            string name = null;
            List<GH_CroSec> gCrossSections = new List<GH_CroSec>();
            List<CrossSection> crossSections = null;
            List<GH_Alignment> gAlignmnet = new List<GH_Alignment>();
            List<Alignment> alignments = null;
            Composite composite = null;

            // --- input --- 
            if (!DA.GetData(0, ref name)) { return; }
            if (!DA.GetDataList(1, gCrossSections)) { return; }
            crossSections = gCrossSections.ConvertAll(s => s.Value);
            if (!DA.GetDataList(2, gAlignmnet)) { return; }
            alignments = gAlignmnet.ConvertAll(a => a.Value);

            // --- solve ---
            if (crossSections.Count == 0 || alignments.Count == 0) { return; }

            composite = new Composite(name);
            if (crossSections.Count > alignments.Count)
            {
                while (crossSections.Count>alignments.Count)
                {
                    alignments.Add(alignments.Last());
                }
            }
            if (crossSections.Count < alignments.Count)
            {
                while (crossSections.Count < alignments.Count)
                {
                    crossSections.Add(crossSections.Last());
                }
            }

            for (int i = 0; i < crossSections.Count; i++)
            {
                composite.AddCrossSection(crossSections[i], alignments[i]);
            }

            GH_CroSec sec = new GH_CroSec(composite);

            // --- output ---
            DA.SetData(0, sec);
        }

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return Properties.Resources.Composite;
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("9c4880e6-f925-484b-9ec1-cf5cf466d417"); }
        }
    }
}
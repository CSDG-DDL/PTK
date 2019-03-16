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
        /// <summary>
        /// Overrides the exposure level in the components category 
        /// </summary>
        public override GH_Exposure Exposure
        {
            get
            { return GH_Exposure.tertiary; }
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Name", "N", "Add name to the sub-element.", GH_ParamAccess.item, "Not Named Composite");
            pManager.AddGenericParameter("Cross-sections", "S", "Sub CrossSections", GH_ParamAccess.list);
            pManager[0].Optional = true;
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.RegisterParam(new Param_CroSec(), "Cross Section", "S", "Cross Section data to be connected in the Element", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // --- variables ---
            string name = null;
            List<CompositeInput> CompositeInputs = new List<CompositeInput>();


            // --- input --- 
            if (!DA.GetData(0, ref name)) { return; }
            if (!DA.GetDataList(1, CompositeInputs)) { return; }


            // --- solve ---

            List<CrossSection> Crossections = new List<CrossSection>();


            foreach(CompositeInput C in CompositeInputs)
            {
                Crossections.AddRange(C.CrossSections);
            }

            CompositeInput Composite = new CompositeInput(name, Crossections);



            DA.SetData(0, Composite);
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
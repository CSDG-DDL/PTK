using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace PTK
{
    public class PTK_SimpleAlignment : GH_Component
    {
        public PTK_SimpleAlignment()
          : base("LocalAlignment", "Align",
              "Alignment",
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
            { return GH_Exposure.quinary; }
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Name", "N", "Alignment Name", GH_ParamAccess.item, "Not Named Alignment");
            pManager.AddNumberParameter("Offset Y", "Y", "Offset length local y", GH_ParamAccess.item, 0.0);
            pManager.AddNumberParameter("Offset Z", "Z", "Offset length local z", GH_ParamAccess.item, 0.0);
            pManager.AddNumberParameter("Rotation Angle", "R", "Rotational angle in degree", GH_ParamAccess.item, 0.0);
            pManager.AddVectorParameter("Along Vector", "V", "Rotate the Z direction of the section plane along the direction of this vector", GH_ParamAccess.item, new Vector3d());

            pManager[0].Optional = true;
            pManager[1].Optional = true;
            pManager[2].Optional = true;
            pManager[3].Optional = true;
            pManager[4].Optional = true;
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.RegisterParam(new Param_Alignment(), "Alignment", "A", "AlignmentData to be added to materializer", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // --- variables ---
            string name = null;
            double offsetY = new double();
            double offsetZ = new double();
            double rotationAngle = new double();
            Vector3d alongVector = new Vector3d();

            // --- input --- 
            if (!DA.GetData(0, ref name)) { return; }
            if (!DA.GetData(1, ref offsetY)) { return; }
            if (!DA.GetData(2, ref offsetZ)) { return; }
            if (!DA.GetData(3, ref rotationAngle)) { return; }
            if (!DA.GetData(4, ref alongVector)) { return; }

            // --- solve ---
            GH_Alignment ali = new GH_Alignment(new Alignment(name, offsetY, offsetZ, rotationAngle, alongVector));

            // --- output ---
            DA.SetData(0, ali);
        }

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return PTK.Properties.Resources.Alignment;
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("a2017425-8288-4c78-9111-f9d9588a34df"); }
        }
    }
}
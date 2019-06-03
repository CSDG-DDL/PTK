﻿using System;
using System.Collections.Generic;
using System.Windows.Forms;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace PTK
{
    public class PTK_CircularCrossSection : GH_Component
    {
        public PTK_CircularCrossSection()
          : base("Circular Cross Section", "CirCroSec",
              "CrossSection is being generated based on width, height, alignment and height-direction ",
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
            pManager.AddTextParameter("Name", "N", "Add Cross Section Name", GH_ParamAccess.item, "Not Named CirCroSec");
            pManager.AddNumberParameter("Radius", "R", "", GH_ParamAccess.item,100);
            pManager.AddParameter(new Param_MaterialProperty(), "Material properties", "M", "Add material properties", GH_ParamAccess.item);
            pManager.AddParameter(new Param_Alignment(), "Alignment", "A", "Local Alignment", GH_ParamAccess.item);

            pManager[0].Optional = true;
            pManager[1].Optional = true;
            pManager[2].Optional = true;
            pManager[3].Optional = true;
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.RegisterParam(new Param_CroSec(), "Cross Section", "S", "Cross Section data to be connected in the Element", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // --- variables ---
            string name = null;
            double radius = new double();
            GH_MaterialProperty gMaterial = null;
            MaterialProperty material = null;
            GH_Alignment gAlignment = null;
            Alignment alignment = null;

            // --- input --- 
            if (!DA.GetData(0, ref name)) { return; }
            if (!DA.GetData(1, ref radius)) { return; }
            if (!DA.GetData(2, ref gMaterial)) { return; }
            material = gMaterial.Value;
            if (!DA.GetData(3, ref gAlignment)) { return; }
            alignment = gAlignment.Value;

            // --- solve ---
            GH_CroSec sec = new GH_CroSec(new CircularCroSec(name, material, radius, alignment));

            // --- output ---
            DA.SetData(0, sec);
        }

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return PTK.Properties.Resources.CircularCS;
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("0CE57537-D208-4F70-93F2-055616F37675"); }
        }
    }
}
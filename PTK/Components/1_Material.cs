﻿using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;

namespace PTK
{
    public class PTK_Material : GH_Component
    {
        public PTK_Material()
          : base("Material", "Mat", "Create a Material",
              CommonProps.category, CommonProps.subcate1)
        {
            Message = CommonProps.initialMessage;
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Name", "N", "Material Name in large classification such as \"Wood\", \"Iron\"", GH_ParamAccess.item, "Not Named Material");
            pManager.AddParameter(new Param_MaterialProperty(), "Structural Material Prop", "SMP", "Data representing the structural nature of the material", GH_ParamAccess.item);
            pManager.AddColourParameter("Color", "C", "Preview Color", GH_ParamAccess.item,new System.Drawing.Color());
            pManager[0].Optional = true;
            pManager[1].Optional = true;
            pManager[2].Optional = true;
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.RegisterParam(new Param_Material(), "Material", "M", "Data of the Material constituting the Element");
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // --- variables ---
            string name = null;
            GH_MaterialProperty gProp = null;
            MaterialProperty prop = null;
            GH_Colour gColor = null;
            Color color;

            // --- input --- 
            if (!DA.GetData(0, ref name)) { return; }
            if (!DA.GetData(1, ref gProp)) {
                prop = new MaterialProperty("Not Named Material Prop");
            }
            else
            {
                prop = gProp.Value;
            }
            if(!DA.GetData(2,ref gColor)) { return; }
            color = gColor.Value;


            // --- solve ---
            GH_Material material = new GH_Material(new Material(name, prop, color));

            // --- output ---
            DA.SetData(0, material);
        }

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return PTK.Properties.Resources.Material;
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("911bef7b-feea-46d8-abe9-f686d11b9c41"); }
        }
    }
}
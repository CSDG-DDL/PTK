﻿using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;

namespace PTK
{
    public class PTK_UTIL_2_Disassemble : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the PTK_Util_2_Disassemble class.
        /// </summary>
        public PTK_UTIL_2_Disassemble()
          : base("Disassemble", "DA (PTK)",
              "Disassemble PTK Assemble Model",
              "PTK", "UTIL")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("PTK Assembly", "PTK A", "PTK Assembly", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("PTK NODE", "PTK N", "PTK NODE", GH_ParamAccess.item);
            pManager.AddGenericParameter("PTK ELEM", "PTK E", "PTK ELEM", GH_ParamAccess.item);
            pManager.AddGenericParameter("PTK MAT", "PTK M", "PTK MAT", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            #region variables
            GH_ObjectWrapper wrapAssembly = new GH_ObjectWrapper();
            Assembly assemble;
            List<Node> nodes = new List<Node>();
            List<Element> elems = new List<Element>();
            List<Material> mats = new List<Material>();
            #endregion

            #region input
            if (!DA.GetData(0, ref wrapAssembly)) { return; }
            #endregion

            #region solve
            wrapAssembly.CastTo<Assembly>(out assemble);
            nodes = assemble.Nodes;
            elems = assemble.Elems;
            mats = assemble.Mats;
            #endregion

            #region output
            DA.SetData(0, nodes);
            DA.SetData(1, elems);
            DA.SetData(2, mats);
            #endregion
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
                return PTK.Properties.Resources.icontest2;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("807ac401-b08a-4702-8328-84b152af5724"); }
        }
    }
}
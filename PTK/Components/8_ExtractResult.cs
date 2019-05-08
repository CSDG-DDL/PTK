using System;
using System.Collections.Generic;

using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;

namespace PTK
{
    public class PTK_ExtractResult : GH_Component
    {
        public PTK_ExtractResult()
          : base("Deconstruct Result", "DR",
              "Deconstructs a result into into values",
              CommonProps.category, CommonProps.subcate8)
        {
            Message = CommonProps.initialMessage;
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter( "Result", "R", "Result", GH_ParamAccess.item);
            pManager[0].Optional = true;
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddNumberParameter("LoadCase", "LC", "Load case of the force", GH_ParamAccess.item);               //0
            pManager.AddNumberParameter("KarambaID", "KID", "Index of the karamba element", GH_ParamAccess.item);       //1
            pManager.AddNumberParameter("Position", "P", "Position on the element", GH_ParamAccess.item);               //2
            pManager.AddPointParameter("PositionPT", "PPT", "Point3d of the force", GH_ParamAccess.item);               //3
            pManager.AddNumberParameter("Tension", "UT", "Tension utilization", GH_ParamAccess.item);                        //4
            pManager.AddNumberParameter("Compression", "UC", "Compression utilization", GH_ParamAccess.item);        //5
            pManager.AddNumberParameter("Bending", "UB", "Bending utilization", GH_ParamAccess.item);        //6
            pManager.AddNumberParameter("Combined", "UBC", "Combined compression and bending utilization", GH_ParamAccess.item);                  //7
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // --- variables ---
            Result result = null;

            // --- input --- 
            if (!DA.GetData(0, ref result)) { return; }
            

            // --- solve ---


            // --- output ---
            DA.SetData(0, result.loadcase);
            DA.SetData(1, result.karambaElementID);
            DA.SetData(2, result.position);
            DA.SetData(3, result.positionPoint);
            DA.SetData(4, result.utilTension);
            DA.SetData(5, result.utilCompression);
            DA.SetData(6, result.utilBending);
            DA.SetData(7, result.utilCombinedAxialBending);


        }

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return PTK.Properties.Resources.Force;
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("dd123cf2-521c-11a4-1331-f0335469c0dd"); }
        }
    }
}

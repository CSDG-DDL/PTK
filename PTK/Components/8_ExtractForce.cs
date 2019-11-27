using System;
using System.Collections.Generic;

using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;


namespace PTK
{
    public class PTK_ExtractForce : GH_Component
    {
        public PTK_ExtractForce()
          : base("Deconstruct Force", "DF",
              "Deconstructs a force into into values",
              CommonProps.category, CommonProps.subcate8)
        {
            Message = CommonProps.initialMessage;
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddParameter(new Param_Force(), "Force", "F", "Force", GH_ParamAccess.item);
            pManager[0].Optional = true;
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddNumberParameter("LoadCase", "LC", "Load case of the force", GH_ParamAccess.item);               //0
            pManager.AddNumberParameter("KarambaID", "KID", "Index of the karamba element", GH_ParamAccess.item);       //1
            pManager.AddNumberParameter("Position", "P", "Position on the element", GH_ParamAccess.item);               //2
            pManager.AddPointParameter("PositionPT", "PPT", "Point3d of the force", GH_ParamAccess.item);               //3
            pManager.AddNumberParameter("AxialForce", "FX", "Axial force", GH_ParamAccess.item);                        //4
            pManager.AddNumberParameter("ShearForce1", "FY", "Shear force in direction 1", GH_ParamAccess.item);        //5
            pManager.AddNumberParameter("ShearForce2", "FZ", "Shear force in direction 2", GH_ParamAccess.item);        //6
            pManager.AddNumberParameter("TorsionMoment", "MX", "Torsion moment", GH_ParamAccess.item);                  //7
            pManager.AddNumberParameter("BendingMoment1", "MY", "Bending moment in direction 1", GH_ParamAccess.item);  //8
            pManager.AddNumberParameter("BendingMoment2", "MZ", "Bending moment in direction 2", GH_ParamAccess.item);  //9
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // --- variables ---
            GH_Force gForce = null;

            // --- input --- 
            if (!DA.GetData(0, ref gForce)) { return; }
            Force force = gForce.Value;

            // --- solve ---


            // --- output ---
            DA.SetData(0, force.loadcase);
            DA.SetData(1, force.karambaElementID);
            DA.SetData(2, force.position);
            DA.SetData(3, force.positionPoint);
            DA.SetData(4, force.FX);
            DA.SetData(5, force.FY);
            DA.SetData(6, force.FZ);
            DA.SetData(7, force.MX);
            DA.SetData(8, force.MY);
            DA.SetData(9, force.MZ);

        }

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return PTK.Properties.Resources.ExtForce;
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("dd123cf2-521c-11a4-8448-f0335469c0dd"); }
        }
    }
}
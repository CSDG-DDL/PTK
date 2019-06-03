using System;
using System.Collections.Generic;

using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;


namespace PTK
{
    public class PTK_ExtractStructuralForces : GH_Component
    {
        public PTK_ExtractStructuralForces()
          : base("Deconstruct Structural Forces", "DSF",
              "Deconstructs a structural force class into the forces",
              CommonProps.category, CommonProps.subcate8)
        {
            Message = CommonProps.initialMessage;
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddParameter(new Param_StructuralData(), "StructuralData", "SD", "StructuralData", GH_ParamAccess.item);
            pManager[0].Optional = true;
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new Param_Force(),"MaximumCompression", "FXC", "Maximum compression force in the element", GH_ParamAccess.item);
            pManager.AddParameter(new Param_Force(), "MaximumTension", "FXT", "Maximum tension force in the element", GH_ParamAccess.item);
            pManager.AddParameter(new Param_Force(), "MaximumShear1", "FY", "Maximum shear force in the element", GH_ParamAccess.item);
            pManager.AddParameter(new Param_Force(), "MaximumShear2", "FZ", "Maximum shear force in the element", GH_ParamAccess.item);
            pManager.AddParameter(new Param_Force(), "MaximumTorsion", "MX", "Maximum bending in the element", GH_ParamAccess.item);
            pManager.AddParameter(new Param_Force(), "MaximumBending1", "MY", "Maximum bending in the element", GH_ParamAccess.item);
            pManager.AddParameter(new Param_Force(), "MaximumBending2", "MZ", "Maximum bending in the element", GH_ParamAccess.item);
            pManager.AddParameter(new Param_Force(), "List of Forces", "List", "List of all forces in the element", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // --- variables ---
            GH_StructuralData gSD = null;

            // --- input --- 
            if (!DA.GetData(0, ref gSD)) { return; }
            StructuralData sd = gSD.Value;

            // --- solve ---
            Force FXC = sd.StructuralForces.maxCompressionForce ;
            Force FXT = sd.StructuralForces.maxTensionForce;
            Force FY = sd.StructuralForces.maxShearDir1;
            Force FZ = sd.StructuralForces.maxShearDir2;
            Force MX = sd.StructuralForces.maxTorsion;
            Force MY = sd.StructuralForces.maxBendingDir1;
            Force MZ = sd.StructuralForces.maxBendingDir2 ;
            List<GH_Force> listGHForces = new List<GH_Force>();
            foreach (var ghF in sd.StructuralForces.forces)
            {
                listGHForces.Add(new GH_Force(ghF) );
            }
            

            // --- output ---
            DA.SetData(0, new GH_Force(FXC) );
            DA.SetData(1, new GH_Force(FXT) );
            DA.SetData(2, new GH_Force(FY) );
            DA.SetData(3, new GH_Force(FZ) );
            DA.SetData(4, new GH_Force(MX) );
            DA.SetData(5, new GH_Force(MY) );
            DA.SetData(6, new GH_Force(MZ) );
            DA.SetDataList(7, listGHForces);

        }

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return PTK.Properties.Resources.ExtSD;
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("dd123cf2-521c-44a4-8448-f0335469c0dd"); }
        }
    }
}
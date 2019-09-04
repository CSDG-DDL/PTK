    using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PTK
{
    public class PTK_Element : GH_Component
    {

        public PTK_Element()
          : base("Element", "Element",
              "creates a beam element.",
              CommonProps.category, CommonProps.subcate2)
        {
            Message = CommonProps.initialMessage;
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Tag", "T", "Add a tag to the element here.", GH_ParamAccess.item, "Not Named Element");
            pManager.AddCurveParameter("Base Curve", "CCCC", "Add curves that shall be materalized", GH_ParamAccess.item);
            pManager.AddGenericParameter( "CrossSection", "CS", "CrossSection", GH_ParamAccess.item);
            pManager.AddGenericParameter("Alignment", "A", "Global Alignment", GH_ParamAccess.item);
            pManager.AddParameter(new Param_StructuralData(), "Forces", "F", "Add forces", GH_ParamAccess.item);
            pManager.AddParameter(new Param_Joint(), "Joint", "J", "Add joint", GH_ParamAccess.list);
            pManager.AddIntegerParameter("Structural Priority", "P", "Add integer value to set the priority of the member", GH_ParamAccess.item, 0);
            pManager.AddBooleanParameter("Intersection Nodes?", "I?", "Whether the element intersects other members at other than the end point", GH_ParamAccess.item, true);

            pManager[0].Optional = true;
            pManager[1].Optional = true;
            pManager[2].Optional = true;
            pManager[3].Optional = true;
            pManager[4].Optional = true;
            pManager[5].Optional = true;
            pManager[6].Optional = true;
            pManager[7].Optional = true;
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.RegisterParam(new Param_Element1D(), "Element", "E", "PTK Elements", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            //test

            // --- variables ---
            string tag = null;

           

            Curve curve = null;
            GH_CroSec gCroSec = null;
            ElementAlign ElementAlignment = null;
            CompositeInput Composite = null;
            //GH_StructuralData gStructuralData = null;
            StructuralData structuralData = null;
            List<GH_Joint> gJoints = new List<GH_Joint>();
            List<Joint> joints = null;
            int priority = new int();
            bool intersect = true;

            // --- input --- 
            if (!DA.GetData(0, ref tag)) { return; }
            if (!DA.GetData(1, ref curve)) { return; }
            int test = 0;

            if (!curve.IsLinear())
            {
                if (curve.IsPolyline())
                {
                    curve = new Line(curve.PointAtStart, curve.PointAtEnd).ToNurbsCurve();

                }
                else
                {
                    throw new ArgumentException("Sorry! This version of Reindeer does not allow non-linear curves");
                }


                

            }
            




            if (!DA.GetData(2, ref Composite))

            {
                Composite = new CompositeInput();
            }
                
            
            
            
            if (!DA.GetData(3, ref ElementAlignment))
            {
                GlobalAlignmentRules.AlignmentFromVector VectorAlign = new GlobalAlignmentRules.AlignmentFromVector(new Vector3d(0,0,1));



                ElementAlignment = new ElementAlign(VectorAlign.GenerateVector, 0, 0);

            }
            
            //Generating Alignment
            
            /*
            if (!DA.GetData(4, ref gStructuralData))
            {
                structuralData = new StructuralData();
            }
            else
            {
                structuralData = gStructuralData.Value;
            }
            */

            if (!DA.GetDataList(5, gJoints))
            {
                joints = new List<Joint>();
            }
            else
            {
                joints = gJoints.ConvertAll(j => j.Value);
            }
            if (!DA.GetData(6, ref priority)) { return; }
            if (!DA.GetData(7, ref intersect)) { return; }








            // --- solve ---
            GH_Element1D elem = new GH_Element1D(new Element1D(tag, curve, Composite, ElementAlignment, structuralData, joints, priority, intersect));

            // --- output ---
            DA.SetData(0, elem);
        }

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return PTK.Properties.Resources.Element;
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("0f259d4d-3cf2-4337-9545-c392178e1fe1"); }
        }
    }
}

using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace PTK.Components
{
    public class _11_BTL_Tenon : GH_Component
    {
        List<Rectangle3d> publicRectangle = new List<Rectangle3d>();
        List<Box> publicBox = new List<Box>();
        


        /// <summary>
        /// Initializes a new instance of the _11_BTL_Tenon class.
        /// </summary>
        public _11_BTL_Tenon()
          : base("Tenon", "T",
              "Define Tenon process. NB: Current bug in 0.5. If the defined tenon is larger than the stock, the BTLX wil decrease the size of the tenon. This is not shown in the NURBS-model... Preview does not include fillet/chamfer",
              CommonProps.category, CommonProps.subcate11)
        {
            Message = CommonProps.initialMessage;
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddParameter(new Param_Element1D(), "Element", "E", "Element", GH_ParamAccess.item);
            pManager.AddPlaneParameter("TenonPlane", "P", "Add TenonPlane, X-axis equals length-direction", GH_ParamAccess.item);
            pManager.AddNumberParameter("Length", "L", "Add Length-parameter", GH_ParamAccess.item,50);
            pManager.AddNumberParameter("Width", "W", "Add Width-parameter", GH_ParamAccess.item,50);
            pManager.AddNumberParameter("Height", "H", "Add Height-parameter", GH_ParamAccess.item,80);
            pManager.AddBooleanParameter("ExtendTop?", "T", "Extend tenon upwards?", GH_ParamAccess.item, false);
            pManager.AddBooleanParameter("ExtendBottom?", "B", "Extend tenon Downwards?", GH_ParamAccess.item, false);
            pManager.AddBooleanParameter("Chamfer?", "C", "Chamfer end tip of tenon?", GH_ParamAccess.item, false);
            pManager.AddNumberParameter("Shaperadius", "R", "Add shaperadius. (Active if Shapemode is set to Radius)", GH_ParamAccess.item, 20);
            pManager.AddIntegerParameter("Shapemode", "M", "Choose Shapemode. 0=Automatic, 1=Square, 2=Round, 3=rounded by tool radius, 4=Radius-based",GH_ParamAccess.item, 0);
            pManager.AddBooleanParameter("FlipDirection?", "F", "set True if you want to flip direction of the Mortise", GH_ParamAccess.item, false);
            pManager[5].Optional = true;
            pManager[6].Optional = true;
            pManager[7].Optional = true;
            pManager[8].Optional = true;
            pManager[9].Optional = true;

        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("BTL-Tenon", "T", "", GH_ParamAccess.item);
            pManager.AddPlaneParameter("Plane", "", "", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // --- variables ---
            GH_Element1D gElement = null;


            Plane TenonPlane = new Plane();
            double Length = 30;
            double Width = 30;
            double Height =30;
            bool LengthLimitedTop = true;
            bool LengthLimitedBottom = true;
            bool Chamfer = false ;
            double Shaperadius = 0 ;
            int Shapetype = 0;
            bool FlipPlane = false;
            


            // --- input --- 
            if (!DA.GetData(0, ref gElement)) { return; }
            Element1D element = gElement.Value;

            if (!DA.GetData(1, ref TenonPlane)) { return; }
            if (!DA.GetData(2, ref Length)) { return; }
            if (!DA.GetData(3, ref Width)) { return; }
            if (!DA.GetData(4, ref Height)) { return; }
            DA.GetData(5, ref LengthLimitedTop);
            DA.GetData(6, ref LengthLimitedBottom);
            DA.GetData(7, ref Chamfer);
            DA.GetData(8, ref Shaperadius);
            DA.GetData(9, ref Shapetype);
            DA.GetData(10, ref FlipPlane);


            BooleanType top = BooleanType.no;
            BooleanType bottom = BooleanType.no;
            BooleanType chamfer = BooleanType.no;

            TenonPlane.Translate(-TenonPlane.XAxis * Length/2);
            if (FlipPlane)
            {
                Plane tempplane = new Plane(TenonPlane.Origin, TenonPlane.XAxis, -TenonPlane.YAxis);
                TenonPlane = tempplane;
            }

            if (!LengthLimitedTop) { top = BooleanType.yes; }
            if (!LengthLimitedBottom) { bottom = BooleanType.yes; }
            if (Chamfer) { chamfer = BooleanType.yes; }


            




            // --- solve ---
            BTLTenon tenon = new BTLTenon(TenonPlane, Width, Length, Height, top, bottom, chamfer, Shapetype, Shaperadius,FlipPlane );

            // Making Object with delegate and ID
            OrderedTimberProcess Order = new OrderedTimberProcess(element, new PerformTimberProcessDelegate(tenon.DelegateProcess));


            //////////////////////////////////////////////////////////////////
            /////THIS IS A TEMPORARY GENERATION OF AN ASSEMBLY! START

            //List<Element1D> elems = new List<Element1D>();
            //elems.Add(element);

            //List<OrderedTimberProcess> Orders = new List<OrderedTimberProcess>();
            //Orders.Add(Order);


            //BuildingProject GrasshopperProject = new BuildingProject(new ProjectType());
            //GrasshopperProject.PrepairElements(elems, Orders);
            //GrasshopperProject.ManufactureProject(ManufactureMode.BOTH);


            

            //DA.SetData(1, tenon.RefPlane);



            /////THIS IS A TEMPORARY GENERATION OF AN ASSEMBLY! END
            //////////////////////////////////////////////////////////////////

            Interval x = new Interval(0, Length);
            Interval y = new Interval(-Width / 2, Width / 2);
            Interval z = new Interval(0, Height);
            double offset = Width / 2;
            Interval xx = new Interval(-offset, Length + offset);
            Interval yy = new Interval(-offset - Width / 2, offset + Width / 2);

            Rectangle3d shape = new Rectangle3d(TenonPlane, xx, yy);


            Box tenonshape = new Box(TenonPlane, x, y, z);
            publicBox.Add(tenonshape);
            publicRectangle.Add(shape);


            // --- output ---
            DA.SetData(0, Order);
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
                return Properties.Resources.Tenon;
            }
        }


        public override void ExpireSolution(bool recompute)
        {
            publicRectangle.Clear();
            publicBox.Clear();
            base.ExpireSolution(recompute);

        }

        public override void DrawViewportMeshes(IGH_PreviewArgs args)
        {


            if (publicRectangle.Count >= 1)
            {
                for(int i =0; i< publicRectangle.Count;i++)
                {
                    

                    args.Display.DepthMode = Rhino.Display.DepthMode.AlwaysInFront;

                    args.Display.DrawCurve(publicRectangle[i].ToNurbsCurve(), System.Drawing.Color.Purple,3);
                    
                    args.Display.DrawBrepShaded(publicBox[i].ToBrep(), new Rhino.Display.DisplayMaterial(System.Drawing.Color.Purple));
                }

            }
            else
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Your cut actually does not intersect at any elements!");
            }



        }



        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("fedb6ee7-dad6-406f-8611-10b9eef05f16"); }
        }
    }
}

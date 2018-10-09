using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using GH_IO.Serialization;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Attributes;

using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
using Rhino.Geometry;


namespace PTK
{

    public class PTK_Support : GH_Component
    {
        
        private string boolSupString = "";
        private bool[] boolSupArray = { false, false, false, false, false, false }; // six degrees of freedom
        
        public PTK_Support()
          : base("Support", "Support",
              "Add Supports Conditions here",
              CommonProps.category, CommonProps.subcate3)
        {
            //Message = CommonProps.initialMessage;
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Tag", "T", "Tag", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Load Case", "LC", "Load case", GH_ParamAccess.item, 0); 
            pManager.AddPlaneParameter("Fix Plane", "P", "", GH_ParamAccess.item);
            
            pManager[1].Optional = true;
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("PTK Supports", "Sup (PTK)", "Support data to be send to Assembler(PTK)", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // --- variables ---
            string tag = null;
            int lCase = 0;
            Plane supPln = new Plane();
            List<Support> sups = new List<Support>();

            // --- input --- 
            if (!DA.GetData(0, ref tag)) { return; }
            if (!DA.GetData(1, ref lCase)) { return; }
            if (!DA.GetData(2, ref supPln)) { return; }

            // --- solve ---
            GH_Support sup = new GH_Support(new Support(tag, lCase, supPln, new List<bool>(boolSupArray)));

            // --- output ---
            Message = boolSupString;
            DA.SetData(0, sup);
        }

        public override void CreateAttributes()
        {
            m_attributes = new Attributes_Custom(this);
        }

        public static void Menu_CustomOnClick(PTK_Support _comp)
        {
            if (_comp.boolSupString == "") _comp.boolSupString = "000000";
            Forms.F01_Supports frm = new Forms.F01_Supports(_comp.boolSupString);
            frm.BoolSupString = _comp.boolSupString;

            if (frm.ShowDialog() == DialogResult.OK)
            {
                _comp.boolSupString = frm.BoolSupString;
                _comp.boolSupArray = Support.ConditionsStringToArray(_comp.boolSupString);
                
                _comp.ExpireSolution(true);
            }
        }

        public class Attributes_Custom : GH_ComponentAttributes
        {
            private Rectangle ButtonBounds { get; set; }

            public Attributes_Custom(GH_Component owner) : base(owner) { }

            protected override void Layout()
            {
                base.Layout();

                Rectangle rec0 = GH_Convert.ToRectangle(Bounds);
                rec0.Height += 26;

                Rectangle rec1 = rec0;
                rec1.Y = rec1.Bottom - 26;
                rec1.Height = 26;
                rec1.Inflate(-4, -4);
                
                Bounds = rec0;
                ButtonBounds = rec1;
                
            }

            protected override void Render(GH_Canvas canvas, Graphics graphics, GH_CanvasChannel channel)
            {
                base.Render(canvas, graphics, channel);
                if (channel == GH_CanvasChannel.Objects)
                {
                    GH_Capsule button = GH_Capsule.CreateTextCapsule
                        (ButtonBounds, ButtonBounds, GH_Palette.Black, "Set Supports", 2, 0);
                    button.Render(graphics, Selected, Owner.Locked, false);
                    button.Dispose();
                }
            }

            public override GH_ObjectResponse RespondToMouseDown(GH_Canvas sender, GH_CanvasMouseEvent e)
            {
                if (e.Button == MouseButtons.Left)
                {
                    RectangleF rec = ButtonBounds;
                    if (rec.Contains(e.CanvasLocation))
                    {
                        Menu_CustomOnClick((PTK_Support)Owner);
                        return GH_ObjectResponse.Handled;
                    }
                }
                return base.RespondToMouseDown(sender, e);
            }
        }
        
        // Data saving function
        public override bool Write(GH_IWriter writer)
        {
            writer.SetString("boolSupString", boolSupString);
            return base.Write(writer);
        }

        // Data reading function
        public override bool Read(GH_IReader reader)
        {
            boolSupString = reader.GetString("boolSupString");
            // set boolSupArray values when it loads.
            this.boolSupArray = Support.ConditionsStringToArray(boolSupString);
            return base.Read(reader);
        }

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return PTK.Properties.Resources.Support;
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("965bef7b-feea-46d1-abe9-f686d28c4c41"); }
        }
    }
    
}

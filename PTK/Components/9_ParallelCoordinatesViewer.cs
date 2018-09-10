using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Drawing;

using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Attributes;
using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;

using Newtonsoft.Json;


namespace PTK
{
    public class ParallelCoordinatesViewerComponent : GH_Component
    {
        public List<ParetoSolution> ParetoSolutions { get; set; } = new List<ParetoSolution>();


        public ParallelCoordinatesViewerComponent()
          : base("ParallelCoordinatesViewer", "ParaCoorView",
              "Description",
              CommonProps.category, CommonProps.subcate9)
        {
        }

        public override void CreateAttributes()
        {
            m_attributes = new Attributes_Custom(this);
        }


        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddParameter(new Param_ParetoSolution(), "Pareto Solution", "PS", "Pareto Solution list", GH_ParamAccess.list);
        }


        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<GH_ParetoSolution> gParetoSolutions = new List<GH_ParetoSolution>();

            if(!DA.GetDataList(0, gParetoSolutions))
            {
                ParetoSolutions = new List<ParetoSolution>();
            }
            else
            {
                ParetoSolutions = gParetoSolutions.ConvertAll(p => p.Value);
            }
        }

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return Properties.Resources.ParaCoor;
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("c209addb-d004-4841-937e-15a6a0e9ebdc"); }
        }

        public class Attributes_Custom : GH_ComponentAttributes
        {
            public Attributes_Custom(GH_Component owner) : base(owner)
            {
            }

            private Rectangle ButtonBounds { get; set; }    //Rectangle of button to be added

            //-------Change layout by overwriting setting
            protected override void Layout()
            {
                base.Layout();

                int buttonHeight = 22;      //The height of the button to be added
                Rectangle rec0 = GH_Convert.ToRectangle(Bounds);    //Component rectangle
                rec0.Height += buttonHeight;

                Rectangle rec1 = rec0;
                rec1.Y = rec1.Bottom - buttonHeight;
                rec1.Height = buttonHeight;
                rec1.Inflate(-2, -2);       //Offset button size

                Bounds = rec0;              //Change the whole shape
                ButtonBounds = rec1;        //Change shape of button
            }

            //-------Add button to component
            protected override void Render(GH_Canvas canvas, Graphics graphics, GH_CanvasChannel channel)
            {
                base.Render(canvas, graphics, channel);
                if (channel == GH_CanvasChannel.Objects)
                {
                    GH_Capsule button = GH_Capsule.CreateTextCapsule(ButtonBounds, ButtonBounds, GH_Palette.Black, "Viewer", 2, 0);  //Add button
                    button.Render(graphics, Selected, Owner.Locked, false);
                    button.Dispose();
                }
            }
            //-------Addition of button clicking behavior
            public override GH_ObjectResponse RespondToMouseDown(GH_Canvas sender, GH_CanvasMouseEvent e)
            {
                if (e.Button == MouseButtons.Left)
                {
                    Rectangle rec = ButtonBounds;
                    if (rec.Contains(System.Drawing.Point.Round(e.CanvasLocation)))
                    {
                        ParaCoorOption.ShowParaCoorForm((ParallelCoordinatesViewerComponent)Owner);        //Show Form
                        return GH_ObjectResponse.Handled;
                    }
                }
                return base.RespondToMouseDown(sender, e);
            }
        }

    }

    internal static class ParaCoorOption
    {

        public static ParallelCoordinatesViewerComponent Comp { get; private set; }   //Parent component
        public static GH_Document GhDoc { get; private set; }                   //Reference to Grasshopper's document

        public static ParaCoorForm paraCoorFrom;

        //-------Initialize
        public static void SetParaCoorOption(ParallelCoordinatesViewerComponent _comp)
        {
            Comp = _comp;
            GhDoc = Comp.OnPingDocument();
        }


        public static bool SetParamFromForm(string data)
        {
            var dataDic = JsonConvert.DeserializeObject<Dictionary<string, decimal>>(data);

            foreach (IGH_DocumentObject obj in GhDoc.Objects)
            {
                var slider = obj as Grasshopper.Kernel.Special.GH_NumberSlider;
                if (slider == null) continue;

                foreach (string key in dataDic.Keys)
                {
                    if (slider.NickName == key)
                    {
                        decimal sliMin = slider.Slider.Minimum;
                        decimal d = dataDic[key];
                        slider.SetSliderValue(d * (slider.Slider.Maximum - sliMin) + sliMin);
                        break;
                    }
                }

            }
            GhDoc.NewSolution(false);    //Recalculate uncalculated parts
            return true;
        }

        //-------Show Form
        public static void ShowParaCoorForm(ParallelCoordinatesViewerComponent _comp)
        {
            _comp.ExpireSolution(true);
            SetParaCoorOption(_comp);
            if (paraCoorFrom == null)
            {
                paraCoorFrom = new ParaCoorForm(_comp);
                GH_WindowsFormUtil.CenterFormOnEditor(paraCoorFrom, true);
            }
            paraCoorFrom.Show();
        }
    }

}

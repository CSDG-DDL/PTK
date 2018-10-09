using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Attributes;
using Grasshopper.Kernel.Data;
using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
using Rhino.Geometry;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using Rhino.Display;

namespace PTK.Components
{
    public class PTK_PreviewCrossSection : GH_Param<GH_CroSec>
    {
        private double m_min;
        private double m_max;
        private List<CrossSection> m_secs;
        public double Min => m_min;
        public double Max => m_max;
        public List<CrossSection> Secs => m_secs;

        public PTK_PreviewCrossSection()
            : base(new GH_InstanceDescription("Preview CrossSection", "PrevCroSec", "Preview CrossSection", 
                CommonProps.category, CommonProps.subcate6),GH_ParamAccess.item)
        {
            m_min = 0.0;
            m_max = 100.0;
            m_secs = new List<CrossSection>();
        }

        public override void CreateAttributes()
        {
            base.m_attributes = new GH_PreviewCrossSectionAttributes(this);
        }

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return Properties.Resources.PreElement;
            }
        }
        public override Guid ComponentGuid
        {
            get { return new Guid("503737E7-A0C7-4EE0-B50F-18E29784577C"); }
        }
        protected override GH_CroSec InstantiateT()
        {
            return new GH_CroSec();
        }

        protected override void CollectVolatileData_FromSources()
        {
            m_secs.Clear();
            VolatileData.Clear();
            if (SourceCount != 0)
            {
                int pathIndex = -1;
                using(IEnumerator<IGH_Param> enumerator = Sources.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        IGH_Param param = enumerator.Current;
                        param.CollectData();　//取得
                        if (param.Phase == GH_SolutionPhase.Collected || param.Phase == GH_SolutionPhase.Computed)
                        {
                            if(param.VolatileData is GH_Structure<GH_CroSec> data)
                            //if (param.GetType().Equals(GetType()))
                            {
                                //GH_Structure<GH_CroSec> data = (GH_Structure<GH_CroSec>)param.VolatileData;
                                base.m_data.AppendRange(data);
                            }
                            else
                            {
                                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, $"Only Input CrossSection Data");
                            }
                        }
                    }
                }
            }
            base.m_data.TrimExcess();
        }

        protected override void OnVolatileDataCollected()
        {
            m_secs.Clear();
            Secs.AddRange(m_data.ToList().ConvertAll(s => s.Value));
            //base.OnVolatileDataCollected();
        }
    }

    public class GH_PreviewCrossSectionAttributes : GH_ResizableAttributes<PTK_PreviewCrossSection>
    {
        private GH_Capsule capsule;
        private List<GraphicsPath> secPaths;
        //private int active;

        public GH_PreviewCrossSectionAttributes(PTK_PreviewCrossSection owner)
        : base(owner)
        {
            secPaths = new List<GraphicsPath>();
            //active = -1;
            Rectangle r = new Rectangle(0, 0, 150, 150);
            Bounds = r;
        }

        public override bool HasOutputGrip => false;
        protected override Size MinimumSize => new Size(50,50);
        protected override Padding SizingBorders => new Padding(6);

        private RectangleF GraphBounds
        {
            get
            {
                RectangleF box = Bounds;
                box.Inflate(-6f, -6f);
                return box;
            }
        }

        protected override void Layout()
        {
            RectangleF rectangleF2 = Bounds = new RectangleF(Pivot, Bounds.Size);
            Bounds = GH_Convert.ToRectangle(Bounds);
        }
        public override void ExpireLayout()
        {
            base.ExpireLayout();
            if (capsule != null)
            {
                capsule.Dispose();
                capsule = null;
            }
            if(secPaths != null)
            {
                foreach(var sec in secPaths)
                {
                    sec?.Dispose();
                }
                secPaths.Clear();
            }
        }
        protected override void PrepareForRender(GH_Canvas canvas)
        {
            if (capsule == null)
            {
                GH_Palette palette = GH_Palette.Hidden;
                if (Owner.RuntimeMessageLevel == GH_RuntimeMessageLevel.Error)
                {
                    palette = GH_Palette.Error;
                }
                capsule = GH_Capsule.CreateCapsule(Bounds, palette, 5, 30);
                capsule.AddInputGrip(InputGrip.Y);
                capsule.SetJaggedEdges(false, true);
            }
            if (secPaths == null)
            {
                secPaths = new List<GraphicsPath>();
            }
            if (secPaths.Count != Owner.Secs.Count)
            {
                secPaths.Clear();
                if (Owner.Secs.Count != 0)
                {
                    int n = Owner.Secs.Count - 1;
                    for (int i = 0; i <= n; i++)
                    {
                        secPaths.Add(GetPath());
                    }
                }
            }
            //base.PrepareForRender(canvas);
        }

        protected override void Render(GH_Canvas canvas, Graphics graphics, GH_CanvasChannel channel)
        {
            switch (channel)
            {
                case GH_CanvasChannel.Wires:
                    RenderIncomingWires(canvas.Painter, Owner.Sources, Owner.WireDisplay);
                    break;
                case GH_CanvasChannel.Objects:
                    {
                        GH_Viewport viewport = canvas.Viewport;
                        RectangleF bounds = Bounds;
                        bool visible = viewport.IsVisible(ref bounds, 10f);
                        Bounds = bounds;
                        if (visible)
                        {
                            capsule.Render(graphics, Selected, Owner.Locked, true);
                            Rectangle rectangle = GH_Convert.ToRectangle(GraphBounds);
                            GH_GraphicsUtil.Grid(graphics, rectangle, 10);
                            if (secPaths.Count > 0)
                            {
                                graphics.SetClip(rectangle);
                                int num = Owner.Secs.Count - 1;
                                for (int i = 0; i <= num; i++)
                                {
                                    DrawPath(graphics, i);
                                }
                                graphics.ResetClip();
                            }
                            GH_GraphicsUtil.ShadowRectangle(graphics, rectangle, 10, 50);
                            graphics.DrawRectangle(Pens.Black, rectangle);
                        }
                        break;
                    }
            }


            //base.Render(canvas, graphics, channel);
        }

        private GraphicsPath GetPath()
        {
            GraphicsPath path = new GraphicsPath();
            path.AddLine(0, 0, 100, 100);
            return path;
        }

        private void DrawPath(Graphics graphics, int index)
        {
            if (index >= 0 && index < secPaths.Count && secPaths[index] != null)
            {
                Pen pen = null;
                if (Owner.Locked)
                {
                    pen = new Pen(Color.FromArgb(150, Color.Black), 3f);
                }
                else
                {
                    double hue = (double)index / (double)Owner.Secs.Count;
                    ColorHSL color = default(ColorHSL);
                    color = new ColorHSL(hue, 1.0, 0.25);
                    pen = new Pen(color.ToArgbColor(), 3f);
                }
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;
                pen.LineJoin = LineJoin.Round;
                graphics.DrawPath(pen, secPaths[index]);
                pen.Dispose();
            }
        }
    }
}
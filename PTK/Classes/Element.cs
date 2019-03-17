using Rhino.Geometry;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace PTK
{
    public abstract class Element
    {
        // --- field ---
        public string Tag { get; private set; } = "N/A";

        // --- constructors --- 
        public Element() { }
        public Element(string _tag)
        {
            Tag = _tag;
        }
    }

    public class Element1D : Element
    {
        // --- field ---
        private Curve baseCurve = null;
        //public Curve BaseCurve { get; private set; } = null;
        public Point3d PointAtStart { get; private set; } = new Point3d();
        public Point3d PointAtEnd { get; private set; } = new Point3d();
        public Plane CroSecLocalPlane { get; private set; }
        public CrossSection CrossSection { get; private set; } = null;
        public Alignment Alignment { get; private set; } = new Alignment("Alignment");
        public Force Forces { get; private set; } = new Force();
        public List<Joint> Joints { get; private set; } = new List<Joint>();
        public bool IsIntersectWithOther { get; private set; } = true;
        public int Priority { get; private set; } = 0;

        public List<Curve> EdgeCurves { get; private set; } = new List<Curve>();

        // --- constructors --- 
        public Element1D() : base()
        {
            InitializeLocalPlane();
        }
        public Element1D(string _tag) : base(_tag)
        {
            InitializeLocalPlane();
        }
        public Element1D(string _tag, Curve _curve, CrossSection _crossSection, Alignment _alignmnet, Force _forces, List<Joint> _joints, int _priority, bool _intersect) : base(_tag)
        {
            BaseCurve = _curve;
            PointAtStart = _curve.PointAtStart;
            PointAtEnd = _curve.PointAtEnd;
            CrossSection = _crossSection;
            Alignment = _alignmnet;
            Forces = _forces;
            Joints = _joints;
            IsIntersectWithOther = _intersect;
            Priority = _priority;
            InitializeLocalPlane();
        }
        public Element1D( Element1D _elem, Force _forces) : base()
        {
            BaseCurve = _elem.baseCurve;
            PointAtStart = _elem.PointAtStart;
            PointAtEnd = _elem.PointAtEnd;
            CrossSection = _elem.CrossSection;
            Alignment = _elem.Alignment;
            Forces = _forces;
            Joints = _elem.Joints;
            IsIntersectWithOther = _elem.IsIntersectWithOther;
            Priority = _elem.Priority;
            InitializeLocalPlane();
        }

        // --- methods ---
        public Curve BaseCurve
        {
            get { return baseCurve.DuplicateCurve(); }
            set { baseCurve = value; }
        }

        private void InitializeLocalPlane()
        {
            if (BaseCurve != null)
            {
                Vector3d localX = BaseCurve.TangentAtStart;
                Vector3d globalZ = Vector3d.ZAxis;

                // determination of local-y direction
                // case A: where local X is parallel to global Z.
                // (such as most of the columns)
                // case B: other than case A. (such as beams or inclined columns)
                // localY direction is obtained by the cross product of globalZ and localX.



                Plane tempPlane = new Plane(BaseCurve.PointAtStart, BaseCurve.TangentAtStart);

                





                Vector3d localY = tempPlane.XAxis;   //case B
                if (localY.Length == 0)
                {
                    localY = Vector3d.YAxis;    //case A
                }

                Vector3d localZ = Vector3d.CrossProduct(localX, localY);
                Plane localYZ = tempPlane;// new Plane(BaseCurve.PointAtStart, localY, localZ);

                //AlongVector
                if (Alignment.AlongVector.Length != 0)
                {
                    if (Alignment.AlongVector.IsParallelTo(localYZ.ZAxis)  ==0)
                    {
                        double rot = Vector3d.VectorAngle(localYZ.YAxis, Alignment.AlongVector, localYZ);
                        localYZ.Rotate(rot, localYZ.ZAxis);
                    }
                    
                }

                // rotation
                if (Alignment.RotationAngle != 0.0)
                {
                    double rot = Alignment.RotationAngle * Math.PI / 180; // degree to radian
                    localYZ.Rotate(rot, localYZ.ZAxis);
                }

                // move origin
                double offsetV = 0.0;
                double offsetU = 0.0;
                double height = CrossSection.GetHeight();
                double width = CrossSection.GetWidth();
                if (Alignment.AnchorVert == AlignmentAnchorVert.Top)
                {
                    offsetV += height / 2;
                }
                else if (Alignment.AnchorVert == AlignmentAnchorVert.Bottom)
                {
                    offsetV -= height / 2;
                }
                if (Alignment.AnchorHori == AlignmentAnchorHori.Right)
                {
                    offsetV += width / 2;
                }
                else if (Alignment.AnchorHori == AlignmentAnchorHori.Left)
                {
                    offsetV -= width / 2;
                }   
                offsetV += Alignment.OffsetZ;
                offsetU += Alignment.OffsetY;
                localYZ.Origin = localYZ.PointAt(offsetU, offsetV);
                
                CroSecLocalPlane = localYZ;

                //CreateEdgeCurves

                Vector3d BaseVector = baseCurve.PointAtEnd - baseCurve.PointAtStart;

                Point3d TRs = CroSecLocalPlane.PointAt(height / 2, width / 2);
                Point3d BRs = CroSecLocalPlane.PointAt(height / -2, width / 2);
                Point3d BLs = CroSecLocalPlane.PointAt(height / -2, width / -2);
                Point3d TLs = CroSecLocalPlane.PointAt(height / 2, width / -2);

                Point3d TRe = TRs;
                TRe.Transform(Transform.Translation(BaseVector));
                Point3d BRe = BRs;
                BRe.Transform(Transform.Translation(BaseVector));
                Point3d BLe = BLs;
                BLe.Transform(Transform.Translation(BaseVector));
                Point3d TLe = TLs;
                TLe.Transform(Transform.Translation(BaseVector));
                
                Curve edgeTR = new LineCurve(TRs, TRe);
                Curve edgeBR = new LineCurve(BRs, BRe);
                Curve edgeBL = new LineCurve(BLs, BLe);
                Curve edgeTL = new LineCurve(TLs, TLe);
                                
                EdgeCurves.Add(edgeTR);
                EdgeCurves.Add(edgeBR);
                EdgeCurves.Add(edgeBL);
                EdgeCurves.Add(edgeTL);
            }
            else
            {
                CroSecLocalPlane = new Plane();
            }



        }

        
        public Element1D DeepCopy()
        {
            return (Element1D)base.MemberwiseClone();
        }
        public override string ToString()
        {
            string info;
            info = "<Element1D>\n" +
                " Tag:" + Tag + "\n" +
                " PointAtStart:" + PointAtStart.ToString() +
                " PointAtEnd:" + PointAtEnd.ToString() + "\n" +
                " CrossSection:" + CrossSection.Name;
            return info;
        }
        public bool IsValid()
        {
            return Tag != "N/A";
        }
    }



    public class GH_Element1D : GH_Goo<Element1D>
    {
        public GH_Element1D() { }
        public GH_Element1D(GH_Element1D other) : base(other.Value) { this.Value = other.Value.DeepCopy(); }
        public GH_Element1D(Element1D ele) : base(ele) { this.Value = ele; }
        public override IGH_Goo Duplicate()
        {
            return new GH_Element1D(this);
        }
        public override bool IsValid => base.m_value.IsValid();
        public override string TypeName => "Element1D";
        public override string TypeDescription => "A linear Element";
        public override string ToString()
        {
            return Value.ToString();
        }
    }

    public class Param_Element1D : GH_PersistentParam<GH_Element1D>
    {
        public Param_Element1D() : base(new GH_InstanceDescription("Element1D", "Elem1D", "A linear Element", CommonProps.category, CommonProps.subcate0)) { }

        protected override System.Drawing.Bitmap Icon { get { return Properties.Resources.ParaElement; } }  //Set icon image

        public override Guid ComponentGuid => new Guid("76479A6F-4C3D-43E0-B85E-FF2C6A99FEA5");

        protected override GH_GetterResult Prompt_Plural(ref List<GH_Element1D> values)
        {
            return GH_GetterResult.success;
        }

        protected override GH_GetterResult Prompt_Singular(ref GH_Element1D value)
        {
            return GH_GetterResult.success;
        }
    }
}


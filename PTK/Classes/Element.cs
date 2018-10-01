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
        public string Tag { get; private set; }
        // --- constructors --- 
        public Element()
        {
            Tag = "N/A";
        }
        public Element(string _tag)
        {
            Tag = _tag;
        }
    }

    public class Element1D : Element
    {
        // --- field ---
        public Curve BaseCurve { get; private set; }
        public Point3d PointAtStart { get; private set; }
        public Point3d PointAtEnd { get; private set; }
        public Plane CroSecLocalPlane { get; private set; }
        public Composite Composite { get; private set; }
        public List<Sub2DElement> Sub2DElements { get; private set; }
        public List<CrossSection> CrossSections { get; private set; }
        public List<MaterialProperty> Materials { get; private set; }
        public Alignment Alignment { get; private set; }
        public List<Force> Forces { get; private set; }
        public List<Joint> Joints { get; private set; }
        public bool IsIntersectWithOther { get; private set; } = true;
        public int Priority { get; private set; } = 0;

        // --- constructors --- 
        public Element1D() : base()
        {
            BaseCurve = null;
            PointAtStart = new Point3d();
            PointAtEnd = new Point3d();
            Composite = new Composite();
            Sub2DElements = new List<Sub2DElement>();
            CrossSections = new List<CrossSection>();
            Materials = new List<MaterialProperty>();
            Alignment = new Alignment();
            Forces = new List<Force>();
            Joints = new List<Joint>();
            Priority = int.MinValue;
            InitializeLocalPlane();
        }
        public Element1D(string _tag) : base(_tag)
        {
            BaseCurve = null;
            PointAtStart = new Point3d();
            PointAtEnd = new Point3d();
            Composite = new Composite();
            Sub2DElements = new List<Sub2DElement>();
            CrossSections = new List<CrossSection>();
            Materials = new List<MaterialProperty>();
            Alignment = new Alignment();
            Forces = new List<Force>();
            Joints = new List<Joint>();
            Priority = int.MinValue;
            InitializeLocalPlane();
        }

        public Element1D(string _tag, Curve _curve, List<Force> _forces, List<Joint> _joints, Composite _composite, int _priority, bool _intersect = true) : base(_tag)
        {
            BaseCurve = _curve;
            PointAtStart = _curve.PointAtStart;
            PointAtEnd = _curve.PointAtEnd;
            Composite = _composite;
            SetSub2DElements();
            CrossSections = new List<CrossSection>();
            Materials = new List<MaterialProperty>();
            SetCrossSections();
            SetMaterial();
            Alignment = _composite.Alignment;
            Forces = _forces;
            Joints = _joints;
            IsIntersectWithOther = _intersect;
            Priority = _priority;
            InitializeLocalPlane();
        }

        // --- methods ---
        private void SetSub2DElements()
        {
            if (Composite.Sub2DElements != null)
            {
                Sub2DElements = Composite.Sub2DElements;
            }
            else
            {
                Sub2DElements = new List<Sub2DElement>();
            }
        }

        private void SetCrossSections()
        {
            if (Composite.Sub2DElements != null)
            {
                foreach (Sub2DElement se in Composite.Sub2DElements)
                    CrossSections.Add(se.CrossSection);
            }
            else
            {
                CrossSections = new List<CrossSection>();
            }
        }

        private void SetMaterial()
        {
            if(Composite.Sub2DElements != null)
            {
                foreach(Sub2DElement se in Composite.Sub2DElements)
                {
                    Materials.Add(se.MaterialProperty);
                }
            }
            else
            {
                Materials = new List<MaterialProperty>();
            }
        }

        private void InitializeLocalPlane()
        {
            if (BaseCurve != null)
            {
                List<CrossSection> crossSections = new List<CrossSection>();
                foreach (Sub2DElement se in Sub2DElements)
                {
                    crossSections.Add(se.CrossSection);
                }
                // List<CrossSection> Sections = SubElement.CrossSections;

                Vector3d localX = BaseCurve.TangentAtStart;
                Vector3d globalZ = Vector3d.ZAxis;

                // determination of local-y direction
                // case A: where local X is parallel to global Z.
                // (such as most of the columns)
                // case B: other than case A. (such as beams or inclined columns)
                // localY direction is obtained by the cross product of globalZ and localX.

                Vector3d localY = Vector3d.CrossProduct(globalZ, localX);   //case B
                if (localY.Length == 0)
                {
                    localY = Vector3d.YAxis;    //case A
                }

                Vector3d localZ = Vector3d.CrossProduct(localX, localY);
                Plane localYZ = new Plane(BaseCurve.PointAtStart, localY, localZ);

                //AlongVector
                if (Alignment.AlongVector.Length != 0)
                {
                    double rot = Vector3d.VectorAngle(localYZ.YAxis, Alignment.AlongVector, localYZ);
                    localYZ.Rotate(rot, localYZ.ZAxis);
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
                CrossSection.GetMaxHeightAndWidth(crossSections, out double height, out double width);
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
            info = "<Element1D> Tag:" + Tag +
                " PointAtStart:" + PointAtStart.ToString() +
                " PointAtEnd:" + PointAtEnd.ToString();
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


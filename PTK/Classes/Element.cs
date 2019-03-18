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
        public CrossSection CrossSection { get; private set; } = null;  //THIS IS GOING OUT!
        public CompositeNew Composite { get; private set; }
        public Force Forces { get; private set; } = new Force();
        public List<Joint> Joints { get; private set; } = new List<Joint>();
        public bool IsIntersectWithOther { get; private set; } = true;
        public int Priority { get; private set; } = 0;
        public ElementAlign Elementalignment { get; private set; }

    // --- constructors --- 
        public Element1D() : base()
        {
            InitializeLocalPlane();
        }
        public Element1D(string _tag) : base(_tag)
        {
            InitializeLocalPlane();
        }

        public Element1D(string _tag, Curve _curve, CompositeInput _compositeInput, ElementAlign _elementalignmnet, Force _forces, List<Joint> _joints, int _priority, bool _intersect) : base(_tag)
        {
            BaseCurve = _curve;
            PointAtStart = _curve.PointAtStart;
            PointAtEnd = _curve.PointAtEnd;
            Elementalignment = _elementalignmnet;
            Forces = _forces;
            Joints = _joints;
            IsIntersectWithOther = _intersect;
            Priority = _priority;
            InitializeLocalPlane();

            Composite = new CompositeNew(_compositeInput, this);

            CrossSection = new RectangleCroSec("", Composite.MaterialProperty, Composite.HeightSimplified, Composite.WidthSimplified, new Alignment());

            
        }

        
        public Element1D( Element1D _elem, Force _forces) : base()
        {
            BaseCurve = _elem.baseCurve;
            PointAtStart = _elem.PointAtStart;
            PointAtEnd = _elem.PointAtEnd;
            Composite = _elem.Composite;
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


                

                Plane InitialPlane = new Plane(BaseCurve.PointAtStart, BaseCurve.TangentAtStart);

                Vector3d Alignmentvector = Elementalignment.ElementAlignmentRule(BaseCurve);
                Vector3d InitialXvector = InitialPlane.YAxis;

                double angle = Vector3d.VectorAngle(InitialXvector, Alignmentvector, InitialPlane);

                Plane BaseCurveYZPlane = new Plane(InitialPlane);

                BaseCurveYZPlane.Rotate(angle, BaseCurveYZPlane.ZAxis, BaseCurveYZPlane.Origin);



                double ElementOffsetY = Elementalignment.OffsetY;
                double ElementOffsetZ = Elementalignment.OffsetZ;

                CroSecLocalPlane = new Plane(BaseCurveYZPlane);

                
                Point3d test = CroSecLocalPlane.PointAt(ElementOffsetY, ElementOffsetZ);
                CroSecLocalPlane = new Plane(test, CroSecLocalPlane.XAxis, CroSecLocalPlane.YAxis);






               


            }
            else
            {
                CroSecLocalPlane = new Plane();
            }
        }

        public Brep GenerateSimplifiedGeometry()
        {
            

            Plane CornerPlane = CroSecLocalPlane;




            Rectangle3d shape = new Rectangle3d(CornerPlane, Composite.WidthInterval, Composite.HeightInterval);

            if (BaseCurve.IsLinear())
            {
                Line line = new Line(BaseCurve.PointAtStart, BaseCurve.PointAtEnd);
                Brep brep = Extrusion.CreateExtrusion(shape.ToNurbsCurve(), line.Direction).ToBrep();
                brep = brep.CapPlanarHoles(CommonProps.tolerances);
                return brep;
            }
            else
            {
                Brep[] sweepreps = Brep.CreateFromSweep(BaseCurve, shape.ToNurbsCurve(), true, CommonProps.tolerances);
                if (sweepreps.Length > 0)
                {
                    return sweepreps[0];
                }
                else
                {
                    return new Brep();
                }

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
                " PointAtEnd:" + PointAtEnd.ToString() + "\n";
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

    public class SubElement
    {
        // --- field ---
        public string Name { get; private set; }
        public int Id { get; private set; }
        public Curve BaseCurve { get; private set; }
        public Point3d PointAtStart { get; private set; } = new Point3d();
        public Point3d PointAtEnd { get; private set; } = new Point3d();
        public Plane CroSecLocalCenterPlane { get; private set; }
        public Plane CrosSecLocalCornerPlane { get; private set; }
        public double Width { get; private set; }
        public double Height { get; private set; }
        public NurbsCurve Shape2d { get; private set; }
        public List<Point3d> Shape2dCorners { get; private set; }
        public Alignment Alignment { get; private set; } = new Alignment("Alignment");
        public MaterialProperty Material { get; private set; }
        public PartType BTLPart { get;  set; }


        // --- constructors --- 


        public SubElement(Element1D MainElement, CrossSection CrossSection)
        {
            Name = CrossSection.Name;

            Alignment = CrossSection.Alignment;
            Material = CrossSection.MaterialProperty;
            Width = CrossSection.GetWidth();
            Height = CrossSection.GetHeight();

            BaseCurve = MainElement.BaseCurve.DuplicateCurve();

            CroSecLocalCenterPlane = GenerateCrossSectionCenterPlanePlane(MainElement.CroSecLocalPlane);
            CrosSecLocalCornerPlane = GenerateCrossSectionCornerPlane(MainElement.CroSecLocalPlane);

            Rectangle3d shape = new Rectangle3d(CrosSecLocalCornerPlane, Width, Height);
            Shape2dCorners = new List<Point3d>();


            Shape2dCorners.Add(CrosSecLocalCornerPlane.PointAt(0, 0));
            Shape2dCorners.Add(CrosSecLocalCornerPlane.PointAt(0, Height));
            Shape2dCorners.Add(CrosSecLocalCornerPlane.PointAt(Width,0));
            Shape2dCorners.Add(CrosSecLocalCornerPlane.PointAt(Width, Height));


            Shape2d = shape.ToNurbsCurve();

        }

        public Plane GenerateCrossSectionCenterPlanePlane(Plane CroSecLocalPlane)
        {
            Plane plane = CroSecLocalPlane;

            plane.Rotate(Alignment.RotationAngle, plane.ZAxis);
            double offsety = Alignment.OffsetY;
            double offsetz = Alignment.OffsetZ;

            plane.Origin = plane.Origin + plane.XAxis * offsety + plane.YAxis * offsetz;
            BaseCurve.Translate(plane.XAxis * offsety + plane.YAxis * offsetz);

            return plane;

        }

        public Plane GenerateCrossSectionCornerPlane(Plane _croSecLocalPlane)
        {
            Plane plane = GenerateCrossSectionCenterPlanePlane(_croSecLocalPlane);
            plane.Origin = plane.Origin - plane.XAxis * Width / 2 - plane.YAxis * Height / 2;

            return plane;

        }




        public Brep GenerateElementGeometry()
        {

            if (BaseCurve.IsLinear())
            {
                Line line = new Line(BaseCurve.PointAtStart, BaseCurve.PointAtEnd);
                Brep brep = Extrusion.CreateExtrusion(Shape2d.ToNurbsCurve(), line.Direction).ToBrep();
                brep = brep.CapPlanarHoles(CommonProps.tolerances);
                return brep;
            }
            else
            {
                Brep[] sweepreps = Brep.CreateFromSweep(BaseCurve, Shape2d.ToNurbsCurve(), true, CommonProps.tolerances);
                if (sweepreps.Length > 0)
                {
                    return sweepreps[0];
                }
                else
                {
                    return new Brep();
                }
            }

        }




    }



}


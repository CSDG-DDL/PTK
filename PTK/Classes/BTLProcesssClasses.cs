using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry;


namespace PTK
{
    public class BTLFunctions       //THis is the BTL element. There can be more than one BTL element in a main element (Element.cl)
    {
        static public Plane AlignInputPlane(Line _refEdge, Plane _refPlane, Plane _cutPlane, out OrientationType orientationtype)
        {


            double lineparameter = 0;

            if (Rhino.Geometry.Intersect.Intersection.LinePlane(_refEdge, _cutPlane, out lineparameter))
            {

            }


            Point3d intersectPoint = _refEdge.PointAt(lineparameter);
            Line intersectionLine = new Line();
            Rhino.Geometry.Intersect.Intersection.PlanePlane(_refPlane, _cutPlane, out intersectionLine);
            _cutPlane.Origin = intersectPoint;

            Line directionLine = FlipLine(_refPlane.YAxis, intersectionLine);




            double angle = Vector3d.VectorAngle(_cutPlane.XAxis, directionLine.Direction, _cutPlane);

            _cutPlane.Rotate(angle, _cutPlane.ZAxis, _cutPlane.Origin);

            orientationtype = OrientationType.start;
            if (Vector3d.VectorAngle(_refPlane.XAxis, _cutPlane.ZAxis) < Math.PI / 2)
            {
                orientationtype = OrientationType.end;
            }




            return _cutPlane;
        }

        public static Vector3d AlignVector (Vector3d _AlignmentVector, Line _line)
        {
            Vector3d AlignVector = new Vector3d();
            if (Vector3d.VectorAngle(_AlignmentVector, _line.Direction) < Math.PI / 2)
            {
                AlignVector = _line.Direction;
            }
            else
            {
                AlignVector = -_line.Direction;
            }

            return AlignVector;


        }


        public static OrientationType GeneratePlaneAngles(Plane RefPlane, Plane Inputplane, out double angle, out double inclination, out double rotation)
        {

            
            Line AngleLine = new Line();
            Line InclinationLine = new Line();
            Plane AnglePlane = Inputplane;
            OrientationType orientationtype; 

            if(Rhino.Geometry.Intersect.Intersection.PlanePlane(RefPlane, AnglePlane, out AngleLine))
            {
                Vector3d AngleVector = AlignVector(RefPlane.YAxis, AngleLine);

                angle = Vector3d.VectorAngle(RefPlane.XAxis, AngleVector, RefPlane);

                Plane Inclinationplane = new Plane(AngleLine.From, AngleVector);

                if (Rhino.Geometry.Intersect.Intersection.PlanePlane(AnglePlane, Inclinationplane, out InclinationLine));
                
                Vector3d InclinationVector = AlignVector(-RefPlane.ZAxis, InclinationLine);

                inclination = Vector3d.VectorAngle(RefPlane.XAxis, InclinationVector, Inclinationplane);

                rotation = Vector3d.VectorAngle(AnglePlane.XAxis, -AngleVector, AnglePlane);

                if (Vector3d.VectorAngle(RefPlane.XAxis, AnglePlane.ZAxis) < Math.PI / 2)
                {
                    orientationtype = OrientationType.end;
                    angle = Math.PI - angle;
                    inclination = Math.PI - angle;
                    return orientationtype;

                }
                   
                else
                {
                    orientationtype = OrientationType.start;
                    return orientationtype;
                }


            }
            else
            {
                angle = inclination = rotation = 0;
                return OrientationType.start;
            }



        }

        

        public static Line FlipLine(Vector3d _guide, Line _line)
        {
            List<double> angle = new List<double>();
            List<Line> intersectionlines = new List<Line>();
            Line flipline = _line;
            flipline.Flip();
            angle.Add(Vector3d.VectorAngle(_guide, _line.Direction));
            intersectionlines.Add(_line);
            angle.Add(Vector3d.VectorAngle(_guide, flipline.Direction));
            intersectionlines.Add(flipline);

            //Returning the line with the smallest angle: Aligning the line to face the same direction as the vector
            if (angle[0] < angle[1])
            {
                return intersectionlines[0];
            }
            else
            {
                return intersectionlines[1];
            }



        }
        public static List<Point3d> GetValidVoidPoints(Plane _CutPlane, List<Point3d> TestPoints)
        {


            List<Point3d> Voidpoints = new List<Point3d>();

            foreach (Point3d point in TestPoints)
            {
                Point3d localaxispoint;
                _CutPlane.RemapToPlaneSpace(point, out localaxispoint);
                if (localaxispoint.Z > -.00001)
                {
                    Voidpoints.Add(point);
                }

            }
            return Voidpoints;


        }

        public static List<Point3d> GetCutPoints(Plane _CutPlane, List<Refside> _refsides)
        {

            List<Point3d> CutPoints = new List<Point3d>();
            foreach (Refside side in _refsides)
            {
                double tempDouble = 0;
                if (Rhino.Geometry.Intersect.Intersection.LinePlane(side.RefEdge, _CutPlane, out tempDouble)) ;
                if (0 < tempDouble && tempDouble < side.RefEdge.Length)
                {
                    CutPoints.Add(side.RefEdge.PointAt(tempDouble));
                }
            }
            return CutPoints;





        }

        public static Refside GetRefSideFromPlane(List<Refside> _refSides, Plane _CutPlane, out List<Point3d> _CutPoints)
        {
            int i = 0;
            double smallestDistance = 9999999;
            int sideIndex = 0;

            List<Point3d> CutPoints = new List<Point3d>();
            foreach (Refside side in _refSides)
            {

                double tempDouble = 0;
                if (Rhino.Geometry.Intersect.Intersection.LinePlane(side.RefEdge, _CutPlane, out tempDouble)) ;
                if (0 < tempDouble && tempDouble < side.RefEdge.Length)
                {

                    Point3d cutPoint = side.RefEdge.PointAt(tempDouble);
                    double distance = side.RefPoint.DistanceTo(cutPoint);
                    CutPoints.Add(cutPoint);


                    if (distance < smallestDistance)
                    {

                        smallestDistance = distance;
                        sideIndex = i;
                    }

                }
                i++;
            }

            _CutPoints = CutPoints;
            return _refSides[sideIndex];



            //Checking each intersection, determine which is smallest, return plane, line and origo





        }


    }


    public class Refside
    {
        // --- field ---
        public uint RefSideID { get; private set; }
        public Plane RefPlane { get; private set; }
        public Line RefEdge { get; private set; }
        public Point3d RefPoint { get; private set; }
        public double RefSideXLength { get; private set; }
        public double RefSideYLength { get; private set; }
        public double RefSideZLength { get; private set; }

        // --- constructors --- 
        public Refside(uint _refsideID, Plane _refPlane, double _refSideXLength, double _refSideYLength, double _refSideZLength)
        {
            RefSideID = _refsideID;
            RefPlane = _refPlane;
            RefEdge = new Line(RefPlane.Origin, RefPlane.XAxis, _refSideXLength);
            RefPoint = RefPlane.Origin;
            RefSideXLength = _refSideXLength;
            RefSideYLength = _refSideYLength;
            RefSideZLength = _refSideZLength;
        }
    }


    public class BTLDrill
    {
        public Line DrillLine { get; private set; }
        public double Radius { get; private set; }

        public BTLDrill (Line _drillLine, double _radius)
        {
            DrillLine = _drillLine;
            Radius = _radius;
        }
        // --- methods ---
        public PerformedProcess DelegateProcess(BTLPartGeometry _BTLPartGeometry, ManufactureMode _mode)
        {


            List<Refside> Refsides = _BTLPartGeometry.Refsides;
            List<Point3d> CornerPoints = _BTLPartGeometry.CornerPoints;
            List<Point3d> EndPoints = _BTLPartGeometry.Endpoints;
            List<Point3d> StartPoints = _BTLPartGeometry.StartPoints;




            //Finding the best refsides


            Point3d intersectPoint = new Point3d();
            Refside Refside = null;
            Point3d localPlaneInsertPoint = new Point3d();
            Point3d localPlaneIntersectPoint = new Point3d();

            foreach (Refside side in Refsides)
            {
                double param =0;
                if (Rhino.Geometry.Intersect.Intersection.LinePlane(DrillLine, side.RefPlane, out param)==true)
                {
                    if (0 <= param && param <= 1)
                    {
                        intersectPoint = DrillLine.PointAt(param);


                        side.RefPlane.RemapToPlaneSpace(intersectPoint, out localPlaneIntersectPoint);
                        if (localPlaneIntersectPoint.X < side.RefSideXLength && localPlaneIntersectPoint.Y < side.RefSideYLength)
                        {
                            Refside = side;
                            break;
                        }
                    }
                }
                
                
            }

            if (Refside == null)
            {
                return null;
            }


            //Flipping line if incorrect direction. 
            Point3d LineStartPoint = DrillLine.From;
            Point3d LocalLineStartPoint = new Point3d();
            Refside.RefPlane.RemapToPlaneSpace(intersectPoint, out localPlaneInsertPoint);
            Refside.RefPlane.RemapToPlaneSpace(LineStartPoint, out LocalLineStartPoint);
            Line AlignedDrillLine = DrillLine;
            if (LocalLineStartPoint.Z < 0)
            {
                AlignedDrillLine = new Line(DrillLine.To, DrillLine.From);
            }

            //Get endpoint in localAxis

            Point3d localPlaneEndPoint = new Point3d();
            Refside.RefPlane.RemapToPlaneSpace(AlignedDrillLine.To, out localPlaneEndPoint);
            Point3d localpPlaneProjectedEndPoint = new Point3d(localPlaneEndPoint);
            localpPlaneProjectedEndPoint.Z = 0;

            DrillingType Drill = new DrillingType();
            Drill.StartX = localPlaneInsertPoint.X;
            Drill.StartY = localPlaneInsertPoint.Y;


            if (Math.Abs(localPlaneEndPoint.Z) < Refside.RefSideZLength)
            {
                Drill.Depth = -localPlaneEndPoint.Z;
                Drill.DepthLimited = BooleanType.yes;
            }
            else
            {
                Drill.DepthLimited = BooleanType.no;
                
            }

            

            Drill.Diameter = Radius * 2;


            Point3d LocaldirectionPoint = new Point3d(localPlaneInsertPoint);
            LocaldirectionPoint.X = LocaldirectionPoint.X - 10;


            Line ProjectedLine = new Line(localPlaneInsertPoint, localpPlaneProjectedEndPoint);
            Line drillAngleLine = new Line(localPlaneInsertPoint, localPlaneEndPoint);
            Line DirectionLine = new Line(localPlaneInsertPoint, LocaldirectionPoint);

            if (ProjectedLine.Length < CommonProps.tolerances)
            {
                Drill.Angle = 0;
                Drill.Inclination = 90;
            }
            else
            {
                Drill.Angle = Rhino.RhinoMath.ToDegrees( Vector3d.VectorAngle(DirectionLine.Direction, ProjectedLine.Direction ,new Plane(Refside.RefPlane.Origin,- Refside.RefPlane.XAxis, -Refside.RefPlane.YAxis )));
                Drill.Inclination = Rhino.RhinoMath.ToDegrees(Vector3d.VectorAngle(drillAngleLine.Direction, ProjectedLine.Direction));
            }

            

            Drill.ReferencePlaneID = Refside.RefSideID;
            Drill.Name = "Drill";
            

            //Making NURBS GEOMETRY
            Circle Circle = new Circle(new Plane(DrillLine.From, DrillLine.Direction), Radius);


            Cylinder Cylinder = new Cylinder(Circle, DrillLine.Length);

            return new PerformedProcess(Drill, Brep.CreateFromCylinder(Cylinder, true, true));
        }


    }






    public class BTLCut
    {
        // --- field ---
        public Plane CutPlane { get; private set; }

        // --- constructors --- 
        public BTLCut(Plane _cutPlane)
        {
            CutPlane = _cutPlane;
        }

        // --- methods ---
        public PerformedProcess DelegateProcess(BTLPartGeometry _BTLPartGeometry, ManufactureMode _mode)
        {
            List<Point3d> cutpoints = new List<Point3d>();

            List<Refside> Refsides = _BTLPartGeometry.Refsides;
            List<Point3d> CornerPoints = _BTLPartGeometry.CornerPoints;
            List<Point3d> EndPoints = _BTLPartGeometry.Endpoints;
            List<Point3d> StartPoints = _BTLPartGeometry.StartPoints;

            //Calculating the refside that has the cutpoint closest to the refpoint
            Refside RefSide = Refsides[0];// BTLFunctions.GetRefSideFromPlane(Refsides, CutPlane, out cutpoints);


            //Assigning variables based on chosen refplane
            uint RefSideId = RefSide.RefSideID;
            Plane RefPlane = RefSide.RefPlane;
            Line RefEdge = RefSide.RefEdge;
            Point3d RefPoint = RefSide.RefPoint;

            Point3d intersectPoint = new Point3d();

            double lineparameter = 0;

            if (Rhino.Geometry.Intersect.Intersection.LinePlane(RefEdge, CutPlane, out lineparameter))
                intersectPoint = RefEdge.PointAt(lineparameter);




            //intersectPoint = intersectionevent.PointA;
            Line directionLine = new Line();
            if (Rhino.Geometry.Intersect.Intersection.PlanePlane(RefPlane, CutPlane, out directionLine)) ;


            OrientationType orientation;


            //AlignInputPlane aligns the x-axis of the plane to the surface of the ref-side
            CutPlane = BTLFunctions.AlignInputPlane(RefEdge, RefPlane, CutPlane, out orientation);


            Plane inclinationplane = new Plane(CutPlane.Origin, CutPlane.YAxis, CutPlane.ZAxis);





            Vector3d RefVector = RefEdge.Direction;
            Vector3d Cutvector = CutPlane.YAxis;

            List<Point3d> voidpoints = new List<Point3d>();
            voidpoints.AddRange(BTLFunctions.GetCutPoints(CutPlane, Refsides));  //adding the four points where the refEdge and the cutplane intersects
            voidpoints.AddRange(CornerPoints);


            if (orientation == OrientationType.end)
            {

                RefVector.Reverse();
                Cutvector.Reverse(); //Correct
            }


            voidpoints = BTLFunctions.GetValidVoidPoints(CutPlane, voidpoints);



            //Calculating negative distance by remaping to planespace
            double startX = 0;
            Point3d localaxispoint;
            Plane checkplane = new Plane(RefEdge.From, RefEdge.Direction);
            checkplane.RemapToPlaneSpace(intersectPoint, out localaxispoint);


            //Creating voidbox
            Box box = new Box(CutPlane, voidpoints);


            //Creating BTL processing
            JackRafterCutType JackRafterCut = new JackRafterCutType();
            JackRafterCut.Orientation = orientation;
            JackRafterCut.ReferencePlaneID = RefSideId;
            JackRafterCut.Process = BooleanType.yes;
            JackRafterCut.StartX = localaxispoint.Z;
            JackRafterCut.StartY = 0.0;
            JackRafterCut.StartDepth = 0.0;
            JackRafterCut.Angle = Vector3d.VectorAngle(RefVector, CutPlane.XAxis);
            JackRafterCut.Angle = Convert.ToDouble(Rhino.RhinoMath.ToDegrees(JackRafterCut.Angle));
            JackRafterCut.Inclination = Vector3d.VectorAngle(RefVector, Cutvector, new Plane(directionLine.From, directionLine.Direction));
            JackRafterCut.Inclination = Convert.ToDouble(Rhino.RhinoMath.ToDegrees(JackRafterCut.Inclination));
            JackRafterCut.StartDepth = 0.0;
            JackRafterCut.Name= "Cut";


            return new PerformedProcess(JackRafterCut, Brep.CreateFromBox(box));

        }



    }

    public enum TenonMode
    {
        PlaneMode,RectangleMode
    }

    



    public class BTLTenon
    {
        // --- field ---
        public TenonMode TenonMode { get; private set; }
        public Plane TenonPlane { get; private set; } //Xaxis is the lengthDirection of the tenon
        public double Width { get; private set; }
        public double Length { get; private set; }
        public double Height { get; private set; }
        public BooleanType LengthLimitedTop { get; private set; }
        public BooleanType LengthLimitedBottom { get; private set; }
        public BooleanType Chamfer { get; private set; }
        public double ShapeRadius { get; private set; }
        public TenonShapeType Shapetype { get; private set; }


        public BTLTenon(Plane _tenonPlane, double _width, double _length, double _height, BooleanType _lengthLimitedTop, BooleanType _lengthLimitedBottom, BooleanType _chamfer, int _shapetype, double _radius)
        {
            TenonPlane = _tenonPlane;
            Width = _width;
            Height = _height;
            Length = _length;
            LengthLimitedTop = _lengthLimitedTop;
            LengthLimitedBottom = _lengthLimitedBottom;
            Chamfer = _chamfer;
            ShapeRadius = _radius;

            TenonMode = TenonMode.PlaneMode;
            if(_shapetype < 0 || _shapetype > 4)
            {
                _shapetype = 0;
            }
            if (_shapetype == 0) { Shapetype = TenonShapeType.automatic; }
            if (_shapetype == 1) { Shapetype = TenonShapeType.square; }
            if (_shapetype == 2) { Shapetype = TenonShapeType.round; }
            if (_shapetype == 3) { Shapetype = TenonShapeType.rounded; }
            if (_shapetype == 4) { Shapetype = TenonShapeType.radius; }


            

        }
        




        public PerformedProcess DelegateProcess(BTLPartGeometry _BTLPartGeometry, ManufactureMode _mode)
        {



            //Calculating tenontplane, width/ height depth here if needed

            //Finding the refside's negative z-axis that has a has the smallest angle compared to the x-axis of the tenon-plane
            List<Refside> Refsides = _BTLPartGeometry.Refsides;


            Refside Refside = Refsides[0];
            Vector3d TenonLengthVector = TenonPlane.XAxis;
            Vector3d RefsideAngle = new Vector3d();
            double smallestAngle = 1000;
            foreach (Refside side in Refsides)  //Cycling through refsides and choosing the one with smallest angle
            {
                RefsideAngle = -side.RefPlane.ZAxis;

                double angle = Vector3d.VectorAngle(TenonLengthVector, RefsideAngle);
                if (angle < smallestAngle)
                {
                    smallestAngle = angle;
                    Refside = side;
                }
            }



            Plane RefPlane = Refside.RefPlane;
            Point3d LocalStartPoint = new Point3d();
            RefPlane.RemapToPlaneSpace(TenonPlane.Origin, out LocalStartPoint);


            double Angle;
            double Inclination;
            double Rotation;

            BTLFunctions.GeneratePlaneAngles(RefPlane, TenonPlane, out Angle, out Inclination, out Rotation);


            TenonType Tenon = new TenonType();
            Tenon.Name = "Tenon";
            Tenon.StartX = LocalStartPoint.X;
            Tenon.StartY = LocalStartPoint.Y;
            Tenon.StartDepth = Math.Abs(LocalStartPoint.Z);
            Tenon.Orientation = OrientationType.start;
            Tenon.LengthLimitedBottom = LengthLimitedBottom;
            Tenon.LengthLimitedTop = LengthLimitedTop;
            Tenon.Length = Length;
            Tenon.Width = Width;
            Tenon.Height = Height;
            Tenon.Shape = Shapetype;
            Tenon.ShapeRadius = ShapeRadius;
            Tenon.Chamfer = Chamfer;
            Tenon.Angle = Rhino.RhinoMath.ToDegrees(Angle);
            Tenon.Inclination = Rhino.RhinoMath.ToDegrees(Inclination);
            Tenon.Rotation = Rhino.RhinoMath.ToDegrees(Rotation);
            Tenon.ReferencePlaneID = Refside.RefSideID;


            Interval y = new Interval(-Width / 2, Width / 2);
            Interval x = new Interval(0, Length);
            Rectangle3d tenonRectangle = new Rectangle3d(TenonPlane, x, y);

            List<Point3d> voidpoints = new List<Point3d>();
            voidpoints.AddRange(BTLFunctions.GetCutPoints(TenonPlane, Refsides));  //adding the four points where the refEdge and the cutplane intersects
            voidpoints.AddRange(_BTLPartGeometry.CornerPoints);
            voidpoints = BTLFunctions.GetValidVoidPoints(TenonPlane, voidpoints);

            Box box = new Box(TenonPlane, voidpoints);

            Brep Boxa = Brep.CreateFromBox(box);
            
            Extrusion TenonShape = Extrusion.Create(tenonRectangle.ToNurbsCurve(), Height, true);
            

            double tolerance = CommonProps.tolerances;
            Rhino.Geometry.Brep[] breps = Rhino.Geometry.Brep.CreateBooleanDifference(Boxa, TenonShape.ToBrep(), tolerance);
            



            return new PerformedProcess(Tenon, breps[0] );






        }



}

        
    

    



        
    


}












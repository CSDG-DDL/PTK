﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry;


namespace PTK
{

    public class PlaneArrow
    {
        public Line PosLine { get; private set; }
        public Line NegLine { get; private set; }
        public Surface SurfacePlane { get; private set; }

        public PlaneArrow (Plane _plane, double _size)
        {
            Interval Domain = new Interval(-_size *3, _size * 3);
            SurfacePlane = new PlaneSurface(_plane, Domain, Domain);
            
            NegLine = new Line(SurfacePlane.PointAt(-_size * 3, -_size * 3), _plane.ZAxis, _size * 3);
            PosLine = new Line(SurfacePlane.PointAt(-_size * 3, -_size * 3), -_plane.ZAxis, _size * 1);
        }

    }

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

        public static OrientationType GeneratePlaneAnglesPerp(Plane RefPlane, Plane Inputplane, out double angle, out double inclination, out double rotation)
        {
            Line RefPlaneCutLine = new Line();
            Vector3d RefPlaneCutVector = new Vector3d();

            if (Rhino.Geometry.Intersect.Intersection.PlanePlane(RefPlane, Inputplane, out RefPlaneCutLine))
            {
                RefPlaneCutVector = RefPlaneCutLine.Direction;  //The intersectionline between the refplane and the inputplane

                double RefXCutPlaneAngle = Vector3d.VectorAngle(RefPlane.YAxis, RefPlaneCutVector);

                if (RefXCutPlaneAngle > Math.PI/2)    //Aligning the refplane line with the y-vector of the refplane
                {
                    RefPlaneCutVector = -RefPlaneCutVector;
                }


                OrientationType Type;
                
                Plane NormalCutLinePlane = new Plane(RefPlane.Origin, -RefPlaneCutVector);   //A plane with normalplane paralell to cutline. Facing towards refedge (oposite of refplane.yaxis)


                double NormalAngle = Vector3d.VectorAngle(RefPlane.XAxis, -Inputplane.ZAxis);

                if (NormalAngle<= Math.PI/2)
                {
                    
                    angle = Vector3d.VectorAngle(RefPlane.XAxis, RefPlaneCutVector, RefPlane);
                    inclination = Vector3d.VectorAngle(Inputplane.XAxis, RefPlane.XAxis, NormalCutLinePlane);
                    rotation = Vector3d.VectorAngle(Inputplane.XAxis, -RefPlaneCutVector);
                    return OrientationType.start;

                }
                else
                {
                    
                    angle = Vector3d.VectorAngle(RefPlaneCutVector, -RefPlane.XAxis, RefPlane);
                    inclination = Vector3d.VectorAngle(-RefPlane.XAxis, Inputplane.XAxis, NormalCutLinePlane);
                    rotation = Vector3d.VectorAngle(-RefPlaneCutVector, Inputplane.XAxis);
                    return OrientationType.end;
                }
                

                


            }
            angle = 0;
            inclination = 0;
            rotation = 0;

            return OrientationType.error;


            

            






        }

        


        


        public static void GeneratePlaneAnglesParallell(BTLPartGeometry _BTLPartGeometry, double Length, Plane WorkPlane, bool FlipDirection, out double Angle, out double Inclination, out double Slope, out Point3d LocalStartPoint, out Refside Refside, out Plane UpdatedWorkPlane)  //used when when plane is closed to parallell to refplane
        {
            //Flipping plane
            if (FlipDirection)
            {
                Plane tempplane = new Plane(WorkPlane.Origin, WorkPlane.XAxis, -WorkPlane.YAxis);
                WorkPlane = tempplane;
            }

            UpdatedWorkPlane = WorkPlane; 


            List<Refside> Refsides = _BTLPartGeometry.Refsides;





            //Choosing reference plane

            double SmallestAngle = 1000;
            Refside = Refsides[0];

            foreach (Refside Side in Refsides)
            {
                double side = Side.RefSideID;

                double angle = Vector3d.VectorAngle(Side.RefPlane.ZAxis, WorkPlane.ZAxis);
                if (angle <= SmallestAngle)
                {
                    SmallestAngle = angle;
                    Refside = Side;
                }

            }



            //Initializing correct plane
            Plane RefPlane = Refside.RefPlane;


            if (Vector3d.VectorAngle(RefPlane.XAxis, WorkPlane.XAxis, RefPlane) > Math.PI)
            {

                Plane tempplane = new Plane(WorkPlane);
                tempplane.XAxis.Unitize();
                tempplane.Translate(tempplane.XAxis * Length);
                tempplane.Rotate(Math.PI, tempplane.ZAxis, tempplane.Origin);
                WorkPlane = tempplane;
            }



            Angle = Vector3d.VectorAngle(RefPlane.XAxis, WorkPlane.XAxis, RefPlane);




            Plane RotationAlignedRefPlane = new Plane(RefPlane);


            //Moving and orienting the RotationAligned Plane. Still parallell to refplane, but aligned with X-axis of the Workplane.
            RotationAlignedRefPlane.Rotate(Angle, RotationAlignedRefPlane.ZAxis);
            RotationAlignedRefPlane.Origin = WorkPlane.Origin;

            //A new plane used for slopecalculations
            Plane SlopePlane = new Plane(RotationAlignedRefPlane.Origin, RotationAlignedRefPlane.XAxis, RotationAlignedRefPlane.ZAxis);



            Slope = Vector3d.VectorAngle(RotationAlignedRefPlane.XAxis, WorkPlane.ZAxis, SlopePlane);




            Plane InclinationPlane = new Plane(WorkPlane.Origin, WorkPlane.ZAxis, WorkPlane.YAxis);


            Inclination = Vector3d.VectorAngle(RotationAlignedRefPlane.YAxis, -WorkPlane.ZAxis, InclinationPlane);

            LocalStartPoint = new Point3d();

            RefPlane.RemapToPlaneSpace(WorkPlane.Origin, out LocalStartPoint);


        }
        public static Brep GenerateTenonShape(Plane _localPlane, double _height, Interval _BottomWidth, Interval _TopWidth, Interval _length, TenonShapeType Type, double Radius)
        {
            Point3d CenterTopPt = _localPlane.Origin;
            _localPlane.RemapToPlaneSpace(_localPlane.Origin, out CenterTopPt);
            Point3d LeftTopPt = new Point3d(CenterTopPt);
            Point3d RightTopPt = new Point3d(CenterTopPt);
            Point3d LeftBtmPt = new Point3d(CenterTopPt);
            Point3d RightBtmPt = new Point3d(CenterTopPt);

            LeftTopPt.X = RightTopPt.X = _length.T0;
            LeftBtmPt.X = RightBtmPt.X = _length.T1;

            LeftTopPt.Y = _TopWidth.T0;
            RightTopPt.Y = _TopWidth.T1;
            LeftBtmPt.Y = _BottomWidth.T0;
            RightBtmPt.Y = _BottomWidth.T1;

            List<double> widhts = new List<double>();
            widhts.Add(_TopWidth.T1);
            widhts.Add(_BottomWidth.T1);
            widhts.Sort();

            if (Type == TenonShapeType.round || Radius >= widhts[0])
            {

                Radius = widhts[0] - .01;
            }





            Interval NormalizedInterval = new Interval(0, 1);
            Curve Topline = new Line(LeftTopPt, RightTopPt).ToNurbsCurve();
            Curve BtmLine = new Line(LeftBtmPt, RightBtmPt).ToNurbsCurve();
            Curve LeftLine = new Line(LeftBtmPt, LeftTopPt).ToNurbsCurve();
            Curve RightLine = new Line(RightBtmPt, RightTopPt).ToNurbsCurve();

            List<Curve> AllCurves = new List<Curve>();

            if (Type == TenonShapeType.square)
            {
                AllCurves.Add(Topline);
                AllCurves.Add(BtmLine);
                AllCurves.Add(LeftLine);
                AllCurves.Add(RightLine);
            }



            else
            {
                Topline.Domain = BtmLine.Domain = LeftLine.Domain = RightLine.Domain = NormalizedInterval;

                Curve[] Toplines = Topline.Split(0.5);
                Curve[] BtmLines = BtmLine.Split(0.5);
                Curve[] LeftLines = LeftLine.Split(0.5);
                Curve[] RightLines = RightLine.Split(0.5);
                Toplines[0].Reverse();
                RightLines[0].Reverse();
                BtmLines[0].Reverse();
                LeftLines[0].Reverse();

                Curve[] TopRight = Curve.CreateFilletCurves(Toplines[1], RightTopPt, RightLines[1], RightTopPt, Radius, true, true, false, CommonProps.tolerances, CommonProps.tolerances);
                Curve[] TopLeft = Curve.CreateFilletCurves(Toplines[0], Toplines[0].PointAtStart, LeftLines[1], LeftLines[1].PointAtEnd, Radius, true, true, false, CommonProps.tolerances, CommonProps.tolerances);
                Curve[] BtmRight = Curve.CreateFilletCurves(BtmLines[1], RightBtmPt, RightLines[0], RightBtmPt, Radius, true, true, false, CommonProps.tolerances, CommonProps.tolerances);
                Curve[] BtmLeft = Curve.CreateFilletCurves(BtmLines[0], LeftBtmPt, LeftLines[0], LeftBtmPt, Radius, true, true, false, CommonProps.tolerances, CommonProps.tolerances);



                AllCurves.Add(TopRight[0]);
                AllCurves.Add(TopLeft[0]);
                AllCurves.Add(BtmRight[0]);
                AllCurves.Add(BtmLeft[0]);
            }






            Curve[] JoinedCurve = Curve.JoinCurves(AllCurves);
            if (JoinedCurve[0].IsClosed)
            {
                Extrusion Tenonshape = Extrusion.Create(JoinedCurve[0], -_height, true);

                Tenonshape.Transform(Transform.PlaneToPlane(Plane.WorldXY, _localPlane));
                return Tenonshape.ToBrep();
            }
            else
            {
                return new Brep();
            }





        }

        public static Brep GenerateMortiseShape(Plane _localPlane, Interval _heightinterval, Interval _BottomWidth, Interval _TopWidth, Interval _length, TenonShapeType Type, double Radius, double _flankAngle)
        {
            _localPlane.Translate(_localPlane.ZAxis * _heightinterval.T0);
            Point3d CenterTopPt = _localPlane.Origin;
            _localPlane.RemapToPlaneSpace(_localPlane.Origin, out CenterTopPt);
            Point3d LeftTopPt = new Point3d(CenterTopPt);
            Point3d RightTopPt = new Point3d(CenterTopPt);
            Point3d LeftBtmPt = new Point3d(CenterTopPt);
            Point3d RightBtmPt = new Point3d(CenterTopPt);




            LeftTopPt.X = RightTopPt.X = _length.T0;
            LeftBtmPt.X = RightBtmPt.X = _length.T1;

            LeftTopPt.Y = _TopWidth.T0;
            RightTopPt.Y = _TopWidth.T1;
            LeftBtmPt.Y = _BottomWidth.T0;
            RightBtmPt.Y = _BottomWidth.T1;

            List<double> widhts = new List<double>();
            widhts.Add(_TopWidth.T1);
            widhts.Add(_BottomWidth.T1);
            widhts.Sort();

            if (Type == TenonShapeType.round || Radius >= widhts[0])
            {

                Radius = widhts[0] - .01;
            }





            Interval NormalizedInterval = new Interval(0, 1);
            Curve Topline = new Line(LeftTopPt, RightTopPt).ToNurbsCurve();
            Curve BtmLine = new Line(LeftBtmPt, RightBtmPt).ToNurbsCurve();
            Curve LeftLine = new Line(LeftBtmPt, LeftTopPt).ToNurbsCurve();
            Curve RightLine = new Line(RightBtmPt, RightTopPt).ToNurbsCurve();

            List<Curve> AllCurves = new List<Curve>();

            if (Type == TenonShapeType.square)
            {
                AllCurves.Add(Topline);
                AllCurves.Add(BtmLine);
                AllCurves.Add(LeftLine);
                AllCurves.Add(RightLine);
            }



            else
            {
                Topline.Domain = BtmLine.Domain = LeftLine.Domain = RightLine.Domain = NormalizedInterval;

                Curve[] Toplines = Topline.Split(0.5);
                Curve[] BtmLines = BtmLine.Split(0.5);
                Curve[] LeftLines = LeftLine.Split(0.5);
                Curve[] RightLines = RightLine.Split(0.5);
                Toplines[0].Reverse();
                RightLines[0].Reverse();
                BtmLines[0].Reverse();
                LeftLines[0].Reverse();

                Curve[] TopRight = Curve.CreateFilletCurves(Toplines[1], RightTopPt, RightLines[1], RightTopPt, Radius, true, true, false, CommonProps.tolerances, CommonProps.tolerances);
                Curve[] TopLeft = Curve.CreateFilletCurves(Toplines[0], Toplines[0].PointAtStart, LeftLines[1], LeftLines[1].PointAtEnd, Radius, true, true, false, CommonProps.tolerances, CommonProps.tolerances);
                Curve[] BtmRight = Curve.CreateFilletCurves(BtmLines[1], RightBtmPt, RightLines[0], RightBtmPt, Radius, true, true, false, CommonProps.tolerances, CommonProps.tolerances);
                Curve[] BtmLeft = Curve.CreateFilletCurves(BtmLines[0], LeftBtmPt, LeftLines[0], LeftBtmPt, Radius, true, true, false, CommonProps.tolerances, CommonProps.tolerances);



                AllCurves.Add(TopRight[0]);
                AllCurves.Add(TopLeft[0]);
                AllCurves.Add(BtmRight[0]);
                AllCurves.Add(BtmLeft[0]);
            }






            Curve[] JoinedCurve = Curve.JoinCurves(AllCurves);
            if (JoinedCurve[0].IsClosed)
            {
                Extrusion Tenonshape;
                if (_flankAngle < CommonProps.tolerances)
                {
                   Tenonshape  = Extrusion.Create(JoinedCurve[0], -_heightinterval.T1, true);
                   Tenonshape.Transform(Transform.PlaneToPlane(Plane.WorldXY, _localPlane));
                   return Tenonshape.ToBrep();
                }
                else
                {
                    double height = _heightinterval.T0;

                    if (_flankAngle < Rhino.RhinoMath.ToRadians(5))
                    {
                        _flankAngle = Rhino.RhinoMath.ToRadians(5);
                    }
                    if (_flankAngle > Rhino.RhinoMath.ToRadians(35))
                    {
                        _flankAngle = Rhino.RhinoMath.ToRadians(35);
                    }

                    double offsetValue = Math.Tan(_flankAngle) * height;
                    Curve[] OffsetCurve = JoinedCurve[0].Offset(Plane.WorldXY, offsetValue, CommonProps.tolerances, CurveOffsetCornerStyle.Round);

                    JoinedCurve[0].Translate(Vector3d.ZAxis * -height);
                    List<Curve> LoftCurves = new List<Curve>();
                    LoftCurves.Add(JoinedCurve[0]);
                    LoftCurves.Add(OffsetCurve[0]);

                    Brep[] temp = Brep.CreateFromLoft(LoftCurves, Point3d.Unset, Point3d.Unset, LoftType.Normal,false);
                    List<Brep> Breps = new List<Brep>();
                    Breps.Add(temp[0]);
                    Breps.Add(Brep.CreatePlanarBreps(JoinedCurve[0])[0]);
                    Breps.Add(Brep.CreatePlanarBreps(OffsetCurve[0])[0]);

                    Brep[] Closed = Brep.CreateSolid(Breps, CommonProps.tolerances);


                    Closed[0].Transform(Transform.PlaneToPlane(Plane.WorldXY, _localPlane));
                    return Closed[0];
                }
                
                


                
            }
            else
            {
                return new Brep();
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
        public bool FlipDirection { get; private set; }


        public BTLTenon(Plane _tenonPlane, double _width, double _length, double _height, BooleanType _lengthLimitedTop, BooleanType _lengthLimitedBottom, BooleanType _chamfer, int _shapetype, double _radius, bool _flipDirection)
        {
            FlipDirection = _flipDirection;
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
            if (FlipDirection)
            {
                Plane tempplane = new Plane(TenonPlane.Origin, TenonPlane.XAxis, -TenonPlane.YAxis);
                TenonPlane = tempplane;
            }



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

            OrientationType type = BTLFunctions.GeneratePlaneAnglesPerp(RefPlane, TenonPlane, out Angle, out Inclination, out Rotation);


            TenonType Tenon = new TenonType();
            Tenon.Name = "Tenon";
            Tenon.StartX = LocalStartPoint.X;
            Tenon.StartY = LocalStartPoint.Y;
            Tenon.StartDepth = Math.Abs(LocalStartPoint.Z);
            Tenon.Orientation = type;
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

            //Making intervals for tenonshape
            Interval Topwidth = new Interval(-Width / 2, Width / 2);
            Interval BtmWidth = Topwidth; //Not relevant for tenon, but simplifies the code
            Interval LengthInterval = new Interval(0, Length);


            //Generating points for boundingbox
            List<Point3d> voidpoints = new List<Point3d>();
            voidpoints.AddRange(BTLFunctions.GetCutPoints(TenonPlane, Refsides));  //adding the four points where the refEdge and the cutplane intersects
            voidpoints.AddRange(_BTLPartGeometry.CornerPoints);
            voidpoints = BTLFunctions.GetValidVoidPoints(TenonPlane, voidpoints);

            Box box = new Box(TenonPlane, voidpoints);
            double extra = 1000;
            box.X = new Interval(box.X.T0 - extra, box.X.T1 + extra);
            box.Y = new Interval(box.Y.T0 - extra, box.Y.T1 + extra);
            box.Z = new Interval(box.Z.T0, box.Z.T1 + extra);
            

            Brep Boxa = Brep.CreateFromBox(box);
            


            Brep TenonShape = BTLFunctions.GenerateTenonShape(TenonPlane, Height, BtmWidth, Topwidth, LengthInterval, Shapetype, ShapeRadius);

            double tolerance = CommonProps.tolerances;
            Rhino.Geometry.Brep[] breps = Rhino.Geometry.Brep.CreateBooleanDifference(Boxa, TenonShape, tolerance);


            return new PerformedProcess(Tenon, breps[0] );

        }

        


    }


    public class BTLMortise
    {
        // --- field ---
        public TenonMode TenonMode { get; private set; }
        public Plane WorkPlane { get; private set; } //Xaxis is the lengthDirection of the tenon
        public double Width { get; private set; }
        public double Length { get; private set; }
        public double Depth { get; private set; }
        public BooleanType LengthLimitedTop { get; private set; }
        public BooleanType LengthLimitedBottom { get; private set; }
        public bool FlipDirection { get; private set; }

        public double ShapeRadius { get; private set; }
        public TenonShapeType Shapetype { get; private set; }


        public BTLMortise(Plane _workPlane, double _width, double _length, double _depth, BooleanType _lengthLimitedTop, BooleanType _lengthLimitedBottom, int _shapetype, double _radius, bool _flipDirection)
        {
            WorkPlane = _workPlane;

            

            Width = _width;
            Depth = _depth;
            Length = _length;
            LengthLimitedTop = _lengthLimitedTop;
            LengthLimitedBottom = _lengthLimitedBottom;
            ShapeRadius = _radius;
            FlipDirection = _flipDirection;

            TenonMode = TenonMode.PlaneMode;
            if (_shapetype < 0 || _shapetype > 4)
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
            double Angle;
            double Inclination;
            double Slope;
            Point3d LocalStartPoint;
            Refside Refside;
            Plane UpdatedWorkPlane;
            


            BTLFunctions.GeneratePlaneAnglesParallell(_BTLPartGeometry, Length, WorkPlane, FlipDirection, out Angle, out Inclination, out Slope, out LocalStartPoint, out Refside, out UpdatedWorkPlane);
            



            MortiseType Mortise = new MortiseType();
            Mortise.Name = "Mortise";
            Mortise.StartX = LocalStartPoint.X;
            Mortise.StartY = LocalStartPoint.Y;
            Mortise.StartDepth = Math.Abs(LocalStartPoint.Z);
            Mortise.LengthLimitedBottom = LengthLimitedBottom;
            Mortise.LengthLimitedTop = LengthLimitedBottom;
            Mortise.Length = Length;
            Mortise.Width = Width;
            Mortise.Depth = Depth;
            Mortise.Shape = Shapetype;
            Mortise.ShapeRadius = ShapeRadius;


            Mortise.Angle = Rhino.RhinoMath.ToDegrees(Angle);
            Mortise.Inclination = Rhino.RhinoMath.ToDegrees(Inclination);
            Mortise.Slope = Rhino.RhinoMath.ToDegrees(Slope);
            Mortise.ReferencePlaneID = Refside.RefSideID;

            //Making intervals for tenonshape
            Interval Topwidth = new Interval(-Width / 2, Width / 2);
            Interval BtmWidth = Topwidth; //Not Mortise for tenon, but simplifies the code
            Interval LengthInterval = new Interval(0, Length);
            Interval DepthInternval = new Interval(-Depth, 1000);

            Brep MortiseShape = BTLFunctions.GenerateMortiseShape(UpdatedWorkPlane, DepthInternval, BtmWidth, Topwidth, LengthInterval, Shapetype, ShapeRadius,0);
            return new PerformedProcess(Mortise, MortiseShape);

        }


        



    }



    public class BTLDovetailMortise
    {
        // --- field ---
        public TenonMode TenonMode { get; private set; }
        public Plane WorkPlane { get; private set; } //Xaxis is the lengthDirection of the tenon
        public double Width { get; private set; }
        public double Length { get; private set; }
        public double Depth { get; private set; }
        public BooleanType LengthLimitedTop { get; private set; }
        public BooleanType LengthLimitedBottom { get; private set; }
        public bool FlipDirection { get; private set; }
        public double ConeAngle { get; private set; }
        public BooleanType UseFlankAngle { get; private set; }  //Donno what this is...
        public double FlankAngle { get; private set; }
        public double ShapeRadius { get; private set; }
        public TenonShapeType Shapetype { get; private set; }


        public BTLDovetailMortise(Plane _workPlane, double _width, double _length, double _depth, BooleanType _lengthLimitedTop, BooleanType _lengthLimitedBottom, int _shapetype, double _radius, bool _flipDirection, double _coneAngle, double _flankAngle, bool _useFlankAngle )
        {
            WorkPlane = _workPlane;

            

            Width = _width;
            Depth = _depth;
            Length = _length;
            LengthLimitedTop = _lengthLimitedTop;
            LengthLimitedBottom = _lengthLimitedBottom;
            ShapeRadius = _radius;
            FlipDirection = _flipDirection;
            ConeAngle = _coneAngle;

            if (!_useFlankAngle)
            {
                FlankAngle = 0;
                UseFlankAngle = BooleanType.no;
            }
            else
            {
                FlankAngle = _flankAngle;
                UseFlankAngle = BooleanType.yes;
            }

            

            TenonMode = TenonMode.PlaneMode;
            if (_shapetype < 0 || _shapetype > 4)
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
            double Angle;
            double Inclination;
            double Slope;
            Point3d LocalStartPoint;
            Refside Refside;
            Plane UpdatedWorkPlane;


            BTLFunctions.GeneratePlaneAnglesParallell(_BTLPartGeometry, Length, WorkPlane, FlipDirection, out Angle, out Inclination, out Slope, out LocalStartPoint, out Refside, out UpdatedWorkPlane);




            DovetailMortiseType DovetailMortise = new DovetailMortiseType();

            DovetailMortise.ConeAngleSpecified = true;
            
            

            DovetailMortise.Name = "DovetailMortise";
            DovetailMortise.StartX = LocalStartPoint.X;
            DovetailMortise.StartY = LocalStartPoint.Y;
            DovetailMortise.StartDepth = Math.Abs(LocalStartPoint.Z);
            DovetailMortise.LengthLimitedBottom = LengthLimitedBottom;
            DovetailMortise.LimitationTop = LimitationTopType.limited;
            DovetailMortise.Length = Length;
            DovetailMortise.Width = Width;
            DovetailMortise.Depth = Depth;
            DovetailMortise.Shape = TenonShapeType.radius;
            DovetailMortise.ShapeRadius = ShapeRadius;
            DovetailMortise.UseFlankAngle = UseFlankAngle;
            DovetailMortise.FlankAngle = Rhino.RhinoMath.ToDegrees( FlankAngle);
            DovetailMortise.ConeAngle = Rhino.RhinoMath.ToDegrees(ConeAngle);
            DovetailMortise.Angle = Rhino.RhinoMath.ToDegrees(Angle);
            DovetailMortise.Inclination = Rhino.RhinoMath.ToDegrees(Inclination);
            DovetailMortise.Slope = Rhino.RhinoMath.ToDegrees(Slope);
            DovetailMortise.ReferencePlaneID = Refside.RefSideID;


            double ExtraWidth = Width+ 2*Math.Tan(ConeAngle) * Length;


            //Making intervals for tenonshape
            Interval BtmWidth = new Interval(-Width / 2, Width / 2);
            Interval Topwidth = new Interval(-ExtraWidth / 2, ExtraWidth / 2);
            Interval LengthInterval = new Interval(0, Length);
            Interval DepthInternval = new Interval(-Depth, 1000);

            Brep MortiseShape = BTLFunctions.GenerateMortiseShape(UpdatedWorkPlane, DepthInternval, BtmWidth, Topwidth, LengthInterval, Shapetype, ShapeRadius,FlankAngle);
            return new PerformedProcess(DovetailMortise, MortiseShape);

        }






    }



    public class CustomBrep
    {
        // --- field ---
        public Brep CustomBrepShape { get; private set; }

        // --- constructors --- 
        public CustomBrep(Brep _customBrepShape)
        {
            CustomBrepShape = _customBrepShape;
        }

        // --- methods ---
        public PerformedProcess DelegateProcess(BTLPartGeometry _BTLPartGeometry, ManufactureMode _mode)
        {





            //Creating BTL processing

            SphereType DUMMY = new SphereType();
            DUMMY.Name = "NotInUse";
            DUMMY.Comment = "Not in use";
            DUMMY.Process = BooleanType.no;
            DUMMY.Radius = 0;
            DUMMY.Orientation = OrientationType.end;
            DUMMY.StartX = 0;
            DUMMY.StartY = 0;
            DUMMY.StartDepth = 0;
            DUMMY.Length = 0;
            DUMMY.StartOffset = 0;





            return new PerformedProcess(DUMMY, CustomBrepShape);

        }



    }






}












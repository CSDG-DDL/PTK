using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;



namespace PTK
{
    class GlobalAlignmentRules
    {
        public class GlobalAlignmentRule
        {
            public GenerateNodeGroupPlane NodeGroupDelegate;

            public GlobalAlignmentRule(GenerateNodeGroupPlane _nodeGroupDelegate)
            {
                NodeGroupDelegate = _nodeGroupDelegate;
            }

            public GlobalAlignmentRule()
            {

            }



            
        }



        public class AlignmentFromSurfaces
        {
            // --- field ---
            private List<Surface> alignmentSurfaces;
            private double maximumDistance;

            // --- constructors --- 
            public AlignmentFromSurfaces(List<Surface> _AlignmentSurfaces, double _MaximumDistance)
            {
                alignmentSurfaces = _AlignmentSurfaces;
                maximumDistance = _MaximumDistance;
            }

            // --- methods ---
            public Vector3d GenerateVector(Curve Curve)  //Checking element length
            {

                Curve.Domain = new Interval(0, 1);

                Point3d testPoint = Curve.PointAt(0.5);


                List<double> distance = new List<double>();


                double SmallestDistance = 10000000;

                Vector3d AlignVector = new Vector3d();

                foreach(Surface Surface in alignmentSurfaces)
                {
                    double u;
                    double v;

                    Surface.ClosestPoint(testPoint, out u, out v);

                    Point3d SurfacePoint = Surface.PointAt(u, v);

                    double Distance = SurfacePoint.DistanceTo(testPoint);
                    
                    if (Distance < SmallestDistance)
                    {
                        SmallestDistance = Distance;
                        AlignVector =  Surface.NormalAt(u, v);
                    }


                }

                return AlignVector;
            }


        }

        public class AlignmentFromVector
        {
            // --- field ---
            private Vector3d alignmentVector;
            

            // --- constructors --- 
            public AlignmentFromVector(Vector3d _alignmentVector)
            {
                alignmentVector = _alignmentVector;
            }

            // --- methods ---
            public Vector3d GenerateVector(Curve Curve)  //Checking element length
            {
                return alignmentVector;
            }


        }

        public class AlignmentFromPoints
        {
            // --- field ---
            private List<Surface> alignmentSurfaces;
            private double maximumDistance;
            List<Point3d> points;
            double curveDomain;

            // --- constructors --- 
            public AlignmentFromPoints(List<Point3d> _points, double _curveDomain)
            {
                points = _points;
                curveDomain = _curveDomain;
            }

            // --- methods ---
            public Vector3d GenerateVector(Curve Curve)  //Checking element length
            {

                Curve.Domain = new Interval(0, 1);

                if (curveDomain < 0)
                {
                    curveDomain = 0;
                }
                if (curveDomain > 1)
                {
                    curveDomain = 1;
                }




                Point3d PointOnCurve = Curve.PointAt(curveDomain);

                




                double SmallestDistance = 10000000;

                Vector3d AlignVector = new Vector3d();

                foreach (Point3d point in points)
                {

                    double Distance = point.DistanceTo(PointOnCurve);


                    if (Distance < SmallestDistance)
                    {
                        SmallestDistance = Distance;
                        AlignVector = new Vector3d(PointOnCurve - point);
                    }


                }

                return AlignVector;
            }


        }

        public class AlignmentFromPlane
        {
            // --- field ---
            private Plane plane;
            

            // --- constructors --- 
            public AlignmentFromPlane(Plane _plane)
            {
                plane = _plane;
                
            }

            // --- methods ---
            public Vector3d GenerateVector(Curve Curve)  //Checking element length
            {


                
                Point3d testPoint = Curve.PointAt(0.5);
                Point3d PointOnPlane = plane.ClosestPoint(testPoint);
                Vector3d AlignVector = testPoint - PointOnPlane;

                return AlignVector;
            }


        }




    }
}

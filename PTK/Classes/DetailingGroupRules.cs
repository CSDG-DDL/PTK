using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using feb;
using Rhino.Geometry;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.DocObjects;
using Rhino.Geometry.Intersect;

namespace PTK.Rules

{
    public class Rule
    {
        public CheckGroupDelegate checkdelegate;

        public Rule(CheckGroupDelegate _checkdelegate)
        {
            checkdelegate = _checkdelegate;
        }
    }

   

    public class ElementLength
    {
        // --- field ---
        private double minLength = 0;
        private double maxLength = 100000000000;

        // --- constructors --- 
        public ElementLength(double _min, double _max)
        {
            minLength = _min;
            maxLength = _max;
        }

        // --- methods ---
        public bool check(Detail _detail)  //Checking element length
        {
            Detail detail = _detail;
            Node node = detail.Node;
            List<Element1D> elements = detail.Elements;// ElementsPriorityMap.Keys.ToList();

            bool valid = false;
            foreach (Element1D element in elements)
            {
                double curvelength = element.BaseCurve.GetLength();
                if (minLength < curvelength && curvelength < maxLength == true)
                {
                    valid = true;
                }
                else
                {
                    valid = false;
                    break;
                }
            }
            return valid;
        }

    }

    public class ElementAmount 
    {
        // --- field ---
        private int minAmount = 0;
        private int maxAmount = 1000;

        // --- constructors --- 
        public ElementAmount(int _minAmount, int _maxAmount)
        {
            minAmount = _minAmount;
            maxAmount = _maxAmount;
        }

        // --- methods ---
        public bool check(Detail _detail)
        {
            Detail detail = _detail;
            Node node = detail.Node;
            List<Element1D> elements = detail.Elements;// ElementsPriorityMap.Keys.ToList();

            bool valid = false;
            foreach (Element1D element in elements)
            {
                if (minAmount <= elements.Count && elements.Count <= maxAmount == true)
                {
                    valid = true;
                }
                else
                {
                    valid = false;
                    break;
                }
            }
            return valid;
        }
    }

    public class NodeHitRegion 
    {
        // --- field ---
        public List<Curve> Polycurves { get; private set; }
        public double MaxDist = 9999999;

        // --- constructors --- 
        public NodeHitRegion(List<Curve> _polyCurves, double _maxDist)
        {
            Polycurves = _polyCurves;
            MaxDist = _maxDist;
        }

        // --- methods ---
        public bool check(Detail _detail)
        
        //This method checks if the node point is in the regions or not.
        {
            Detail detail = _detail;
            Node node = detail.Node;
            List<Element1D> elements = detail.Elements;// ElementsPriorityMap.Keys.ToList();
            Point3d point = node.Point;
            Point3d point2;
            double tolerance = CommonProps.tolerances;
            Plane Curveplane = new Plane();

            for (int i = 0; i < Polycurves.Count; i++)
            {
                Curve polycurve = Polycurves[i];


                if (polycurve.TryGetPlane(out Curveplane))
                {
                    point2 = Curveplane.ClosestPoint(point);

                if (point.DistanceTo(point2) >= MaxDist)
                {
                    return false;
                }
                    {
                    PointContainment relationship = polycurve.Contains(point2, Curveplane, tolerance);
                    if (relationship == PointContainment.Inside || relationship == PointContainment.Coincident)
                    {
                        if (Curveplane.DistanceTo(point2) < tolerance)
                        {
                            return true;
                        }
                    }
                    }
                }
            }

            return false;
        }
    }


    public class ElementTag
    {
        // --- field ---
        private List<string> tagsAre = new List<string>();
        private int mode = 0;

        // --- constructors --- 
        public ElementTag(List<string> _tagsAre, int _mode = 0)
        {
            tagsAre = _tagsAre;
            mode = _mode;
        }

        // --- methods ---
        public bool check(Detail _detail)
        {
            List<Element1D> _elements = _detail.Elements;
            List<String> detailTags = new List<string>();
            List<String> tagsAreStrict = tagsAre;
            List<String> tagsAreDistinct = tagsAre.Distinct().ToList();

            tagsAreStrict.Sort();
            tagsAreDistinct.Sort();

            bool valid = false;
           
            if (mode >= 4) //mode verifier
                mode = 0;

            if (mode == 0)  // Mode 0 - One of - The detail must contain one of the inputted tags
            {
                for (int i = 0; i < _elements.Count; i++)
                {
                    for (int j = 0; j < tagsAre.Count; j++)
                    {
                        if (_elements[i].Tag.Equals(tagsAre[j]))
                        {
                            valid = true;
                            break;
                        }
                    }
                }
            }

            if (mode == 1) // Mode 1 - At least -  The detail must contain all the inputted tags, but can also contain other tags
            {
                for (int j = 0; j < _elements.Count; j++)
                {
                    foreach (Element element in _elements)
                    {
                        detailTags.Add(element.Tag);
                    }

                    if (tagsAre.Count == 1)
                    {
                        if (detailTags.Contains(tagsAre[0]))
                            valid = true;
                    }
                    else
                    {
                        List<string> detailTagsDistinct = detailTags.Distinct().ToList();

                        if (detailTagsDistinct.Except(tagsAreDistinct).Count() == 0 && tagsAreDistinct.Except(detailTagsDistinct).Count() == 0)
                            valid = true;
                    }
                }
            }


            if (mode == 2) // Mode 2 - Distinct - The detail must contain all the inputted tags and no other tags
            {
                for (int j = 0; j < _elements.Count; j++)
                {

                    foreach (Element element in _elements)
                    {
                        detailTags.Add(element.Tag);
                    }

                    List<string> detailTagsDistinct = detailTags.Distinct().ToList();
                    detailTagsDistinct.Sort();
                    if (detailTagsDistinct.SequenceEqual(tagsAreDistinct))
                    {
                        valid = true;
                    }
                }
            }

            if (mode == 3) // Mode 3 - Strict - The detai must contain all the inputted tags and the exact amount 

            {
                for (int j = 0; j < _elements.Count; j++)
                {
                    foreach (Element element in _elements)
                    {
                        detailTags.Add(element.Tag);
                    }

                    detailTags.Sort();
                    if (detailTags.SequenceEqual(tagsAreStrict))
                    {
                        valid = true;
                    }
                }
            }
            return valid;
        }

    }


    public class ElementAngle
    {
        
        // --- field ---
        private int minimumAngle = 0;
        private int maximumAngle = 360;

        // --- constructors ---

        public ElementAngle(int _minimumAngle = 0, int _maximumAngle = 360)
        {
            minimumAngle = _minimumAngle;
            maximumAngle = _maximumAngle;
        }

        // --- properties ---
        // --- methods ---
    
        public bool check(Detail _detail)
        {
            Node _node = _detail.Node;
            List<Element1D> _elems = _detail.Elements;
            List<Vector3d> elementVectors = new List<Vector3d>();

            List<Point3d> pointsOnElement = new List<Point3d>();
            bool valid = new bool();
            int count = _elems.Count;
            Plane nodePlane = Plane.WorldXY;
   
            if (_elems.Count <= 1)
            {
            return false;
            }
                
            for (int i = 0; i < _elems.Count; i++)
            {
            //Creates a unitized vector of each element, starting in the node,
            Curve basecurve = _elems[i].BaseCurve;
            basecurve.Domain = new Interval(0,1);
            Point3d pointOnElement = basecurve.PointAt(0.5);
            pointsOnElement.Add(pointOnElement);
            Line elementLine = new Line(_node.Point, pointOnElement);
            Vector3d _elementVector = elementLine.Direction;
            elementVectors.Add(_elementVector);
                
            }
            //Creates a nodePlane 
            Plane.FitPlaneToPoints(pointsOnElement,out nodePlane);
            nodePlane.Origin = _node.Point;

            //for (int i = 0; i < _elems.Count; i++)
            //{
            ////Creates a unitized vector of each element, starting in the node, projected on the nodePlane
            //    Point3d pointOnElement = _elems[i].BaseCurve.PointAt(0.5);
            //    Point3d _pointOnElement = new Point3d(pointOnElement.X,pointOnElement.Y,nodePlane.OriginZ);
            //    Line elementLine = new Line(_node.Point, _pointOnElement);
            //    Vector3d _elementVector1 = elementLine.UnitTangent;
            //    elementVectors2D.Add(_elementVector1);
            //}
            
            //Deconstructs nodePlane to find the vectors on the plane 
            //Vector3d nX = nodePlane.XAxis;
            //Vector3d nY = nodePlane.YAxis;
            //Vector3d nZ = nodePlane.ZAxis;

            double angleToNodeVector;

            //Creates a sortingCircle on the Nodeplane and sorts the pointsOnElements according to  parameter on sorting circle
            Curve sortingCircle = new Circle(nodePlane, _node.Point, 10).ToNurbsCurve();
            List<double> ts = new List<double>();

            foreach (Point3d point in pointsOnElement)
            {
            double t;
            sortingCircle.ClosestPoint(point, out t);
            ts.Add(t);
            }

            var circleDictionoary = new Dictionary<double, Vector3d>();
            for (int i = 0; i < count; i++) circleDictionoary.Add(ts[i], elementVectors[i]);

            ts.Sort();

            List<Vector3d> sortVectors = new List<Vector3d>();
            for (int i = 0; i < count; i++)
            {
            sortVectors.Add(circleDictionoary[ts[i]]);
            }
            
            ////Finds angles between each element and the x-axis on the nodeplane. Adds to a dictionary and sorts according to the angle.

            //for (int i = 0; i < elementVectors.Count; i++)
            //{
            //    angleToNodeVector = (Vector3d.VectorAngle(nX, elementVectors[i], nodePlane) * 180 / Math.PI);
            //    angles.Add(angleToNodeVector);
            //}

            //var dictionary = new Dictionary<double, Vector3d>();
            //for (int i = 0; i < count; i++) dictionary.Add(angles[i], elementVectors[i]);

            //angles.Sort();

            //List<Vector3d> sortedVectors = new List<Vector3d>();
            //for (int i = 0; i < count; i++)
            //{
            //    sortedVectors.Add(dictionary[angles[i]]);
            //}
            
            //Adding the angles between each element to a list
            List<double> anglesBetween = new List<double>();

            if (count == 2)
            {
                double angleFirst = Vector3d.VectorAngle(sortVectors[1], sortVectors[0],nodePlane) * 180 / Math.PI;
                anglesBetween.Add(angleFirst);
                double angleSecond = Vector3d.VectorAngle(sortVectors[0], sortVectors[1],nodePlane) * 180 / Math.PI;
                anglesBetween.Add(angleSecond);
            }
            else
            {
            //Add the angle between last(count-1) and first(0) index manually
                double angleFirst = Vector3d.VectorAngle(sortVectors[count - 1], sortVectors[0]) * 180 / Math.PI;
                anglesBetween.Add(angleFirst);

            //Adds the angle between the others in a loop
                for (int i = 0; i < count - 1; i++)
                {
                    double angleBetweenNext = Vector3d.VectorAngle(sortVectors[i], sortVectors[i + 1]) * 180 / Math.PI;
                    anglesBetween.Add(angleBetweenNext);
                }
            }
            
            //Check each angleBetween with max and min
            foreach (double angle in anglesBetween)
            {
                if (minimumAngle <= angle && angle <= maximumAngle)
                {
                    valid = true;
                }
                else
                {
                    return false;
                }
            }


            return valid;

        }
    }



    /*
    public class NodeClosestPoint : DetailingGroupRules
    {


        private List<Point3d> points;
        private List<double> shortestDistance;
        private double tolerance;
        private bool OnlyOne; //Bool that tells if only one node pr point is allowed



        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////Properties
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////





        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////Constructors
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


        public NodeClosestPoint(List<Point3d> _points, double _tolerance, bool _OnlyOne)
        {
            points = new List<Point3d>();
            points = _points;
            tolerance = _tolerance;
            OnlyOne = _OnlyOne;

            shortestDistance = Enumerable.Repeat(1000000d, points.Count).ToList();  //Creating a list that has the same count as the point list. Used to check the closest point

        }


        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////Methods
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public bool check(Detail _detail)
        //This method checks if the node point is in the regions or not.
        {
            Point3d nodePoint = _detail.Nodes[0].Pt3d;

            for (int i = 0; i < points.Count; i++)
            {

                Point3d samplePoint = points[i];


                if (!OnlyOne)
                {
                    if (nodePoint.DistanceTo(samplePoint) < tolerance)
                    {
                        return true;
                    }
                }

                if (OnlyOne)
                {
                    double Distance = samplePoint.DistanceTo(nodePoint);

                    if (Distance < shortestDistance[i])
                    {
                        shortestDistance[i] = Distance;
                        return true;
                    }


                }


            }

            return false;




        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////constructors
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



    }
    */




}

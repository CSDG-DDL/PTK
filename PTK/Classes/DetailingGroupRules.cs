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

    /*
    public class ElementForce
    {
        // --- field ---
        private Interval allowedForce;
        private int forceType;

        // --- constructors --- 
        public ElementForce(Interval _allowedForce)
        {
            allowedForce = _allowedForce;
        }
        public ElementForce(Interval _allowedForce, int _forceType)
        {
            allowedForce = _allowedForce;
            forceType = _forceType;
        }

        // --- methods ---
        public bool checkCompression(Detail _detail)
        {
            // checking the compression
            Detail detail = _detail;
            Node node = detail.Node;
            
            if (allowedForce.IncludesParameter(detail.Elements[0].structuralData.maxCompressionForce.FX))
            {
                return true;
            }
            else
            {
                return false; 
            }
            
        }

        public bool checkTension(Detail _detail)
        {
            // checking the tension
            Detail detail = _detail;
            Node node = detail.Node;

            if (allowedForce.IncludesParameter(detail.Elements[0].structuralData.maxTensionForce.FX ))
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        public bool check(Detail _detail)
        {
            // checking the tension
            Detail detail = _detail;
            Node node = detail.Node;
            
            if (forceType == 0)
            {
                if (allowedForce.IncludesParameter(detail.Elements[0].structuralData.maxCompressionForce.FX))
                { return true; }
                else
                { return false; }
            }
            else if (forceType == 1)
            {
                if (allowedForce.IncludesParameter(detail.Elements[0].structuralData.maxTensionForce.FX))
                { return true; }
                else
                { return false; }
            }
            else if (forceType == 2)
            {
                if (allowedForce.IncludesParameter(detail.Elements[0].structuralData.maxShearDir1.FY))
                { return true; }
                else
                { return false; }
            }
            else if (forceType == 3)
            {
                if (allowedForce.IncludesParameter(detail.Elements[0].structuralData.maxShearDir2.FZ))
                { return true; }
                else
                { return false; }
            }
            else if (forceType == 4)
            {
                if (allowedForce.IncludesParameter(detail.Elements[0].structuralData.maxTorsion.MX))
                { return true; }
                else
                { return false; }
            }
            else if (forceType == 5)
            {
                if (allowedForce.IncludesParameter(detail.Elements[0].structuralData.maxBendingDir1.MY))
                { return true; }
                else
                { return false; }
            }
            else if (forceType == 6)
            {
                if (allowedForce.IncludesParameter(detail.Elements[0].structuralData.maxBendingDir2.MZ))
                { return true; }
                else
                { return false; }
            }
            else
                return false;

        }

    }

    */


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
        private int mode;

        // --- constructors --- 
        public ElementTag(List<string> _tagsAre, int _mode)
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
           
            if (mode >= 4)
            {
                mode = 0;
            }
                

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
                List<string> ElementTags = new List<string>();

                for (int j = 0; j < _elements.Count; j++)
                {
                    ElementTags.Add(_elements[j].Tag);
                }
                

                for (int j = 0; j < tagsAre.Count; j++)
                {
                    if (!ElementTags.Contains(tagsAre[j]))
                    {
                        return false;
                    }

                }
                return true;


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

    public class ElementOnNodeDomains
    {
        // --- field ---
        private int mode;

        // --- constructors --- 
        public ElementOnNodeDomains(int _mode)
        {
            mode = _mode;
        }

        // --- methods ---
        public bool check(Detail _detail)
        {
            List<Element1D> elements = _detail.Elements;
            Node node = _detail.Node;

            double tolerance = .000000001;

            Interval IntervalNotOnEnd = new Interval(0 + tolerance, 1 - tolerance);

            int AmountOnends = 0;
            int ElemCount = elements.Count;
            List<Point3d> centerPts = new List<Point3d>();

            foreach (Element1D elem in elements)
            {
                double t;
                Curve TempCurve = elem.BaseCurve;
                TempCurve.Domain = new Interval(0, 1);
                TempCurve.ClosestPoint(node.Point, out t);

       
                centerPts.Add(TempCurve.PointAt(0.5));
                
                if (!IntervalNotOnEnd.IncludesParameter(t))
                {
                    AmountOnends += 1;
                }
            }


                

            if (mode == 0 )  //L-Node
            {
                if (AmountOnends == 2 && ElemCount==2){return true; } else { return false; }
            }

            if (mode == 1) //T-node
            {
                if (AmountOnends == 1 && ElemCount == 2) { return true; } else { return false; }
            }

            if (mode == 2) //X-node
            {
                if (AmountOnends == 0 && ElemCount == 2) { return true; } else { return false; }
            }

            if (mode ==3) //EndNode
            {
                if (ElemCount==1) { return true; } else { return false; }
            }

            if (mode == 4) //StarNode
            {
                if (ElemCount > 2 && ElemCount == AmountOnends) { return true; } else { return false; }
            }

            if (mode ==5) //Planar
            {
                Plane PtPlane = new Plane();
                double deviation;
                List<Point3d> checkPoints = centerPts;
                checkPoints.Add(node.Point);

                Plane.FitPlaneToPoints(checkPoints, out PtPlane, out deviation);
                if (deviation<CommonProps.tolerances) { return true; } else { return false; }
            }

            if (mode == 6) //Orthogonal
            {
                for (int i = 0; i < centerPts.Count; i++)
                {
                    Vector3d FirstVector = new Line(node.Point, centerPts[i]).Direction;

                    for (int j = 0; j < centerPts.Count; j++)
                    {
                        if (j != i)
                        {
                            Vector3d SecondVector = new Line(node.Point, centerPts[j]).Direction;
                            double angle = Vector3d.VectorAngle(elements[j].BaseCurve.TangentAtStart, elements[i].BaseCurve.TangentAtStart);

                            angle = angle - Math.PI / 2;
                            double rest = angle % (Math.PI / 2);
                            rest = Math.Abs(rest);
                            if (rest < Rhino.RhinoDoc.ActiveDoc.ModelAngleToleranceRadians) { return true; }



                            else { return false; }
                        }
                    }

                }
            }
            return false;










            
            
        }

    }


    public class ElementAngle
    {
        
        // --- field ---
        private double minimumAngle = 0;
        private double maximumAngle = Math.PI*2;

        // --- constructors ---

        public ElementAngle(double _minimumAngle, double _maximumAngle)
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
            

            
            //Adding the angles between each element to a list
            List<double> anglesBetween = new List<double>();

            if (count == 2)
            {
                double angleFirst = Vector3d.VectorAngle(sortVectors[1], sortVectors[0],nodePlane);
                anglesBetween.Add(angleFirst);
                double angleSecond = Vector3d.VectorAngle(sortVectors[0], sortVectors[1],nodePlane);
                anglesBetween.Add(angleSecond);
            }
            else
            {
            //Add the angle between last(count-1) and first(0) index manually
                double angleFirst = Vector3d.VectorAngle(sortVectors[count - 1], sortVectors[0]);
                anglesBetween.Add(angleFirst);

            //Adds the angle between the others in a loop
                for (int i = 0; i < count - 1; i++)
                {
                    double angleBetweenNext = Vector3d.VectorAngle(sortVectors[i], sortVectors[i + 1]);
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using feb;
using Rhino.Geometry;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;

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

        // --- constructors --- 
        public NodeHitRegion(List<Curve> _polyCurves)
        {
            Polycurves = new List<Curve>();
        }

        // --- methods ---
        public bool check(Detail _detail)
        //This method checks if the node point is in the regions or not.
        {
            Detail detail = _detail;
            Node node = detail.Node;
            List<Element1D> elements = detail.Elements;// ElementsPriorityMap.Keys.ToList();

            double tolerance = CommonProps.tolerances; ;
            Plane Curveplane = new Plane();

            for (int i = 0; i < Polycurves.Count; i++)
            {
                Curve polycurve = Polycurves[i];
                if (polycurve.TryGetPlane(out Curveplane))
                {
                    PointContainment relationship = polycurve.Contains(node.Point, Curveplane, tolerance);
                    if (relationship == PointContainment.Inside || relationship == PointContainment.Coincident)
                    {
                        if (Curveplane.DistanceTo(node.Point) < tolerance)
                        {
                            return true;
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
            Plane nodePlane = _detail.Node.NodePlane;
            List<Element1D> _elems = _detail.Elements;
            List<Vector3d> elementVectors = new List<Vector3d>();


            for (int i = 0; i < _elems.Count; i++)
            {
                //Creates a unitized vector of each element, starting in the node
                Vector3d _elementVector = new Line(_node.Point, _elems[i].BaseCurve.PointAt(0.5)).UnitTangent;

                elementVectors.Add(_elementVector);
            }

            double x = 0f;
            double y = 0f;
            double z = 0f;

            for (int i = 0; i < elementVectors.Count; i++)
            {
                x += elementVectors[i].X;
                y += elementVectors[i].Y;
                z += elementVectors[i].Z;
            }

            //Creates a plane in the node that has normal like the average of all elementvectors --- SAVED FOR ADDING TO NODE CLASS

            //Vector3d nodeVector = new Vector3d(x / elementVectors.Count, y / elementVectors.Count, z / elementVectors.Count);
            //Plane nodePlane = new Plane(_node.Point, nodeVector);


            //Deconstructs nodePlane to construct a vector on the plane 
            Vector3d nX = nodePlane.XAxis;
            Vector3d nY = nodePlane.YAxis;
            Vector3d nZ = nodePlane.ZAxis;


            double angleToNodeVector = 0;
            bool valid = false;
            //Vector3d nodePlaneVector = (Vector3d.Add(nX));
            var data = new Dictionary<double, Vector3d>();

            //Finds angles between each element and the nodevector. Adds angles to a dictionary to sort according to the angle between element and nodeplane
            for (int i = 0; i < elementVectors.Count; i++)
            {
                angleToNodeVector = (Vector3d.VectorAngle(nX, elementVectors[i], nodePlane) * 180 / Math.PI);
                data.Add(angleToNodeVector, elementVectors[i]);
            }

            var myList = data.ToList();
            myList.Sort((pair1, pair2) =>
                pair1.Value.CompareTo(pair2
                    .Value)); //Adds the dictionary to a keyvaluelist and sorts it according to angles. Returns sorted elementvectors

            List<Vector3d> sortedVectors = new List<Vector3d>();

            foreach (KeyValuePair<double, Vector3d> pair in myList)
            {
                sortedVectors.Add(pair.Value);
            }
            //Check angle between last (count-1) and first(0) index manual

            //double angleBetweenNext = Vector3d.VectorAngle(sortedVectors[(sortedVectors.Count)-1], sortedVectors[0], nodePlane) * 180 / Math.PI;
            //if (minimumAngle <= angleBetweenNext && angleBetweenNext <= maximumAngle)
            //{
            //    valid = true;
            //}
            //else
            //{
            //    return false;
            //}

            ////Check angle between the others in loop
            //for (int i = 0; i < sortedVectors.Count; i++)
            //{
            //    double _angleBetweenNext = 360;
            //    Vector3d.VectorAngle(sortedVectors[i], sortedVectors[i + 1], nodePlane);
            //    if (minimumAngle <= angleBetweenNext && angleBetweenNext <= maximumAngle)
            //    {
            //        valid = true;
            //    }
            //    else
            //    {
            //        return false;
            //    }

            //}

            //return valid;

            //angleBetweenNext = Math.Abs(Math.Atan2(sortedVectors[0].Y - sortedVectors[(sortedVectors.Count) - 1].Y, sortedVectors[0].X - sortedVectors[(sortedVectors.Count) - 1].X) * 180 / Math.PI);
            //    if (minimumAngle <= angleBetweenNext && angleBetweenNext <= maximumAngle)
            //    {
            //        valid = true;
            //    }
            //    else
            //    {
            //        return false;
            //    }

            //    for (int i = 0; i < sortedVectors.Count; i++)
            //    {
            //        angleBetweenNext = Math.Abs(Math.Atan2(sortedVectors[i + 1].Y - sortedVectors[i].Y, sortedVectors[i + 1].X - sortedVectors[i].X) * 180 / Math.PI);
            //        if (minimumAngle <= angleBetweenNext && angleBetweenNext <= maximumAngle)
            //        {
            //            valid = true;
            //        }
            //        else
            //        {
            //            return false;
            //        }
            //    }

            //    return valid;

            //
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

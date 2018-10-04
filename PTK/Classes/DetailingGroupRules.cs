using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

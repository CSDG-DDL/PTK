using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;

namespace PTK
{
    public class CompositeNew
    {
        public string CompositeName { get; private set; }
        public List<SubElement> Subelements { get; private set; }
        public double WidthSimplified { get; private set; }
        public double HeightSimplified { get; private set; }
        public Interval WidthInterval { get; private set; }
        public Interval HeightInterval { get; private set; }
        public MaterialProperty MaterialProperty { get; private set; }



        public CompositeNew()
        {
            CompositeName = "";
            Subelements = new List<SubElement>();
        }

        public CompositeNew(string _compositeName, List<CrossSection> _crossSections, Element1D MainElement)
        {
            CompositeName = _compositeName;
            Subelements = new List<SubElement>();

            foreach(CrossSection CS in _crossSections)
            {
                Subelements.Add(new SubElement(MainElement, CS));
            }

            if (_crossSections.Count == 1)
            {
                WidthSimplified = _crossSections[0].GetWidth();
                WidthSimplified = _crossSections[0].GetWidth();
            }
            else
            {
                //THis is a simplified version of finding genereating widths and heights
                List<Point3d> Points = new List<Point3d>();

                foreach (SubElement s in Subelements)
                {
                    Points.AddRange(s.Shape2dCorners);


                }

                Plane csplane = MainElement.CroSecLocalPlane;
                List<double> Widths = new List<double>();
                List<double> Heights = new List<double>();


                foreach (Point3d p in Points)
                {
                    Point3d pt;
                    csplane.RemapToPlaneSpace(p, out pt);

                    Widths.Add(pt.X);
                    Heights.Add(pt.Y);

                }

                Widths.Sort();
                Heights.Sort();

                WidthSimplified = Widths.Last() - Widths.First();
                HeightSimplified = Heights.Last() - Heights.First();



            }


        }

        public CompositeNew(CompositeInput Input, Element1D MainElement)
        {
            

            CompositeName = Input.CompositeName;
            Subelements = new List<SubElement>();


            //NBNB!!! THIS IS A SIMPLIFICATION!!!!!
            MaterialProperty = Input.CrossSections[0].MaterialProperty;

            List<CrossSection> _crossections = Input.CrossSections;


            foreach (CrossSection CS in _crossections)
            {
                Subelements.Add(new SubElement(MainElement, CS));
            }

            if (_crossections.Count == 1)
            {
                WidthSimplified = _crossections[0].GetWidth();
                HeightSimplified = _crossections[0].GetHeight();
                HeightInterval = new Interval(-HeightSimplified / 2, HeightSimplified / 2);
                WidthInterval = new Interval(-WidthSimplified / 2, WidthSimplified / 2);


            }
            else
            {
                //THis is a simplified version of finding genereating widths and heights
                List<Point3d> Points = new List<Point3d>();

                foreach (SubElement s in Subelements)
                {
                    Points.AddRange(s.Shape2dCorners);


                }

                Plane csplane = MainElement.CroSecLocalPlane;
                List<double> Widths = new List<double>();
                List<double> Heights = new List<double>();
                 

                foreach(Point3d p in Points)
                {
                    Point3d pt;
                    csplane.RemapToPlaneSpace(p, out pt);

                    Widths.Add(pt.X);
                    Heights.Add(pt.Y);                    

                }

                Widths.Sort();
                Heights.Sort();

                WidthSimplified = Widths.Last() - Widths.First();
                HeightSimplified = Heights.Last() - Heights.First();


                WidthInterval = new Interval(Widths.First(), Widths.Last());
                HeightInterval = new Interval(Heights.First(), Heights.Last());



            }


        }











    }


    public class CompositeInput
    {
        public string CompositeName { get; private set; }
        public List<CrossSection> CrossSections { get; private set; }

        public CompositeInput(CrossSection _crossSection)
        {
            CompositeName = "Single";
            CrossSections = new List<CrossSection>();
            CrossSections.Add(_crossSection);
        }

        public CompositeInput(string _name, List<CrossSection> _crossSections)
        {
            CompositeName = _name;
            CrossSections = new List<CrossSection>(); ;
            CrossSections = _crossSections;

        }

        public CompositeInput()
        {
            CompositeName = "Empty";
            CrossSections = new List<CrossSection>(); ;
            CrossSections.Add(new RectangleCroSec("Default"));
        }


    }

    



}


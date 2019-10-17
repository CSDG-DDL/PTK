using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grasshopper;
using System.ComponentModel;
using System.Xml.Serialization;
using Rhino.Geometry;


namespace PTK
{
    [Serializable]
    public class Job
    {
        [XmlIgnore]
        public int IDCounter { get; set; }

        [XmlAttribute()]
        public string BvxVersion { get; set; }
        [XmlAttribute()]
        public string DeliveryDate { get; set; }
        public List<Part> Parts { get; set; }
        public string AttDefs { get; set; }




        public Job()
        {
            IDCounter = 0;
            BvxVersion = "1.0";
            DeliveryDate = "05.01.2020";
            Parts = new List<Part>();

        }



        public void AddPart(Part _part)
        {
            _part.PartId = IDCounter;
            IDCounter++;

            Parts.Add(_part);
        }


    }



    public class Part
    {
        [XmlAttribute()]
        public string Name { get; set; }
        [XmlAttribute()]
        public int PartId { get; set; }
        [XmlAttribute()]
        public double Width { get; set; }
        [XmlAttribute()]
        public double Height { get; set; }
        [XmlAttribute()]
        public double Length { get; set; }
        [XmlAttribute()]
        public string Dimension { get; set; }
        [XmlAttribute()]
        public int ReqQuantity { get; set; }
        [XmlAttribute()]
        public string Comments { get; set; }
        [XmlAttribute()]
        public string JobName { get; set; }
        [XmlAttribute()]
        public string Grade { get; set; }

        [System.Xml.Serialization.XmlArrayItemAttribute("SawCut", typeof(SawCut), IsNullable = false)]
        public List<Operations> Operations { get; set; }




        public Part()
        {

        }

        public Part(double _width, double _height, double _length, string _name)
        {
            Name = _name;
            Width = _width;
            Height = _height;
            Length = _length;
            Dimension = "Rectangular";
            ReqQuantity = 1;
            Comments = "Lars/Jens: Kva er Logikken på innhald i comments ? :-)";
            JobName = "Test";
            Grade = "C30";

            Operations = new List<Operations>();

            Operations.Add(new SawCut(0));
            Operations.Add(new SawCut(_length));



        }












    }

    [XmlInclude(typeof(SawCut))]

    public class Operations
    {

    }

    [Serializable]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.design2machine.com")]

    public class SawCut : Operations
    {
        [XmlAttribute()]
        public string ReferenceSide { get; set; }
        [XmlAttribute()]
        public Orientation Orientation { get; set; }
        [XmlAttribute()]
        public Double Angle { get; set; }
        [XmlAttribute()]
        public Double Bevel { get; set; }
        [XmlAttribute()]
        public double CrossMeas1 { get; set; }
        [XmlAttribute()]
        public double CrossMeas2 { get; set; }
        [XmlAttribute()]
        public double LengthMeas { get; set; }


        public SawCut()
        {

        }



        public SawCut(double _LengthMeas)
        {
            ReferenceSide = "1";
            if (_LengthMeas < 0.1)
            {
                Orientation = Orientation.Right;
            }
            else
            {
                Orientation = Orientation.Left;
            }
            Angle = 90;
            Bevel = 90;
            CrossMeas1 = 0;
            CrossMeas2 = 0;
            LengthMeas = _LengthMeas;

        }

    }

    public class Lap : Operations
    {
        [XmlAttribute()]
        public string ReferenceSide { get; set; }
        [XmlAttribute()]
        public Orientation LengthOrientation { get; set; }
        [XmlAttribute()]
        public Double Angle { get; set; }
        [XmlAttribute()]
        public Double Bevel { get; set; }
        [XmlAttribute()]
        public Double Rotation { get; set; }
        [XmlAttribute()]
        public double CrossMeas1 { get; set; }
        [XmlAttribute()]
        public double CrossMeas2 { get; set; }
        [XmlAttribute()]
        public double LengthMeas { get; set; }
        [XmlAttribute()]
        public double Length { get; set; }


        public Lap()
        {

        }



        public Lap(Plane Refplane, Plane _LapPlane)
        {

        }




    }






    public enum Orientation
    {
        Left, Right, Center
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;

namespace PTK.PlaneRules
{
    //THIS SET OF CLASSES ARE RULES THAT DEFINES HOW A DETAILINGGROUPPLANE IS DEFINED

    public class PlaneRule
    {
        public GenerateNodeGroupPlane NodeGroupDelegate;

        public PlaneRule(GenerateNodeGroupPlane _nodeGroupDelegate)
        {
            NodeGroupDelegate = _nodeGroupDelegate;
        }

        public PlaneRule()
        {

        }

        

        public static Vector3d AlignVectorByElementName(Detail _detail, string _name)
        {

            List<Element1D> ValidElements = _detail.Elements.Where(o => o.Tag == _name).ToList();
            if (ValidElements.Count > 0)
            {
                Element1D AlignmentElement = ValidElements[0];
                Point3d NodePoint = _detail.Node.Point;
                Vector3d AlignmentVector = new Vector3d();
                if (NodePoint.DistanceTo(AlignmentElement.PointAtStart) < NodePoint.DistanceTo(AlignmentElement.PointAtEnd))
                {
                    AlignmentVector = AlignmentElement.BaseCurve.TangentAtStart;
                }
                else
                {
                    AlignmentVector = -AlignmentElement.BaseCurve.TangentAtEnd;
                }

                

                return AlignmentVector;


            }

            return Vector3d.XAxis;
        }

    }



    //Constructs a plane based on a input-vector
    public class NormalVectorPlane
    {
        // --- field ---
        private Vector3d NormalVector;
        private string AlignmentElementName;


        // --- constructors --- 
        public NormalVectorPlane(Vector3d _normalVector, string _alignmentElementName)
        {
            NormalVector = _normalVector;
            AlignmentElementName = _alignmentElementName;

        }

        // --- methods ---
        public Plane GenerateDetailingGroupPlane(Detail _detail)  //Checking element length
        {
            Detail detail = _detail;
            

            Plane NodeGroupPlane = new Plane(detail.Node.Point, NormalVector);

            List<Element1D> ValidElements = detail.Elements.Where(o => o.Tag == AlignmentElementName).ToList();
            if (ValidElements.Count > 0)
            {
                Element1D AlignmentElement = ValidElements[0];
                Point3d NodePoint = detail.Node.Point;
                Vector3d AlignmentVector = new Vector3d();
                if(NodePoint.DistanceTo(AlignmentElement.PointAtStart)< NodePoint.DistanceTo(AlignmentElement.PointAtEnd))
                {
                    AlignmentVector = AlignmentElement.BaseCurve.TangentAtStart;
                }
                else
                {
                    AlignmentVector = -AlignmentElement.BaseCurve.TangentAtEnd;
                }
                double Angle = Vector3d.VectorAngle(NodeGroupPlane.XAxis, AlignmentVector, NodeGroupPlane);
                NodeGroupPlane.Rotate(Angle, NodeGroupPlane.ZAxis);


            }
            
            return NodeGroupPlane;
        }


    }

    public class NormalParallellToElementPlane
    {
        // --- field ---
        private string NormalAxisElement;
        private string AlignmentElementName;


        // --- constructors --- 
        public NormalParallellToElementPlane(string _normalAxisElement, string _alignmentElementName)
        {
            NormalAxisElement = _normalAxisElement;
            AlignmentElementName = _alignmentElementName;

        }

        // --- methods ---
        public Plane GenerateDetailingGroupPlane(Detail _detail)  //Checking element length
        {
            Detail detail = _detail;

            Vector3d NormalVector = PlaneRule.AlignVectorByElementName(detail, NormalAxisElement);

            Plane NodeGroupPlane = new Plane(detail.Node.Point, NormalVector);
            
            Vector3d Alignmentvector = PlaneRule.AlignVectorByElementName(detail, AlignmentElementName);
            double Angle = Vector3d.VectorAngle(NodeGroupPlane.XAxis, Alignmentvector, NodeGroupPlane);
            NodeGroupPlane.Rotate(Angle, NodeGroupPlane.ZAxis);


            return NodeGroupPlane;
        }


    }

    



    public class SurfaceNormalPlane
    {
        // --- field ---
        private Surface Surface;
        private string AlignmentElementName;


        // --- constructors --- 
        public SurfaceNormalPlane(Surface _surface, string _alignmentElementName)
        {
            Surface = _surface;
            AlignmentElementName = _alignmentElementName;

        }

        // --- methods ---
        public Plane GenerateDetailingGroupPlane(Detail _detail)  //Checking element length
        {
            Detail detail = _detail;


            Point3d NodePoint = detail.Node.Point;
            double u;
            double v;
            Surface.ClosestPoint(NodePoint, out u, out v);
            

            Plane NodeGroupPlane = new Plane(NodePoint, Surface.NormalAt(u, v));


            List<Element1D> ValidElements = detail.Elements.Where(o => o.Tag == AlignmentElementName).ToList();
            if (ValidElements.Count > 0)
            {
                Element1D AlignmentElement = ValidElements[0];
                
                Vector3d AlignmentVector = new Vector3d();
                if (NodePoint.DistanceTo(AlignmentElement.PointAtStart) < NodePoint.DistanceTo(AlignmentElement.PointAtEnd))
                {
                    AlignmentVector = AlignmentElement.BaseCurve.TangentAtStart;
                }
                else
                {
                    AlignmentVector = -AlignmentElement.BaseCurve.TangentAtEnd;
                }
                double Angle = Vector3d.VectorAngle(NodeGroupPlane.XAxis, AlignmentVector, NodeGroupPlane);
                NodeGroupPlane.Rotate(Angle, NodeGroupPlane.ZAxis);


            }

            return NodeGroupPlane;
        }


    }
}

using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;


namespace PTK
{
    public class Node : IEquatable<Node>
    {
        public Point3d Point { get; private set; }
        public List<Vector3d> DisplacementVectors { get; private set; } = new List<Vector3d>();
        

        public Node()
        {
            Point = new Point3d();
        }
        public Node(Point3d _point)
        {
            Point = _point;
        }

        public bool Equals(Node _other)
        {
            //It is necessary to consider a minute error
            if (Point == _other.Point)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        

        public bool Equals(Point3d _point)
        {
            if(Point == _point)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public int AddDisplacementVector(Vector3d _vector)
        {
            DisplacementVectors.Add(_vector);
            return DisplacementVectors.Count;
        }

        public Node DeepCopy()
        {
            return (Node)base.MemberwiseClone();
        }
        public override string ToString()
        {
            string info;
            info = "<Node> Point:" + Point.ToString() +
                " DisplacementVectors:" + DisplacementVectors.ToString();
            return info;
        }
        public bool IsValid()
        {
            return true;
        }
    }

    public class GH_Node : GH_Goo<Node>
    {
        public GH_Node() { }
        public GH_Node(GH_Node other) : base(other.Value) { this.Value = other.Value.DeepCopy(); }
        public GH_Node(Node sec) : base(sec) { this.Value = sec; }
        public override bool IsValid => base.m_value.IsValid();

        public override string TypeName => "Node";

        public override string TypeDescription => "Description";

        public override IGH_Goo Duplicate()
        {
            return new GH_Node(this);
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }

    public class Param_Node : GH_PersistentParam<GH_Node>
    {
        public Param_Node() : base(new GH_InstanceDescription("Node", "Node", "Description", CommonProps.category, CommonProps.subcate0)) { }

        protected override System.Drawing.Bitmap Icon { get { return Properties.Resources.Node; } }  //Set icon image

        public override Guid ComponentGuid => new Guid("08b7c467-367e-4a25-856b-fae990bfd78a");

        protected override GH_GetterResult Prompt_Plural(ref List<GH_Node> values)
        {
            return GH_GetterResult.success;
        }

        protected override GH_GetterResult Prompt_Singular(ref GH_Node value)
        {
            return GH_GetterResult.success;
        }
    }
}

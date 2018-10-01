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
        // --- field ---
        public Point3d Point { get; private set; }

        // --- constructors --- 
        public Node()
        {
            Point = new Point3d();
        }
        public Node(Point3d _point)
        {
            Point = _point;
        }

        // --- methods ---
        public bool Equals(Node _other)
        {
            return Equals(_other.Point);
        }

        public bool Equals(Point3d _point)
        {
            return Point.EpsilonEquals(_point, CommonProps.tolerances);
        }

        public Node DeepCopy()
        {
            return (Node)base.MemberwiseClone();
        }
        public override string ToString()
        {
            string info;
            info = "<Node> Point:" + Point.ToString();
            return info;
        }
        public bool IsValid()
        {
            return Point != null;
        }
    }

    public class GH_Node : GH_Goo<Node>
    {
        public GH_Node() { }
        public GH_Node(GH_Node other) : base(other.Value) { this.Value = other.Value.DeepCopy(); }
        public GH_Node(Node node) : base(node) { this.Value = node; }
        public override bool IsValid => base.m_value.IsValid();

        public override string TypeName => "Node";

        public override string TypeDescription => "At the intersection of Elements, or the end point";

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
        public Param_Node() : base(new GH_InstanceDescription("Node", "Node", "At the intersection of Elements, or the end point", CommonProps.category, CommonProps.subcate0)) { }

        protected override System.Drawing.Bitmap Icon { get { return Properties.Resources.ParaNode; } }  //Set icon image

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

﻿using System;
using System.Collections.Generic;
using Rhino.Geometry;

namespace PTK
{
    public class Node : IEquatable<Node>
    {
        #region fields
        private int id;
        private List<int> elemIds;
        private Point3d pt3d;
        private double x;
        private double y;
        private double z;
        #endregion

        #region constructors
        public Node(Point3d pt)
        {
            pt3d = pt;
            x = pt.X;
            y = pt.Y;
            z = pt.Z;
            id = -999;
            elemIds = new List<int>();
        }
        #endregion

        #region properties
        public Point3d Pt3d { get { return pt3d; } }
        public double X { get { return x; } }
        public double Y { get { return y; } }
        public double Z { get { return z; } }
        public int ID { get { return id; } set { id = value; } }
        public List<int> ElemIds { get { return elemIds; } set { elemIds = value; } }
        #endregion

        #region methods
        public bool Equals(Node other)
        {
            if (x == other.X && y == other.Y && z == other.Z)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static List<Node> AddElemIds(List<Node> _nodes, Element _elem, Node _nd)
        {
            if (!_nodes.Contains(_nd))
            {
                _nd.ElemIds.Add(_elem.ID);
                _nodes.Add(_nd);
            }
            else
            {
                _nodes.Find(n => n.Pt3d == _nd.Pt3d).elemIds.Add(_elem.ID);
            }

            return _nodes;
        }

        #endregion
    }

    public class Element 
    {
        #region fields
        private int id;
        private int n0id;
        private int n1id;
        private Line elemLine;
        private double length;
        #endregion

        #region constructors
        public Element(Line line)
        {
            elemLine = line;
            length = line.Length;
            id = -999;
            n0id = -999;
            n1id = -999;
        }
        #endregion

        #region properties
        public Line Ln { get { return elemLine; } }
        public double Length { get { return length; } }
        public int N0id { get { return n0id; } }
        public int N1id { get { return n1id; } }
        public int ID { get { return id; } set { id = value; } }
        #endregion

        #region methods
        #endregion
    }
}

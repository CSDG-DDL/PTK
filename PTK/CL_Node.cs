﻿using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;


namespace PTK
{
    public class Node : IEquatable<Node>
    {
        #region fields
        private int id;
        private List<int> elemIds;
        private Point3d pt3d;
        private Plane nodePlane;
        private double x;
        private double y;
        private double z;
        private static int idCount = 0;
        int temp;
        #endregion

        #region constructors
        //The points are given from the PTK4_assemble. ID is unique by iterating each time the class is instanced. idCount is static
        public Node(Point3d pt)
        {
            pt3d = pt;
            x = pt.X;
            y = pt.Y;
            z = pt.Z;
            id = idCount;
            nodePlane = new Plane(pt, new Vector3d(0, 0, 1));
            idCount++;
            elemIds = new List<int>();
            
        }
        #endregion

        #region properties
        public Point3d Pt3d { get { return pt3d; } }
        public List<int> ElemIds { get { return elemIds; } }
        public double X { get { return x; } }
        public double Y { get { return y; } }
        public double Z { get { return z; } }
        public int ID { get { return id; } }  //removed the possability to set an ID

        #endregion

        #region methods


        //Adding neighbours. see the function called AssignNeighbours in function.cs. 
        public void AddNeighbour(int ids)
        { 
            if (elemIds==null)
            {
                elemIds = new List<int>();
            }
            temp = ids;
            elemIds.Add(ids);
        }


        //are the next functions in use? Probably usefull later when extracting the geometry. 
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

        

        public static int FindNodeId(List<Node> _nodes, Point3d _pt)
        {
            int tempId = -999;
            tempId = _nodes.Find(n => n.Pt3d == _pt).ID;

            return tempId;
        }

        public static Node FindNodeById(List<Node> _nodes, int _nid)
        {
            Node tempNode;
            tempNode = _nodes.Find(n => n.ID == _nid);

            return tempNode;
        }

        public static void ResetIDCount()
        {
            idCount = 0;
        }

        #endregion
    }
}

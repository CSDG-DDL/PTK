﻿using Rhino.Geometry;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Grasshopper;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace PTK
{
    public class BuildingProject
    {
        public ProjectType BTLProject{ get; private set; }
        public List<BuildingElement> BuildingElements{ get; private set; }
        public List<BuildingNode> BuildingNodes { get; private set; }
        private bool ready = false;

        public BuildingProject(ProjectType _btlProject)
        {
            BTLProject = _btlProject;
            BuildingElements = new List<BuildingElement>();

        }


        public void PrepairElements(List<Element1D> _elements, List<OrderedTimberProcess> _orderedTimberProcesses)  //PHASE1: PREPAIR
        {
            foreach (Element1D element in _elements)
            {
                List<OrderedTimberProcess> OrderedProcesessInElement = _orderedTimberProcesses.Where(o => o.element == element).ToList();

                List<PerformTimberProcessDelegate> processDelegateInElement = new List<PerformTimberProcessDelegate>();
                foreach (OrderedTimberProcess Process in OrderedProcesessInElement)
                {
                    processDelegateInElement.Add(Process.PerformTimberProcess);
                }



                //Find all correct _ordereded Timberprocesses. Store in list


                BuildingElements.Add(new BuildingElement(element, processDelegateInElement));
            }
        }

        public void ManufactureProject(ManufactureMode _mode)
        {
            foreach(BuildingElement buildingElement in BuildingElements)
            {
                buildingElement.ManufactureElement(_mode);
                if (_mode == ManufactureMode.BTL)
                {
                    BTLProject.Parts.Part.AddRange(buildingElement.BTLParts);
                }
               
            }
        }

        

        public DataTree<Brep> GetBreps()
        {
            DataTree<Brep> dataTree = new DataTree<Brep>();
            for(int i = 0; i < BuildingElements.Count; i++)
            {
                BuildingElement BuildingElement = BuildingElements[i];
                for (int j = 0; j < BuildingElement.Sub3DElements.Count; j++)
                {
                    Sub3DElement Sub3DElement = BuildingElement.Sub3DElements[j];
                    dataTree.AddRange(Sub3DElement.ProcessedStock, new Grasshopper.Kernel.Data.GH_Path(i, j));



                }
            }
            return dataTree;
        }
        
    }

    

    public class BuildingElement
    {
        public Element1D Element { get; private set; }
        public List<Sub3DElement> Sub3DElements { get; private set; }
        public List<PartType> BTLParts;
        public List<OrderedTimberProcess> OrderedTimberProcesseses { get; private set; }
        private bool ready = false;

        public BuildingElement(Element1D _element, List<PerformTimberProcessDelegate> _processDelegate)      //PHASE1: PREPAIR
        {
            BTLParts = new List<PartType>();
            ready = true;
            Element = _element;
            Sub3DElements = new List<Sub3DElement>();
            
            foreach (Sub2DElement subElement in Element.Sub2DElements)
            {
                //ConstructSub3DElement
                Sub3DElements.Add(new Sub3DElement(Element, subElement, _processDelegate));

            }

        }

  

        public void ManufactureElement(ManufactureMode _mode)      //PHASE 2: Manufacture
        {
            if (ready)
            {
                foreach(Sub3DElement subelem3D in Sub3DElements)
                {
                    subelem3D.ManufactureSubElement(_mode);

                    if (_mode == ManufactureMode.BTL || _mode == ManufactureMode.BOTH)
                    {
                        BTLParts.Add(subelem3D.BTLPart);

                    }

                }
            }

        }

    }


    public class Sub3DElement
    {
        public PartType BTLPart { get; private set; }                                           //Needed
        public Brep Stock { get; private set; }  
        public List<Brep> ProcessedStock { get; private set; }
        public List<Brep> VoidProcess { get; private set; }
        public Plane CornerPlane { get; private set; }
        public double height{ get; private set; }
        public double width { get; private set; }
        public double length { get; private set; }

        public List<PerformTimberProcessDelegate> PerformTimberProcesses { get; private set; }  //Needed
        public BTLPartGeometry BTLPartGeometry { get; private set; }
        private bool ready = false;
        

        public Sub3DElement(Element1D _element, Sub2DElement _Sub2DElement, List<PerformTimberProcessDelegate> _processDelegate)     //PHASE1: PREPAIR
        {
            BTLPart = new PartType(); 

             height = _Sub2DElement.CrossSection.GetHeight();
             width = _Sub2DElement.CrossSection.GetWidth();
             length = _element.BaseCurve.GetLength();


            CoordinateSystemType CoordinateSystem = new CoordinateSystemType();

            List<Point3d> startPoints = new List<Point3d>();
            List<Point3d> endPoints = new List<Point3d>();
            List<Point3d> cornerPoints = new List<Point3d>();


            ready = true;
            PerformTimberProcesses = _processDelegate;

            //Making BTL PLANE

            //Move half height, half width
            List<Refside> refSides = new List<Refside>();
            Plane ElementPlaneCentric = _element.CroSecLocalPlane;

            // Making plane centric to Sub-element
            Plane SubElemPlaneCentric = new Plane(ElementPlaneCentric);
            SubElemPlaneCentric.Translate(SubElemPlaneCentric.XAxis * _Sub2DElement.Alignment.OffsetY + SubElemPlaneCentric.YAxis * _Sub2DElement.Alignment.OffsetZ);

            
            


            //Making CornerPlane, bottom left corner
            CornerPlane = new Plane(SubElemPlaneCentric);
            CornerPlane.Translate(CornerPlane.XAxis * width / 2 + (CornerPlane.YAxis * -height / 2));

            Plane btlPlane = new Plane(CornerPlane.Origin, CornerPlane.ZAxis, CornerPlane.YAxis);

            Plane refPlane1 = new Plane(btlPlane.Origin, btlPlane.XAxis, btlPlane.ZAxis);

            Plane refPlane2 = new Plane(refPlane1);
            refPlane2.Translate(btlPlane.YAxis * height);                                   //Move
            refPlane2 = new Plane(refPlane2.Origin, btlPlane.XAxis, -btlPlane.YAxis);         //Rearrange

            Plane refPlane3 = new Plane(refPlane2);
            refPlane3.Translate(btlPlane.ZAxis * width);
            refPlane3 = new Plane(refPlane3.Origin, btlPlane.XAxis, -btlPlane.ZAxis);

            Plane refPlane4 = new Plane(refPlane3);
            refPlane4.Translate(btlPlane.ZAxis * width);
            refPlane4 = new Plane(refPlane4.Origin, btlPlane.XAxis, btlPlane.YAxis);

            refSides.Add(new Refside(1, refPlane1, length));
            refSides.Add(new Refside(2, refPlane2, length));
            refSides.Add(new Refside(3, refPlane3, length));
            refSides.Add(new Refside(4, refPlane4, length));

            



            foreach (Refside side in refSides)
            {
                startPoints.Add(side.RefPoint);
                Plane tempPlane = new Plane(side.RefPoint, CornerPlane.ZAxis);

                tempPlane.Translate(tempPlane.ZAxis * length);
                endPoints.Add(tempPlane.Origin);
            }

            //Cornerpoints are used to define cuttingboxes
            cornerPoints = new List<Point3d>();
            cornerPoints.AddRange(startPoints);
            cornerPoints.AddRange(endPoints);



            CoordinateSystem.XVector.X = btlPlane.XAxis.X;
            CoordinateSystem.XVector.Y = btlPlane.XAxis.Y;
            CoordinateSystem.XVector.Z = btlPlane.XAxis.Z;
            CoordinateSystem.YVector.X = btlPlane.YAxis.X;
            CoordinateSystem.YVector.Y = btlPlane.YAxis.Y;
            CoordinateSystem.YVector.Z = btlPlane.YAxis.Z;
            CoordinateSystem.ReferencePoint.X = btlPlane.OriginX;
            CoordinateSystem.ReferencePoint.Y = btlPlane.OriginY;
            CoordinateSystem.ReferencePoint.Z = btlPlane.OriginZ;

            ReferenceType Reference = new ReferenceType();
            Reference.Position = CoordinateSystem;

             


            BTLPart.Transformations.Transformation.Add(Reference);

            BTLPart.Length = length;
            BTLPart.Width = width;
            BTLPart.Height = height;

            BTLPartGeometry = new BTLPartGeometry(refSides, cornerPoints, endPoints, startPoints);
            


            /////////////////////////////////////////////////////////7



            //Create BTL-part
            // CREATE BTLPARTGEOMETRY
            //Assign
        }



        
    


        public void ManufactureSubElement(ManufactureMode _mode)               //PHASE 2: Manufacture
        {
            if (ready)
            {
                
                foreach (PerformTimberProcessDelegate Perform in PerformTimberProcesses)
                {
                    
                    PerformedProcess PerformedProcess = Perform(BTLPartGeometry, _mode);

                    if (_mode == ManufactureMode.BTL || _mode==ManufactureMode.BOTH)
                    {
                        BTLPart.Processings.Items.Add(PerformedProcess.BTLProcess);
                    }

                    if (_mode == ManufactureMode.NURBS || _mode == ManufactureMode.BOTH)
                    {
                        VoidProcess.Add(PerformedProcess.VoidProcess);
                        
                    }
                 
                }

                if (_mode == ManufactureMode.NURBS || _mode == ManufactureMode.BOTH)
                {
                    
                    Interval ix = new Interval(0, width);
                    Interval iy = new Interval(0, height);
                    Interval iz = new Interval(0, length);

                    Box boxstock = new Box(CornerPlane, ix, iy, iz);
                    Stock = Brep.CreateFromBox(boxstock);
                    List<Brep> boolBrep = new List<Brep>();
                    boolBrep.Add(Stock);

                        
                    if (VoidProcess.Count > 0)
                    {
                        if (true)
                        {
                            double tolerance = CommonProps.tolerances;
                            Rhino.Geometry.Brep[] breps = Rhino.Geometry.Brep.CreateBooleanDifference(boolBrep, VoidProcess, tolerance);
                            if (breps != null)
                                if (breps.Length != 0)
                                {
                                    ProcessedStock.AddRange(breps);
                                    
                                }
                            
                        }
                    }

                    else
                    {
                        ProcessedStock.Add(Stock);
                    }

                    

                    // ProcessedStock = BrepOperation....
                }

            }
            
        }
        

    }

    public class BTLPartGeometry
    {
        public List<Refside> Refsides { get; private set; }
        public List<Point3d> CornerPoints { get; private set; }
        public List<Point3d> Endpoints { get; private set; }
        public List<Point3d> StartPoints { get; private set; }

        public BTLPartGeometry(List<Refside> _refsides, List<Point3d> _cornerPoints, List<Point3d> _endpoints, List<Point3d> _startPoints)
        {
            Refsides = _refsides;
            CornerPoints = _cornerPoints;
            Endpoints = _endpoints;
            StartPoints = _startPoints;
        }
    } 


    public class PerformedProcess
    {
        public ProcessingType BTLProcess { get; private set; }
        public Brep VoidProcess { get; private set; }

        public PerformedProcess(ProcessingType _BTLProcess, Brep _VoidProcess)
        {
            BTLProcess = _BTLProcess;
            VoidProcess = _VoidProcess; 
        }

    }
     

    public class OrderedTimberProcess
    {
        
        public Element1D element;
        public PerformTimberProcessDelegate PerformTimberProcess { get; private set; }

        public OrderedTimberProcess(Element1D _element, PerformTimberProcessDelegate _performTimberProcess)
        {
            element = _element;
            PerformTimberProcess = _performTimberProcess;
        }

    }


    public class BuildingNode
    {
        // In the future, the geometrical node should be stored here. 
    }



}


       



        




    







    


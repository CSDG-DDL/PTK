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
        //public List<BuildingNode> BuildingNodes { get; private set; }
        private bool ready = false;

        public BuildingProject(ProjectType _btlProject)
        {
            BTLProject = _btlProject;
            BuildingElements = new List<BuildingElement>();
        }

        //Extract machining process to be adapted for each element
        //エレメントごとに適応されるべき加工プロセスを抽出
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

        //Set ManufactureMode for all building elements
        //全ビルディングエレメントにManufactureModeを設定
        public void ManufactureProject(ManufactureMode _mode)
        {
            List<PartType> AllParts = new List<PartType>();
            
            foreach (BuildingElement buildingElement in BuildingElements)
            {
                buildingElement.ManufactureElement(_mode);
                if (_mode == ManufactureMode.BTL || _mode == ManufactureMode.BOTH)
                {
                    AllParts.AddRange(buildingElement.BTLParts);
                    
 
                }
            }

            BTLProject.Parts = new ProjectTypeParts();

            BTLProject.Parts.Part = AllParts.ToArray();
            
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
                    if (Sub3DElement.ProcessedStock != null)
                    {
                        dataTree.AddRange(Sub3DElement.ProcessedStock, new Grasshopper.Kernel.Data.GH_Path(i, j));
                    }
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
        //public List<OrderedTimberProcess> OrderedTimberProcesseses { get; private set; }
        private bool ready = false;

        public BuildingElement(Element1D _element, List<PerformTimberProcessDelegate> _processDelegate)      //PHASE1: PREPAIR
        {
            Element = _element;
            Sub3DElements = new List<Sub3DElement>();
            BTLParts = new List<PartType>();
            ready = true;
            
            foreach (Sub2DElement subElement in Element.Sub2DElements)
            {
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
            ProcessedStock = new List<Brep>();
            VoidProcess = new List<Brep>();

            height = _Sub2DElement.CrossSection.GetHeight();
            width = _Sub2DElement.CrossSection.GetWidth();
            length = _element.BaseCurve.GetLength();

            BTLPart = new PartType();
            BTLPart.Height = height;
            BTLPart.Width = width;
            BTLPart.Length = length;
            



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
            Plane TempCorner1 = new Plane(SubElemPlaneCentric);
            TempCorner1.Translate(SubElemPlaneCentric.XAxis * -width / 2 + (SubElemPlaneCentric.YAxis * -height / 2));
            //TempCorner.Translate(TempCorner.XAxis * 100 + (TempCorner.YAxis * 100));
            CornerPlane = TempCorner1;

            Plane TempCorner2 = new Plane(SubElemPlaneCentric);
            TempCorner2.Translate(SubElemPlaneCentric.XAxis * width / 2 + (SubElemPlaneCentric.YAxis * -height / 2));

            Plane btlPlane = new Plane(TempCorner2.Origin, TempCorner2.ZAxis, CornerPlane.YAxis);

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

            refSides.Add(new Refside(1, refPlane1, length, width,height));
            refSides.Add(new Refside(2, refPlane2, length, height, width));
            refSides.Add(new Refside(3, refPlane3, length, width, height));
            refSides.Add(new Refside(4, refPlane4, length, height, width));

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

            CoordinateSystem.XVector = new CoordinateType();
            CoordinateSystem.YVector = new CoordinateType();
            CoordinateSystem.ReferencePoint = new PointType();


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
            

            Reference.GUID ="{"+ Convert.ToString(Guid.NewGuid())+"}";

            ReferenceType[] refe = new ReferenceType[1];



            refe[0] = Reference;
            BTLPart.Transformations = new ComponentTypeTransformations();


            BTLPart.Transformations.Transformation = refe;
   
            



            BTLPart.Width = width;
            BTLPart.Length = length;
            BTLPart.Height = height;
            BTLPart.StartOffset = 0.3;
            BTLPart.EndOffset = 0.3;


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
                List<ProcessingType> AllProcessings = new List<ProcessingType>();
                
                foreach (PerformTimberProcessDelegate Perform in PerformTimberProcesses)
                {
                    
                    PerformedProcess PerformedProcess = Perform(BTLPartGeometry, _mode);

                    if (_mode == ManufactureMode.BTL || _mode==ManufactureMode.BOTH)
                    {

                        AllProcessings.Add(PerformedProcess.BTLProcess);
                        
                    }

                    if (_mode == ManufactureMode.NURBS || _mode == ManufactureMode.BOTH)
                    {
                        VoidProcess.Add(PerformedProcess.VoidProcess);
                        
                    }
                 
                }

                BTLPart.Processings = new ComponentTypeProcessings();
                

                BTLPart.Processings.Items = AllProcessings.ToArray();

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


       



        




    







    


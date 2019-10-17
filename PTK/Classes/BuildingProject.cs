using Rhino.Geometry;
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
        public Job BVSJob { get; private set; }

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


        public DataTree<Brep> GetProcessedStock()
        {
            DataTree<Brep> dataTree = new DataTree<Brep>();
            
            for(int i = 0; i < BuildingElements.Count; i++)
            {
                BuildingElement BuildingElement = BuildingElements[i];
                for (int j = 0; j < BuildingElement.Sub3DElements.Count; j++)
                {
                    

                    ProcessedElement Sub3DElement = BuildingElement.Sub3DElements[j];
                    
                    if (Sub3DElement.ProcessedStock != null)
                    {
                        dataTree.AddRange(Sub3DElement.ProcessedStock, new Grasshopper.Kernel.Data.GH_Path(i, j));
                    }
                    

                }
            }
            return dataTree;
        }

        public DataTree<Brep> GetVoids()
        {
            DataTree<Brep> dataTree = new DataTree<Brep>();
            for (int i = 0; i < BuildingElements.Count; i++)
            {
                BuildingElement BuildingElement = BuildingElements[i];
                for (int j = 0; j < BuildingElement.Sub3DElements.Count; j++)
                {
                    ProcessedElement Sub3DElement = BuildingElement.Sub3DElements[j];
                    if (Sub3DElement.VoidProcess != null)
                    {
                        dataTree.AddRange(Sub3DElement.VoidProcess, new Grasshopper.Kernel.Data.GH_Path(i, j));
                    }
                }
            }
            return dataTree;
        }

        public DataTree<Brep> GetStock()
        {
            DataTree<Brep> dataTree = new DataTree<Brep>();
            for (int i = 0; i < BuildingElements.Count; i++)
            {
                BuildingElement BuildingElement = BuildingElements[i];
                for (int j = 0; j < BuildingElement.Sub3DElements.Count; j++)
                {
                    ProcessedElement Sub3DElement = BuildingElement.Sub3DElements[j];
                    if (Sub3DElement.Stock != null)
                    {
                        dataTree.Add(Sub3DElement.Stock, new Grasshopper.Kernel.Data.GH_Path(i, j));
                    }
                }
            }
            return dataTree;
        }

        public DataTree<Brep> GetProcessSurfaces()
        {
            DataTree<Brep> dataTree = new DataTree<Brep>();
            for (int i = 0; i < BuildingElements.Count; i++)
            {
                BuildingElement BuildingElement = BuildingElements[i];
                for (int j = 0; j < BuildingElement.Sub3DElements.Count; j++)
                {
                    ProcessedElement Sub3DElement = BuildingElement.Sub3DElements[j];
                    if (Sub3DElement.ProcessingSurfaces != null)
                    {
                        dataTree.AddRange(Sub3DElement.ProcessingSurfaces, new Grasshopper.Kernel.Data.GH_Path(i, j));
                    }
                }
            }
            return dataTree;
        }






    }

    
    public class BuildingElement
    {
        public Element1D Element { get; private set; }
        public List<ProcessedElement> Sub3DElements { get; private set; }
        public List<PartType> BTLParts;
        public List<Part> BVXParts;
        //public List<OrderedTimberProcess> OrderedTimberProcesseses { get; private set; }
        private bool ready = false;

        public BuildingElement(Element1D _element, List<PerformTimberProcessDelegate> _processDelegate)      //PHASE1: PREPAIR
        {
            Element = _element;
            Sub3DElements = new List<ProcessedElement>();
            BTLParts = new List<PartType>();
            ready = true;
            
            //if Element has CompositCrossSection => Branch?

            foreach(SubElement S in _element.Composite.Subelements)
            {
                Sub3DElements.Add(new ProcessedElement(S, _processDelegate));
            }

            
        }

        public void ManufactureElement(ManufactureMode _mode)      //PHASE 2: Manufacture
        {
            if (ready)
            {
                foreach(ProcessedElement subelem3D in Sub3DElements)
                {
                    subelem3D.ManufactureSubElement(_mode);

                    if (_mode == ManufactureMode.BTL || _mode == ManufactureMode.BOTH)
                    {
                        BTLParts.Add(subelem3D.BTLPart);
                        BVXParts.Add(subelem3D.BVXPart);
                    }
                }
            }
        }

    }


    public class ProcessedElement
    {
        public PartType BTLPart { get; private set; }                                           //Needed
        public Part BVXPart { get; private set; }
        public Brep Stock { get; private set; }  
        public List<Brep> ProcessedStock { get; private set; }
        public bool HaveErrors { get; private set; }
        public List<Brep> VoidProcess { get; private set; }
        public List<Brep> ProcessingSurfaces { get; private set; }
        public Plane CornerPlane { get; private set; }
        public Plane btlPlane { get; private set; }
        public double height{ get; private set; }
        public double width { get; private set; }
        public double length { get; private set; }

        public List<PerformTimberProcessDelegate> PerformTimberProcesses { get; private set; }  //Needed
        public BTLPartGeometry BTLPartGeometry { get; private set; }
        private bool ready = false;
        

        public ProcessedElement(SubElement _subElement, List<PerformTimberProcessDelegate> _processDelegate)     //PHASE1: PREPAIR
        {

            

            //Creating Parts

            //CREATING BTLX PART
            BTLPart = new PartType();



            ProcessedStock = new List<Brep>();
            VoidProcess = new List<Brep>();
            ProcessingSurfaces = new List<Brep>();

            height = _subElement.Height;
            width = _subElement.Width;
            length = _subElement.BaseCurve.GetLength();

            BTLPart = new PartType();
            BTLPart.Height = height;
            BTLPart.Width = width;
            BTLPart.Length = length;


            //Creating BVX PART

            BVXPart = new Part(width, height, length, _subElement.Name);




            CoordinateSystemType CoordinateSystem = new CoordinateSystemType();

            List<Point3d> startPoints = new List<Point3d>();
            List<Point3d> endPoints = new List<Point3d>();
            List<Point3d> cornerPoints = new List<Point3d>();

            ready = true;
            PerformTimberProcesses = _processDelegate;

            //Making BTL PLANE

            ////Move half height, half width
            //List<Refside> refSides = new List<Refside>();
            //Plane ElementPlaneCentric = _element.CroSecLocalPlane;

            //// Making plane centric to Sub-element
            //Plane SubElemPlaneCentric = new Plane(ElementPlaneCentric);
            //SubElemPlaneCentric.Translate(SubElemPlaneCentric.XAxis * _element.Alignment.OffsetY + SubElemPlaneCentric.YAxis * _element.Alignment.OffsetZ);

            ////Making CornerPlane, bottom left corner
            //Plane TempCorner1 = new Plane(SubElemPlaneCentric);
            //TempCorner1.Translate(SubElemPlaneCentric.XAxis * -width / 2 + (SubElemPlaneCentric.YAxis * -height / 2));
            //TempCorner1.Translate(SubElemPlaneCentric.XAxis * -_element.Alignment.OffsetY + (SubElemPlaneCentric.YAxis * -_element.Alignment.OffsetZ));



            //TempCorner.Translate(TempCorner.XAxis * 100 + (TempCorner.YAxis * 100));


            //Plane TempCorner2 = new Plane(SubElemPlaneCentric);
            //TempCorner2.Translate(SubElemPlaneCentric.XAxis * width / 2 + (SubElemPlaneCentric.YAxis * -height / 2));
            //TempCorner2.Translate(SubElemPlaneCentric.XAxis * -_element.Alignment.OffsetY + (SubElemPlaneCentric.YAxis * -_element.Alignment.OffsetZ));


            CornerPlane = _subElement.CrosSecLocalCornerPlane;
            Plane btlPlane = new Plane(CornerPlane.Origin, CornerPlane.ZAxis, CornerPlane.YAxis);
            btlPlane.Translate(-btlPlane.ZAxis * width);

            Plane refPlane1 = new Plane(btlPlane.Origin, btlPlane.XAxis, btlPlane.ZAxis);
            
            refPlane1.YAxis.Unitize();
            refPlane1.XAxis.Unitize();
            refPlane1.ZAxis.Unitize();

            Plane refPlane2 = new Plane(refPlane1.Origin, refPlane1.XAxis, refPlane1.ZAxis);
            Plane refPlane3 = new Plane(refPlane1.Origin, refPlane1.XAxis, -refPlane1.YAxis);
            Plane refPlane4 = new Plane(refPlane1.Origin, refPlane1.XAxis, -refPlane1.ZAxis);



            Vector3d WidthVector = new Vector3d(refPlane1.YAxis * width);
            Vector3d HeightVector = new Vector3d(-refPlane1.ZAxis * height);

            refPlane2.Translate(HeightVector);
            refPlane3.Translate(WidthVector + HeightVector);
            refPlane4.Translate(WidthVector);


            List<Refside> refSides = new List<Refside>();


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


            double ToMM = CommonFunctions.ConvertToMM();

            CoordinateSystem.XVector.X = btlPlane.XAxis.X;
            CoordinateSystem.XVector.Y = btlPlane.XAxis.Y;
            CoordinateSystem.XVector.Z = btlPlane.XAxis.Z;
            CoordinateSystem.YVector.X = btlPlane.YAxis.X;
            CoordinateSystem.YVector.Y = btlPlane.YAxis.Y;
            CoordinateSystem.YVector.Z = btlPlane.YAxis.Z;
            CoordinateSystem.ReferencePoint.X = btlPlane.OriginX * ToMM;
            CoordinateSystem.ReferencePoint.Y = btlPlane.OriginY * ToMM;
            CoordinateSystem.ReferencePoint.Z = btlPlane.OriginZ * ToMM;

            ReferenceType Reference = new ReferenceType();
            Reference.Position = CoordinateSystem;
            

            Reference.GUID ="{"+ Convert.ToString(Guid.NewGuid())+"}";

            ReferenceType[] refe = new ReferenceType[1];



            refe[0] = Reference;
            BTLPart.Transformations = new ComponentTypeTransformations();


            BTLPart.Transformations.Transformation = refe;
   
            



            BTLPart.Width = width * ToMM;
            BTLPart.Length = length * ToMM;
            BTLPart.Height = height * ToMM;
            BTLPart.StartOffset = 0;
            BTLPart.EndOffset = 0;


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
                List<Operations> BVXAllProcessings = new List<Operations>();
                
                foreach (PerformTimberProcessDelegate Perform in PerformTimberProcesses)
                {
                    
                    PerformedProcess PerformedProcess = Perform(BTLPartGeometry, _mode);

                    if (PerformedProcess != null)
                    {
                        if (_mode == ManufactureMode.BTL || _mode == ManufactureMode.BOTH)
                        {
                            if (PerformedProcess.BTLProcess.Name != "NotInUse")
                            {
                                AllProcessings.Add(PerformedProcess.BTLProcess);
                            }

                            //ADDING BVX
                            if (PerformedProcess.Operation!=null)
                            {
                                BVXAllProcessings.Add(PerformedProcess.Operation);
                            }

                            

                            


                            

                        }

                        if (_mode == ManufactureMode.NURBS || _mode == ManufactureMode.BOTH)
                        {
                            if (PerformedProcess.VoidProcess!=null && PerformedProcess.VoidProcess.GetVolume() > 1)
                            {
                                VoidProcess.Add(PerformedProcess.VoidProcess);
                            }


                            

                        }
                    }

                    
                 
                }


                if (AllProcessings.Count > 0)
                {
                    BTLPart.Processings = new ComponentTypeProcessings();
                    BVXPart.Operations = BVXAllProcessings;


                    BTLPart.Processings.Items = AllProcessings.ToArray();

                    
                }
                if (_mode == ManufactureMode.NURBS || _mode == ManufactureMode.BOTH)
                {

                    Interval ix = new Interval(0, width);
                    Interval iy = new Interval(0, height);
                    Interval iz = new Interval(0, length);

                    Point3d centerPt = CornerPlane.Origin;

                    centerPt = centerPt + CornerPlane.XAxis * width / 2 + CornerPlane.YAxis * height / 2 + CornerPlane.ZAxis * length / 2;

                    Box boxstock = new Box(CornerPlane, ix, iy, iz);
                    Stock = Brep.CreateFromBox(boxstock);
                    List<Brep> boolBrep = new List<Brep>();


                    foreach(Surface s in Stock.Surfaces)
                    {
                        s.SetUserString("old", "yes");
                    }

                    foreach(BrepFace f in Stock.Faces)
                    {
                        f.SetUserString("old", "yes");
                    }
                    foreach(Surface f in Stock.Surfaces)
                    {
                        f.SetUserString("old", "yes");
                    }



                    boolBrep.Add(Stock);


                    if (VoidProcess.Count > 0 && VoidProcess[0]!=null)
                    {
                        if (true)
                        {
                            double tolerance = CommonProps.tolerances;

                            

                            Rhino.Geometry.Brep[] breps = Rhino.Geometry.Brep.CreateBooleanDifference(boolBrep, VoidProcess, tolerance);

                            
                            
                            if (breps != null)
                            {
                                ProcessedStock.AddRange(breps);
                                
                                foreach (Brep brep in breps)
                                {
                                    Brep temp = brep.DuplicateBrep();

                                    for(int i = 0; i < brep.Faces.Count; i++)
                                    {
                                        //String value = brep.Faces[i].GetUserString("old");
                                        String value = brep.Surfaces[brep.Faces[i].SurfaceIndex].GetUserString("old");



                                        if (value != "yes")
                                        {
                                            ProcessingSurfaces.Add(temp.Faces.ExtractFace(i));
                                        }
                                    }
                                        
                                        
                                }

                                


                            }
                            if (breps == null || breps.Length==0)
                            {
                                
                                
                                


                                bool valid = true;
                                foreach (Brep b in VoidProcess)
                                {
                                    if (b.IsPointInside(centerPt, CommonProps.tolerances, true))
                                    {
                                        HaveErrors = true;
                                        valid = false;
                                        break;
                                    }
                                }
                                if (valid)
                                {
                                    ProcessedStock.Add(Stock);
                                    
                                }
                                
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
        public Operations Operation { get; private set; }

        public PerformedProcess(ProcessingType _BTLProcess, Brep _VoidProcess)
        {
            BTLProcess = _BTLProcess;
            VoidProcess = _VoidProcess; 
        }

        public PerformedProcess(ProcessingType _BTLProcess, Brep _VoidProcess, Operations _Operation)
        {
            BTLProcess = _BTLProcess;
            VoidProcess = _VoidProcess;
            Operation = _Operation;
        }


        public PerformedProcess()
        {
            
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


       



        




    







    


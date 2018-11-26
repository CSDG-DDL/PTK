﻿using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Grasshopper;
using Rhino.Geometry;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Runtime.Serialization;


namespace PTK.Components
{
    public class BuildModel : GH_Component
    {
        public BuildModel()
          : base("BuildModel", "Nickname",
              "Exporting BTL file to the designated location",
              CommonProps.category, CommonProps.subcate7)
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Assembly", "A", "", GH_ParamAccess.item);
            pManager.AddGenericParameter("Timber Processes", "P", "", GH_ParamAccess.list);
            pManager.AddTextParameter("Filepath", "", "", GH_ParamAccess.item);

            pManager[1].Optional = true;
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("", "", "", GH_ParamAccess.item);
            pManager.AddBrepParameter("Breps", "", "", GH_ParamAccess.tree);
            pManager.AddPlaneParameter("1", "", "", GH_ParamAccess.list);
            pManager.AddPlaneParameter("2", "", "", GH_ParamAccess.list);
            pManager.AddPlaneParameter("3", "", "", GH_ParamAccess.list);
            pManager.AddPlaneParameter("4", "", "", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // --- variables ---
            Assembly assembly = new Assembly();
            GH_Assembly ghAssembly = new GH_Assembly();
            List<OrderedTimberProcess> Orders = new List<OrderedTimberProcess>();

            string Name = "";
            int priorityKey = 0;
            string filepath = "";

            // --- input --- 
            DA.GetData(0, ref ghAssembly);
            DA.GetDataList(1, Orders);
            DA.GetData(2, ref filepath);

            // --- solve ---
            BuildingProject GrasshopperProject = new BuildingProject(new ProjectType());
            GrasshopperProject.PrepairElements(ghAssembly.Value.Elements, Orders);
            GrasshopperProject.ManufactureProject(ManufactureMode.BOTH);

            


            // Create a new XmlSerializer instance with the type of the test class
            //Initializing the project
            ProjectType Project = GrasshopperProject.BTLProject; 
            
            Project.Name = "PTK";
            Project.Architect = "JOHNBUNJIMarcin";
            Project.Comment = "YeaaaahhH! Finally. ";
            
            

            DataTree<Brep> dataTree = GrasshopperProject.GetBreps();

            //Initializing the file;

            BTLx BTLx = new BTLx();

            BTLx.Project = Project;
            BTLx.Language = "Norsk";
            
            
            


            // Create a new XmlSerializer instance with the type of the test class


            XmlSerializer SerializerObj = new XmlSerializer(typeof(BTLx));
            
            
            

            // Create a new file stream to write the serialized object to a file
            TextWriter WriteFileStream = new StreamWriter(filepath);

            
            SerializerObj.Serialize(WriteFileStream, BTLx);

            List<BuildingElement> Elements = GrasshopperProject.BuildingElements;

            List<Plane> ref1 = new List<Plane>();
            List<Plane> ref2 = new List<Plane>();
            List<Plane> ref3 = new List<Plane>();
            List<Plane> ref4 = new List<Plane>();

            foreach (BuildingElement Elems in Elements)
            {
                ref1.Add(Elems.Sub3DElements[0].BTLPartGeometry.Refsides[0].RefPlane);
                ref2.Add(Elems.Sub3DElements[0].BTLPartGeometry.Refsides[1].RefPlane);
                ref3.Add(Elems.Sub3DElements[0].BTLPartGeometry.Refsides[2].RefPlane);
                ref4.Add(Elems.Sub3DElements[0].BTLPartGeometry.Refsides[3].RefPlane);

            }
            

            // --- output ---
            DA.SetDataList(0, "Yeahhhh");
            DA.SetDataTree(1, dataTree);
            DA.SetDataList(2, ref1);
            DA.SetDataList(3, ref2);
            DA.SetDataList(4, ref3);
            DA.SetDataList(5, ref4);




        }

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return Properties.Resources.BTL;
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("c6bb5772-148b-4381-aad8-8161d8f5856f"); }
        }
    }
}
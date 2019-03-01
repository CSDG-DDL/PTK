using System;
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
            pManager.AddBrepParameter("Stock", "", "", GH_ParamAccess.tree);
            pManager.AddBrepParameter("Voids", "", "", GH_ParamAccess.tree);
            
            pManager.AddBrepParameter("ProcessingSurfaces", "", "", GH_ParamAccess.tree);
            pManager.AddBrepParameter("Processed Component", "", "", GH_ParamAccess.tree);
            pManager.AddBrepParameter("test", "", "", GH_ParamAccess.item);
            

            pManager.HideParameter(0);
            pManager.HideParameter(1);
            pManager.HideParameter(2);


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
            
            Project.Name = "ReindeerProject";
            Project.Architect = "DDL-CSDG";
            Project.Comment = "BetaTest";


            DataTree<Brep> Stock = GrasshopperProject.GetStock();
            DataTree<Brep> Voids = GrasshopperProject.GetVoids();
            DataTree<Brep> ProcessedStock = GrasshopperProject.GetProcessedStock();
            DataTree<Brep> ProcessingSurfaces = GrasshopperProject.GetProcessSurfaces();

 
            

           
            



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

            


            

            // --- output ---
            DA.SetDataTree(0, Stock);
            DA.SetDataTree(1, Voids);
            DA.SetDataTree(2, ProcessingSurfaces);
            DA.SetDataTree(3, ProcessedStock);


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
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
          : base("ProcessModel", "PM",
              "This component lets process the  timber processings(cut/drill/custom/etc) and output NURBS. Additonally, A BTLX file is saved. This file can be used by most manufacturers",
              CommonProps.category, CommonProps.subcate11)
        {
        }

        /// <summary>
        /// Overrides the exposure level in the components category 
        /// </summary>
        public override GH_Exposure Exposure
        {
            get
            { return GH_Exposure.secondary; }
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Assembly", "A", "", GH_ParamAccess.item);
            pManager.AddGenericParameter("Timber Processes", "P", "", GH_ParamAccess.list);
            pManager.AddBooleanParameter("Run BTLX?", "->", "Run BTLX processing? ", GH_ParamAccess.item, true);
            pManager.AddBooleanParameter("Run NURBS?", "->", "Run NURBS processing? Might take minutes if complicated geometry. BOOLEAN ERRORS MAY OCCUR", GH_ParamAccess.item, true);
            pManager.AddGenericParameter("BTLX-setting", "x", "Use the BTLXSetting component to specify project/folder details", GH_ParamAccess.item);
            

            pManager[1].Optional = true;
            pManager[2].Optional = true;
            pManager[3].Optional = true;
            pManager[4].Optional = true;

            pManager[1].DataMapping = GH_DataMapping.Flatten;

        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddBrepParameter("Stock", "", "", GH_ParamAccess.tree);
            pManager.AddBrepParameter("Voids", "", "", GH_ParamAccess.tree);
            
            pManager.AddBrepParameter("ProcessingSurfaces", "", "", GH_ParamAccess.tree);
            pManager.AddBrepParameter("Processed Component", "", "", GH_ParamAccess.tree);
            
            

            pManager.HideParameter(0);
            pManager.HideParameter(1);
            pManager.HideParameter(2);


        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {

            DataTree<Brep> Stock = new DataTree<Brep>();
            DataTree<Brep> Voids = new DataTree<Brep>();
            DataTree<Brep> ProcessedStock = new DataTree<Brep>();
            DataTree<Brep> ProcessingSurfaces = new DataTree<Brep>();


            bool BTLXrun = true;
            bool NURBSrun = true;

            DA.GetData(2, ref BTLXrun);
            DA.GetData(3, ref NURBSrun);

            ManufactureMode mode = ManufactureMode.NONE;


            if (BTLXrun) { mode = ManufactureMode.BTL; }
            if (NURBSrun) { mode = ManufactureMode.NURBS; }
            if (NURBSrun && BTLXrun) { mode = ManufactureMode.BOTH; }


            if (BTLXrun | NURBSrun)
            {
                // --- variables ---
                Assembly assembly = new Assembly();
                GH_Assembly ghAssembly = new GH_Assembly();
                List<OrderedTimberProcess> Orders = new List<OrderedTimberProcess>();
                ProjectType btlxInput = new ProjectType();

                string Name = "";
                int priorityKey = 0;
                string filepath = "";


                // --- input --- 
                DA.GetData(0, ref ghAssembly);
                DA.GetDataList(1, Orders);
                if (!DA.GetData(4, ref btlxInput))
                {
                    btlxInput.Architect = "CSDG-NikkenDDL";
                    btlxInput.Comment = @"C:\Users\Lokaladm\Desktop\PROJECTFOLDER";
                    btlxInput.Name = "NoNamedProject";

                    
                    

                }


                // --- solve ---
                BuildingProject GrasshopperProject = new BuildingProject(new ProjectType());
                GrasshopperProject.PrepairElements(ghAssembly.Value.Elements, Orders);
                GrasshopperProject.ManufactureProject(mode);




                // Create a new XmlSerializer instance with the type of the test class
                //Initializing the project





                

                ProjectType Project = GrasshopperProject.BTLProject;

                Project.Name = btlxInput.Name;
                Project.Architect = btlxInput.Architect;
                Project.DeliveryDate = btlxInput.DeliveryDate;
                Project.Customer = btlxInput.Customer;
                Project.Comment = "BetaTest. THIS FILE MAY LACK PROCESSINGS OR CONTAIN ERRORS! ";

                Job Job = GrasshopperProject.BVSJob;



                //Initializing the file;

                BTLx BTLx = new BTLx();



                BTLx.Project = Project;
                

                


                // Create a new XmlSerializer instance with the type of the test class


                XmlSerializer SerializerObj = new XmlSerializer(typeof(BTLx));

                if (btlxInput.Comment[btlxInput.Comment.Length-1].Equals(@"/"))
                {
                    filepath = btlxInput.Comment + BTLx.Project.Name + ".btlx";
                }
                else
                {
                    filepath = btlxInput.Comment + @"\" + BTLx.Project.Name + ".btlx";
                }

                

                // Create a new file stream to write the serialized object to a file
                TextWriter WriteFileStream = new StreamWriter(@filepath);



                SerializerObj.Serialize(WriteFileStream, BTLx);

                WriteFileStream.Dispose();

                Stock = GrasshopperProject.GetStock();
                Voids = GrasshopperProject.GetVoids();

                
                ProcessedStock = GrasshopperProject.GetProcessedStock();

                

                ProcessingSurfaces = GrasshopperProject.GetProcessSurfaces();
            }

            




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
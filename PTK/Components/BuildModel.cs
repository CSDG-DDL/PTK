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
        /// <summary>
        /// Initializes a new instance of the BuildModel class.
        /// </summary>
        public BuildModel()
          : base("BuildModel", "Nickname",
              "Description",
              "Category", "Subcategory")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Assembly", "A", "", GH_ParamAccess.item);
            pManager.AddGenericParameter("Timber Processes", "P", "", GH_ParamAccess.list);
            pManager.AddTextParameter("Filepath", "", "", GH_ParamAccess.item);

            pManager[1].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("", "", "", GH_ParamAccess.item);
            pManager.AddBrepParameter("Breps", "", "", GH_ParamAccess.tree);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Assembly assembly = new Assembly();
            GH_Assembly ghAssembly = new GH_Assembly();

            List<OrderedTimberProcess> Orders = new List<OrderedTimberProcess>();

            string Name = "";
            int priorityKey = 0;
            string filepath = "";



            DA.GetData(0, ref ghAssembly);
            DA.GetDataList(1, Orders);
            DA.GetData(2, ref filepath);


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
            WriteFileStream.Close();

            DA.SetDataList(0, "Yeahhhh");
            DA.SetDataTree(1, dataTree);

        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
                return null;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("c6bb5772-148b-4381-aad8-8161d8f5856f"); }
        }
    }
}
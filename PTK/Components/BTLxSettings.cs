using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace PTK.Components
{
    public class BTLxSettings : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the BTLxSettings class.
        /// </summary>
        public BTLxSettings()
          : base("BTLxSettings", "X",
              "BTLx project settings",
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

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Project/File name", "N", "Project/File Name", GH_ParamAccess.item,"NoNameProject");
            pManager.AddTextParameter("Folder location", "F", "Folder location to save the btlx file", GH_ParamAccess.item, @"C:\Users\Lokaladm\Desktop\PROJECTFOLDER");
            pManager.AddTextParameter("Architect", "A", "Name of designer/architect/structural engineer", GH_ParamAccess.item,"CSDG-NIKKEN DDL");
            pManager.AddTextParameter("Customer", "N", "Project/File Name", GH_ParamAccess.item,"");
            

        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("BTLX Setting", "x", "BTLX setting to connect to ProcessModel", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string name = "";
            string folderLoc = "";
            string architect = "";
            string customer = "";
            




            if (!DA.GetData(0, ref name)) ;
            if (!DA.GetData(1, ref folderLoc)) ;
            if (!DA.GetData(2, ref architect)) ;
            if (!DA.GetData(3, ref customer)) ;
            


            ProjectType Project = new ProjectType
            {
                Name = name,
                Architect = architect,
                Comment = folderLoc,
                Customer = customer,
                
            };



           

            DA.SetData(0, Project);





        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return Properties.Resources.BTL;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("efd8eede-ad18-49db-84e9-ffeff9e8ef1e"); }
        }
    }
}
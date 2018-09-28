using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Runtime.Serialization;

namespace PTK
{
    public class PTK_7_BtlExport : GH_Component
    {
        public PTK_7_BtlExport()
          : base("BTL EXPORTER (PTK)", "Export BTL",
              "Exporting BTL file to the designated location",
              CommonProps.category, CommonProps.subcate7)
        {
            Message = CommonProps.initialMessage;
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("PTK INPUT", "PTK IN", "PTK DATA INPUT", GH_ParamAccess.item);
            pManager.AddGenericParameter("BTL-processes", "", "", GH_ParamAccess.list);
            pManager.AddTextParameter("FILE LOCATION", "Folder", "Folder LOCATION OF EXPORTED BTL FILE", GH_ParamAccess.item);
            pManager.AddBooleanParameter("ENABLE?", "ENABLE?", "ENABLE EXPORTING?", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddBrepParameter("Result", "", "", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            /*
            bool enable = false;
            Assembly assembly = new Assembly();
            List<BTLprocess> Processes = new List<BTLprocess>();

            string filepath = "";


            DA.GetData(0, ref assembly);
            DA.GetDataList(1, Processes);
            DA.GetData(2, ref filepath);
            DA.GetData(3, ref enable);
            filepath += @"\Test.btlx";

            if (enable)
            {
                //Initializing the parts
                ProjectTypeParts Parts = new ProjectTypeParts();



                foreach (BTLprocess process in Processes)
                {

                    // assembly.Elements.Find(t => t.Id == Convert.ToInt16(process.ElemId)).SubElementBTL[0].BTLPart.Processings.Items.Add(process.Process);  //Adding processess in correct btl part
                    // assembly.Elements.Find(t => t.Id == Convert.ToInt16(process.ElemId)).SubElementBTL[0].BTLProcesses.Add(process);

                }


                List<Brep> allBreps = new List<Brep>();

                
                for (int i = 0; i < assembly.Elements.Count; i++)
                {
                    Parts.Part.Add(assembly.Elements[i].SubElementBTL[0].BTLPart);  //Adding part to parts for each element. Line 73 have included all processess

                    List<BTLprocess> BTLProcessess = assembly.Elements[i].SubElementBTL[0].BTLProcesses;
                    List<Brep> voids = new List<Brep>();
                    List<Brep> keep = new List<Brep>();
                    keep.Add(assembly.Elements[i].ElementGeometry);

                    if (BTLProcessess.Count > 0)
                    {
                        for (int j = 0; j < BTLProcessess.Count; j++)
                        {
                            voids.Add(BTLProcessess[j].Voidgeometry);
                        }

                        double tolerance = 0.1;
                        Rhino.Geometry.Brep[] breps = Rhino.Geometry.Brep.CreateBooleanDifference(keep, voids, tolerance);
                        if (breps != null || breps.Length == 0)
                        {
                            for (int j = 0; j < breps.Length; j++)
                            {
                                allBreps.Add(breps[j]);
                            }
                        }
                    }

                    else
                    {
                        allBreps.AddRange(keep);
                    }


                }
                

                //Initializing the project
                ProjectType Project = new ProjectType();
                Project.Parts = Parts;
                Project.Name = "PTK";
                Project.Architect = "JOHNBUNJIMarcin";
                Project.Comment = "YeaaaahhH! Finally. ";




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

                DA.SetDataList(0, allBreps);
                
            
            }
            */
        }

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return PTK.Properties.Resources.BTL;

            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("a638c80f-bcd6-4ecd-a075-9dc9a9c73a98"); }
        }
    }
}
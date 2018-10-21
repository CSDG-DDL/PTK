
using GH_IO.Serialization;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;

using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;

using System.Xml;
using System.Xml.Xsl;
using System.Xml.Serialization;
using System.IO;

using Karamba.Models;
using Karamba.Elements;



namespace PTK.Components
{
    public class PTK_UtilizationReport : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the _4_4_DimensioningMembers class.
        /// </summary>
        public PTK_UtilizationReport()
          : base("Utilization report", "Timber EC 5 utilization report",
              "Report utilization according to the EC5",
              CommonProps.category, CommonProps.subcate5)
        {
            Message = CommonProps.initialMessage;
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {

            pManager.AddParameter(new Param_StructuralAssembly(), "Structural Assembly", "SA", "Structural Assembly", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Produce report", "R (PTK)", "Export to xml the structural report", GH_ParamAccess.item, false);
            pManager.AddTextParameter("Folder path", "R (PTK)", "Folder path", GH_ParamAccess.item, @"C:\Users\marcinl\Desktop\Nikken_C#\report.xml");
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {

            pManager.AddTextParameter("OUT information", "info", "temporary information from analysis", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {

            #region variables
            string filepath = @"C:\Users\marcinl\Desktop\Nikken_C#\report.xml";
            bool boolstart = false;

            GH_StructuralAssembly gStrAssembly = null;
            StructuralAssembly structuralAssembly = null;
            
            List<PTK_StructuralAnalysis> report_list = new List<PTK_StructuralAnalysis>();

            List<string> infolist = new List<string>();
            #endregion

            #region input
            if (!DA.GetData(0, ref gStrAssembly)) { return; }
            if (!DA.GetData(2, ref filepath)) { return; }
            if (!DA.GetData(1, ref boolstart)) { return; }


            structuralAssembly = gStrAssembly.Value;
            
            #endregion

            infolist.Add("The preview of the structural analysis version 0.5");
            
            
            foreach (var e1 in structuralAssembly.Assembly.Elements)
            {

                report_list.Add(structuralAssembly.ElementReport[e1]);
            }
           
            foreach (var i1 in report_list)
            {
                foreach (System.Reflection.PropertyInfo prop in typeof(PTK_StructuralAnalysis).GetProperties())
                {
                    infolist.Add(String.Format("{0} = {1}", prop.Name, prop.GetValue(i1, null)));
                }



            }
            
            #region creating report of calculation
            // Create a new XmlSerializer instance with the type of the test class
            if (boolstart == true)
            {

                var reportToXML = new System.Xml.Linq.XElement(
                    "Utilizationreport",
                    from reports in report_list

                    select new System.Xml.Linq.XElement(
                        "reports",
                        // input data
                        new System.Xml.Linq.XElement("elemenentId", reports.elementID),
                        new System.Xml.Linq.XElement("Length", reports.elementLength),
                        new System.Xml.Linq.XElement("Height", reports.elementHeight),
                        new System.Xml.Linq.XElement("Width", reports.elementWidth),
                        new System.Xml.Linq.XElement("Area", reports.elementArea),
                        new System.Xml.Linq.XElement("I1", reports.elementMomentOfInertiaDir1),
                        new System.Xml.Linq.XElement("I2", reports.elementMomentOfInertiaDir2),
                        new System.Xml.Linq.XElement("R1", reports.elementRadiusOfGyrationDir1),
                        new System.Xml.Linq.XElement("R2", reports.elementRadiusOfGyrationDir2),
                        new System.Xml.Linq.XElement("F1", reports.elementAxialForce),
                        new System.Xml.Linq.XElement("F2", reports.elementShearForce1),
                        new System.Xml.Linq.XElement("F3", reports.elementShearForce2),
                        new System.Xml.Linq.XElement("M1", reports.elementTorsion),
                        new System.Xml.Linq.XElement("M2", reports.elementBending1),
                        new System.Xml.Linq.XElement("M3", reports.elementBending2),
                        // parameters
                        new System.Xml.Linq.XElement("effLengthDir1", reports.elementEffectiveLengthDir1),
                        new System.Xml.Linq.XElement("effLengthDir2", reports.elementEffectiveLengthDir2),
                        new System.Xml.Linq.XElement("slendernessDir1", reports.elementSlendernessRatioDir1),
                        new System.Xml.Linq.XElement("slendernessDir2", reports.elementSlendernessRatioDir2),
                        new System.Xml.Linq.XElement("eulerForceDir1", reports.elementEulerForceDir1),
                        new System.Xml.Linq.XElement("eulerForceDir1", reports.elementEulerForceDir2),
                        new System.Xml.Linq.XElement("relativeSlendernessDir1", reports.elementSlendernessRelativeDir1),
                        new System.Xml.Linq.XElement("relativeSlendernessDir2", reports.elementSlendernessRelativeDir2),
                        new System.Xml.Linq.XElement("instabilityFactorDir1", reports.elementInstabilityFactorDir1),
                        new System.Xml.Linq.XElement("instabilityFactorDir2", reports.elementInstabilityFactorDir2),
                        new System.Xml.Linq.XElement("bucklingStrengthDir1", reports.elementBucklingStrengthDir1),
                        new System.Xml.Linq.XElement("bucklingStrengthDir2", reports.elementBucklingStrengthDir2),
                        // results
                        new System.Xml.Linq.XElement("utilizationCompression", reports.elementCompressionUtilization),
                        new System.Xml.Linq.XElement("utilizationCompressionAngle", reports.elementCompressionUtilizationAngle),
                        new System.Xml.Linq.XElement("utilizationTension", reports.elementTensionUtilization),
                        new System.Xml.Linq.XElement("utilizationBending", reports.elementBendingUtilization),
                        new System.Xml.Linq.XElement("utilizationCombined", reports.elementCombinedBendingAndAxial)


                        )
                        );

                var doc = new System.Xml.Linq.XDocument(reportToXML);
                doc.Save(filepath);


                // Create a new file stream to write the serialized object to a file


            }

            #endregion





            DA.SetDataList(0, infolist);
        }

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
                return PTK.Properties.Resources.LocalAnalysis;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("6632ee12-b0e1-4125-d1f8-94d01ab78212"); }
        }
    }
}



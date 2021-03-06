﻿
using GH_IO.Serialization;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;

using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;

using System.Xml;
using System.Xml.Xsl;
using System.Xml.Serialization;
using System.IO;

using Karamba.Models;
using Karamba.Elements;



namespace PTK.Components
{
    public class PTK_UtilizationPreview : GH_Component
    {

        List<Brep> PublicElements = new List<Brep>();
        List<Color> PublicColors = new List<Color>();

        Dictionary<Brep, Color> models = new Dictionary<Brep, Color>();
        /// <summary>
        /// Initializes a new instance of the _4_4_DimensioningMembers class.
        /// </summary>
        public PTK_UtilizationPreview()
          : base("Utilization preview", "Timber EC 5 utilization preview",
              "Preview utilization according to the EC5",
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
            pManager.AddBooleanParameter("Show the legend", "R (PTK)", "Highlight the utilization level", GH_ParamAccess.item, false);
            pManager.AddNumberParameter("Range of utilization", "R (PTK)", "Range of utilization", GH_ParamAccess.list, new List<double>() {0,0.5,1 });
            pManager.AddTextParameter("Colors of utilization", "R (PTK)", "Colors of utilization", GH_ParamAccess.list, new List<string>() {"green","yellow","red" });
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("OUT information", "info", "temporary information from analysis", GH_ParamAccess.list);
            pManager.AddGenericParameter("Model", "M", "3d model", GH_ParamAccess.list);
        }
        
        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {

            #region variables
            bool boolstart = false;

            List<double> utilList = new List<double>();
            List<string> colorList = new List<string>();

            GH_StructuralAssembly gStrAssembly = null;
            StructuralAssembly structuralAssembly = null;

            List<string> infolist = new List<string>();
            #endregion

            // --- variables --- from YUTO
            GH_Element1D gElement = null;
            Dictionary<Brep, Color> tmpModels = new Dictionary<Brep, Color>();
            Dictionary<Curve, Color> sectionCurves = new Dictionary<Curve, Color>();

            #region input
            if (!DA.GetData(0, ref gStrAssembly)) { return; }
            if (!DA.GetData(1, ref boolstart)) { return; }
            if (!DA.GetDataList(2, utilList)) { return; }
            if (!DA.GetDataList(3, colorList)) { return; }

            structuralAssembly = gStrAssembly.Value ;

            List<Element1D> elements = structuralAssembly.Elements ;
            List<Color> cList = new List<Color>();

            foreach (var c1 in colorList)
            {
                cList.Add(Color.FromName(c1));
            }
            #endregion
            
            #region creating report of calculation
            // Creat
            List<double> tmpUtilList = new List<double>();
            List<Brep> brepList = new List<Brep>();

            Dictionary<Element1D, double> utiliziationDictionary = new Dictionary<Element1D, double>();
            Dictionary<double, double> utilizationColor = new Dictionary<double, double>();

            

            if (boolstart == true)
            {
                infolist.Add("The algorithm checking utilization is running");
                int c1 = 0;
                foreach (var u1 in utilList)
                {
                    infolist.Add("util range to=" + u1 + " color is=" + colorList[c1]);
                    c1++;
                }
                int i1 = 0;

                foreach (var e1 in structuralAssembly.Elements)
                {
                    tmpUtilList = new List<double>() {
                        e1.StructuralData.StructuralResults.CompressionUtilization,
                        e1.StructuralData.StructuralResults.TensionUtilization,
                        e1.StructuralData.StructuralResults.CompressionUtilizationAngle,
                        e1.StructuralData.StructuralResults.CombinedBendingAndAxial,
                        e1.StructuralData.StructuralResults.BendingUtilization
                        };
                   
                    utiliziationDictionary.Add(e1, tmpUtilList.Max());
                    infolist.Add("for element=" + i1 + " maximum utilization is=" + tmpUtilList.Max());
                    i1++;
                    
                }
            }

            #endregion

            #region creating breps
            List<Brep> BrepElements = new List<Brep>();
            List<Color> Colors = new List<Color>();

            foreach (var element in elements)
            {
                BrepElements.Add( element.GenerateSimplifiedGeometry());

                Color tmpColor = Color.White;
                double tmpUtil = utiliziationDictionary[element];

                if (tmpUtil > utilList[utilList.Count - 1])
                {
                    tmpColor = cList[utilList.Count - 1];
                }
                else
                {
                    tmpColor = Color.White;
                    for (int i1 = 0; i1 < utilList.Count - 1; i1++)
                    {
                        if (tmpUtil >= utilList[i1] && tmpUtil < utilList[i1 + 1])
                        {
                            tmpColor = cList[i1];
                        }
                    }
                };
                Colors.Add(tmpColor);
                
            }
            #endregion 

            infolist.Add("The preview of the structural analysis version 0.5");
            
            DA.SetDataList(1, BrepElements);
            DA.SetDataList(0, infolist);

            PublicColors = Colors;
            PublicElements = BrepElements;

        }

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
                return PTK.Properties.Resources.LocalAnalysisCheck;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("6632ee12-b0e1-4005-8df8-94d01ab78212"); }
        }

        public override void ExpireSolution(bool recompute)
        {
            PublicElements.Clear();
            PublicColors.Clear();
            models.Clear();
            base.ExpireSolution(recompute);
        }

        //public override BoundingBox ClippingBox => models.Keys.ToList()[0].GetBoundingBox(false);
        public override void DrawViewportMeshes(IGH_PreviewArgs args)
        {
            for(int i =0;i<PublicColors.Count;i++)
            {
                //args.Display.DrawObject(m.Key, new Rhino.Display.DisplayMaterial(m.Value, 0.5));
                args.Display.DrawBrepShaded(PublicElements[i], new Rhino.Display.DisplayMaterial(PublicColors[i]));
            }

        }
        public override void DrawViewportWires(IGH_PreviewArgs args)
        {

        }
    }
}



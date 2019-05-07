using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace PTK
{
    public class PTK_UtilizationCheck : GH_Component
    {
        public PTK_UtilizationCheck()
          : base("Utilization check", "Timber EC 5 utilization",
              "Utilization according the EC5",
              CommonProps.category, CommonProps.subcate5)
        {
            Message = CommonProps.initialMessage;
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddParameter(new Param_StructuralAssembly(), "Structural Assembly", "SA", "Structural Assembly", GH_ParamAccess.item);

        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("PTK Report", "R (PTK)", "Structural analysis report", GH_ParamAccess.item);
            pManager.AddTextParameter("OUT information", "info", "temporary information from analysis", GH_ParamAccess.tree);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<string> check1 = new List<string>();
            // --- variables ---
            string singular_system_msg = "singular stiffness matrix";
            GH_StructuralAssembly gStrAssembly = null;
            StructuralAssembly structuralAssembly = null;
            Grasshopper.Kernel.Data.GH_Structure<Grasshopper.Kernel.Types.GH_String> infoTree = new Grasshopper.Kernel.Data.GH_Structure<Grasshopper.Kernel.Types.GH_String>();
            Grasshopper.Kernel.Types.GH_String s1 = new Grasshopper.Kernel.Types.GH_String("The data tree");
            List<string> infolist = new List<string>();
            infolist.Add("Start of algorithm");
            string warning;

            // --- input --- 
            if (!DA.GetData(0, ref gStrAssembly)) { return; }
            structuralAssembly = gStrAssembly.Value;

            // --- solve ---

            // effective length variables
            double tmpefflength_dir1;
            double tmpefflength_dir2;

            // slenderness variables
            double tmpslender_dir1;
            double tmpslender_dir2;

            // euler force variables
            double tmp_euler_dir1;
            double tmp_euler_dir2;

            //relative  slenderness variables
            double tmp_rel_slender_dir1;
            double tmp_rel_slender_dir2;

            //instability factor variables
            double tmp_ins_factor_dir1;
            double tmp_ins_factor_dir2;

            //instability factor variables
            double tmp_buc_strength_dir1;
            double tmp_buc_strength_dir2;

            //instability factor variables
            double tmp_util_compression;

            List<PTK_StructuralAnalysis> report_list = new List<PTK_StructuralAnalysis>();
            int indexReport = 0;
            /// Loop over elements
            /// 

            infolist.Add("The utilization checking according EC5, each element data is stored in new data branch");

            foreach (var e1 in structuralAssembly.Elements)
            {
                PTK_StructuralAnalysis element_report = new PTK_StructuralAnalysis(indexReport);
                indexReport = indexReport + 1;
                infolist.Add("indexreport=" + indexReport);
                infoTree.Append(new Grasshopper.Kernel.Types.GH_String("report for element number=" + indexReport),new Grasshopper.Kernel.Data.GH_Path(indexReport));

                element_report.elementLength = e1.BaseCurve.GetLength();
                double w;
                double h;
                h = e1.Composite.HeightSimplified;
                w = e1.Composite.WidthSimplified;
                element_report.elementWidth = w;
                element_report.elementHeight = h;

                infoTree.Append(new Grasshopper.Kernel.Types.GH_String("Length [mm]=" + element_report.elementLength));
                infoTree.Append(new Grasshopper.Kernel.Types.GH_String("Width [mm]=" + element_report.elementWidth));
                infoTree.Append(new Grasshopper.Kernel.Types.GH_String("Height [mm]=" + element_report.elementHeight));

                element_report.elementEffectiveLengthDir1 = e1.StructuralData.effectiveLength1;
                element_report.elementEffectiveLengthDir2 = e1.StructuralData.effectiveLength2;

                infoTree.Append(new Grasshopper.Kernel.Types.GH_String("EffectiveLengthDir1 [mm]=" + element_report.elementEffectiveLengthDir1));
                infoTree.Append(new Grasshopper.Kernel.Types.GH_String("EffectiveLengthDir2 [mm]=" + element_report.elementEffectiveLengthDir2));

                element_report.elementSlendernessRatioDir1 = e1.StructuralData.slendernessRatio1;
                element_report.elementSlendernessRatioDir2 = e1.StructuralData.slendernessRatio2;

                infoTree.Append(new Grasshopper.Kernel.Types.GH_String("SlendernessRatioDir1=" + element_report.elementSlendernessRatioDir1));
                infoTree.Append(new Grasshopper.Kernel.Types.GH_String("SlendernessRatioDir2=" + element_report.elementSlendernessRatioDir2));

                element_report.elementEulerForceDir1 = e1.StructuralData.eulerForce1;
                element_report.elementEulerForceDir2 = e1.StructuralData.eulerForce2;

                infoTree.Append(new Grasshopper.Kernel.Types.GH_String("EulerForceDir1=" + element_report.elementEulerForceDir1));
                infoTree.Append(new Grasshopper.Kernel.Types.GH_String("EulerForceDir2=" + element_report.elementEulerForceDir2));

                element_report.elementSlendernessRatioDir1 = e1.StructuralData.slendernessRelative1;
                element_report.elementSlendernessRatioDir2 = e1.StructuralData.slendernessRelative2;

                infoTree.Append(new Grasshopper.Kernel.Types.GH_String("SlendernessRatioDir1=" + element_report.elementSlendernessRatioDir1));
                infoTree.Append(new Grasshopper.Kernel.Types.GH_String("SlendernessRatioDir2=" + element_report.elementSlendernessRatioDir2));

                element_report.elementInstabilityFactorDir1 = e1.StructuralData.instabilityFactor1;
                element_report.elementInstabilityFactorDir2 = e1.StructuralData.instabilityFactor2;

                infoTree.Append(new Grasshopper.Kernel.Types.GH_String("InstabilityFactorDir1=" + element_report.elementInstabilityFactorDir1));
                infoTree.Append(new Grasshopper.Kernel.Types.GH_String("InstabilityFactorDir2=" + element_report.elementInstabilityFactorDir2));

                element_report.elementBucklingStrengthDir1 = e1.StructuralData.BucklingStrength1;
                element_report.elementBucklingStrengthDir2 = e1.StructuralData.BucklingStrength2;

                infoTree.Append(new Grasshopper.Kernel.Types.GH_String("BucklingStrengthDir1=" + element_report.elementBucklingStrengthDir1));
                infoTree.Append(new Grasshopper.Kernel.Types.GH_String("BucklingStrengthDir2=" + element_report.elementBucklingStrengthDir2));

                infoTree.Append(new Grasshopper.Kernel.Types.GH_String("The compression [N]=" + e1.StructuralData.StructuralForces.maxCompressionForce.FX));
                infoTree.Append(new Grasshopper.Kernel.Types.GH_String("The tension [N]=" + e1.StructuralData.StructuralForces.maxTensionForce.FX));
                infoTree.Append(new Grasshopper.Kernel.Types.GH_String("The shear dir1 [N]=" + e1.StructuralData.StructuralForces.maxShearDir1.FY));
                infoTree.Append(new Grasshopper.Kernel.Types.GH_String("The shear dir2 [N]=" + e1.StructuralData.StructuralForces.maxShearDir2.FZ));
                infoTree.Append(new Grasshopper.Kernel.Types.GH_String("The torsion [Nmm]=" + e1.StructuralData.StructuralForces.maxTorsion.MX));
                infoTree.Append(new Grasshopper.Kernel.Types.GH_String("The bending dir1 [Nmm]=" + e1.StructuralData.StructuralForces.maxBendingDir1.MY));
                infoTree.Append(new Grasshopper.Kernel.Types.GH_String("The bending dir2 [Nmm]=" + e1.StructuralData.StructuralForces.maxBendingDir2.MZ));

                element_report.elementCompressionUtilization = e1.StructuralData.StructuralResults.CompressionUtilization;
                element_report.elementCompressionUtilizationAngle = e1.StructuralData.StructuralResults.CompressionUtilizationAngle;
                element_report.elementTensionUtilization = e1.StructuralData.StructuralResults.TensionUtilization;
                element_report.elementBendingUtilization = e1.StructuralData.StructuralResults.BendingUtilization;
                element_report.elementCombinedBendingAndAxial = e1.StructuralData.StructuralResults.CombinedBendingAndAxial;

                infoTree.Append(new Grasshopper.Kernel.Types.GH_String("The compression utilization =" + element_report.elementCompressionUtilization));
                infoTree.Append(new Grasshopper.Kernel.Types.GH_String("The compression utilizataion (grain angle)=" + element_report.elementCompressionUtilizationAngle));
                infoTree.Append(new Grasshopper.Kernel.Types.GH_String("The tension utilization =" + element_report.elementTensionUtilization));
                infoTree.Append(new Grasshopper.Kernel.Types.GH_String("The bending utilization=" + element_report.elementBendingUtilization));
                infoTree.Append(new Grasshopper.Kernel.Types.GH_String("The combined utilization =" + element_report.elementCombinedBendingAndAxial));



                var list_of_utilizations = new List<double>() {
                    element_report.elementCompressionUtilization,
                    element_report.elementTensionUtilization,
                    element_report.elementBendingUtilization,
                    element_report.elementCombinedBendingAndAxial
                };
                
                report_list.Add(element_report);
                
            }
            
            // --- output ---
            DA.SetData(0, report_list);
            DA.SetDataTree(1, infoTree);
        }


        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return PTK.Properties.Resources.LocalAnalysis;

            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("9f6403f8-874c-21f8-87ef-6b908c122f2a"); }
        }
    }
}

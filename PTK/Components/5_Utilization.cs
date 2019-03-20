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
            pManager.RegisterParam(new Param_StructuralAssembly(), "Structural Assembly", "SA", "Structural Assembly", GH_ParamAccess.item);
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

                element_report.elementEffectiveLengthDir1 = EffectiveLength(1, e1.BaseCurve.GetLength());
                element_report.elementEffectiveLengthDir2 = EffectiveLength(2, e1.BaseCurve.GetLength());

                infoTree.Append(new Grasshopper.Kernel.Types.GH_String("EffectiveLengthDir1 [mm]=" + element_report.elementEffectiveLengthDir1));
                infoTree.Append(new Grasshopper.Kernel.Types.GH_String("EffectiveLengthDir2 [mm]=" + element_report.elementEffectiveLengthDir2));

                element_report.elementSlendernessRatioDir1 = SlendernessRatio(w, h, 1, e1.BaseCurve.GetLength());
                element_report.elementSlendernessRatioDir2 = SlendernessRatio(w, h, 2, e1.BaseCurve.GetLength());

                infoTree.Append(new Grasshopper.Kernel.Types.GH_String("SlendernessRatioDir1=" + element_report.elementSlendernessRatioDir1));
                infoTree.Append(new Grasshopper.Kernel.Types.GH_String("SlendernessRatioDir2=" + element_report.elementSlendernessRatioDir2));

                element_report.elementEulerForceDir1 = EulerForce(w, h, e1.Composite.MaterialProperty, 1, e1.BaseCurve.GetLength());
                element_report.elementEulerForceDir2 = EulerForce(w, h, e1.Composite.MaterialProperty, 2, e1.BaseCurve.GetLength());

                infoTree.Append(new Grasshopper.Kernel.Types.GH_String("EulerForceDir1=" + element_report.elementEulerForceDir1));
                infoTree.Append(new Grasshopper.Kernel.Types.GH_String("EulerForceDir2=" + element_report.elementEulerForceDir2));

                element_report.elementSlendernessRatioDir1 = SlendernessRelative(w, h, e1.Composite.MaterialProperty, 1, e1.BaseCurve.GetLength());
                element_report.elementSlendernessRatioDir2 = SlendernessRelative(w, h, e1.Composite.MaterialProperty, 2, e1.BaseCurve.GetLength());

                infoTree.Append(new Grasshopper.Kernel.Types.GH_String("SlendernessRatioDir1=" + element_report.elementSlendernessRatioDir1));
                infoTree.Append(new Grasshopper.Kernel.Types.GH_String("SlendernessRatioDir2=" + element_report.elementSlendernessRatioDir2));

                element_report.elementInstabilityFactorDir1 = InstabilityFactor(w, h, e1.Composite.MaterialProperty, 1, e1.BaseCurve.GetLength());
                element_report.elementInstabilityFactorDir2 = InstabilityFactor(w, h, e1.Composite.MaterialProperty, 2, e1.BaseCurve.GetLength());

                infoTree.Append(new Grasshopper.Kernel.Types.GH_String("InstabilityFactorDir1=" + element_report.elementInstabilityFactorDir1));
                infoTree.Append(new Grasshopper.Kernel.Types.GH_String("InstabilityFactorDir2=" + element_report.elementInstabilityFactorDir2));

                element_report.elementBucklingStrengthDir1 = BucklingStrength(w, h, e1.Composite.MaterialProperty, 1, e1.BaseCurve.GetLength());
                element_report.elementBucklingStrengthDir2 = BucklingStrength(w, h, e1.Composite.MaterialProperty, 2, e1.BaseCurve.GetLength());

                infoTree.Append(new Grasshopper.Kernel.Types.GH_String("BucklingStrengthDir1=" + element_report.elementBucklingStrengthDir1));
                infoTree.Append(new Grasshopper.Kernel.Types.GH_String("BucklingStrengthDir2=" + element_report.elementBucklingStrengthDir2));

                infoTree.Append(new Grasshopper.Kernel.Types.GH_String("The compression [N]=" + structuralAssembly.ElementForce[e1].Max_Fx_compression));
                infoTree.Append(new Grasshopper.Kernel.Types.GH_String("The tension [N]=" + structuralAssembly.ElementForce[e1].Max_Fx_tension));
                infoTree.Append(new Grasshopper.Kernel.Types.GH_String("The shear dir1 [N]=" + structuralAssembly.ElementForce[e1].Max_Fy_shear));
                infoTree.Append(new Grasshopper.Kernel.Types.GH_String("The shear dir2 [N]=" + structuralAssembly.ElementForce[e1].Max_Fz_shear));
                infoTree.Append(new Grasshopper.Kernel.Types.GH_String("The torsion [Nmm]=" + structuralAssembly.ElementForce[e1].Max_Mx_torsion));
                infoTree.Append(new Grasshopper.Kernel.Types.GH_String("The bending dir1 [Nmm]=" + structuralAssembly.ElementForce[e1].Max_My_bending));
                infoTree.Append(new Grasshopper.Kernel.Types.GH_String("The bending dir2 [Nmm]=" + structuralAssembly.ElementForce[e1].Max_Mz_bending));

                element_report.elementCompressionUtilization = CompressionUtilization(w, h, e1.Composite.MaterialProperty, structuralAssembly.ElementForce[e1], e1.BaseCurve.GetLength());
                element_report.elementCompressionUtilizationAngle = CompressionUtilizationAngle(w, h, e1.Composite.MaterialProperty, structuralAssembly.ElementForce[e1], e1.BaseCurve.GetLength());
                element_report.elementTensionUtilization = TensionUtilization(w, h, e1.Composite.MaterialProperty, structuralAssembly.ElementForce[e1]);
                element_report.elementBendingUtilization = BendingUtilization(w, h, e1.Composite.MaterialProperty, structuralAssembly.ElementForce[e1]);
                element_report.elementCombinedBendingAndAxial = CombinedBendingAndAxial(w, h, e1.Composite.MaterialProperty, structuralAssembly.ElementForce[e1], e1.BaseCurve.GetLength());

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


                #region temporary solution
                
                tmpefflength_dir1 = EffectiveLength(1, e1.BaseCurve.GetLength());
                tmpefflength_dir2 = EffectiveLength(2, e1.BaseCurve.GetLength());
                tmpslender_dir1 = SlendernessRatio(w, h, 1, e1.BaseCurve.GetLength());
                tmpslender_dir2 = SlendernessRatio(w, h, 2, e1.BaseCurve.GetLength());

                tmp_euler_dir1 = EulerForce(w, h, e1.Composite.MaterialProperty, 1, e1.BaseCurve.GetLength());
                tmp_euler_dir2 = EulerForce(w, h, e1.Composite.MaterialProperty, 2, e1.BaseCurve.GetLength());

                tmp_rel_slender_dir1 = SlendernessRelative(w, h, e1.Composite.MaterialProperty, 1, e1.BaseCurve.GetLength());
                tmp_rel_slender_dir2 = SlendernessRelative(w, h, e1.Composite.MaterialProperty, 2, e1.BaseCurve.GetLength());

                tmp_ins_factor_dir1 = InstabilityFactor(w, h, e1.Composite.MaterialProperty, 1, e1.BaseCurve.GetLength());
                tmp_ins_factor_dir2 = InstabilityFactor(w, h, e1.Composite.MaterialProperty, 2, e1.BaseCurve.GetLength());

                tmp_buc_strength_dir1 = BucklingStrength(w, h, e1.Composite.MaterialProperty, 1, e1.BaseCurve.GetLength());
                tmp_buc_strength_dir2 = BucklingStrength(w, h, e1.Composite.MaterialProperty, 2, e1.BaseCurve.GetLength());

                tmp_util_compression = CompressionUtilization(w, h, e1.Composite.MaterialProperty, structuralAssembly.ElementForce[e1], e1.BaseCurve.GetLength());


                ///This part is just for checking, after validation of calculation can be deleted
                infolist.Add("length=" + e1.BaseCurve.GetLength().ToString());
                infolist.Add("element number =" + indexReport.ToString()) ;
                infolist.Add("element width =" + w.ToString());
                infolist.Add("element height =" + h.ToString());
                double tmparea = w * h;
                infolist.Add("element area  =" + tmparea.ToString());


                infolist.Add("max compression force  =" + structuralAssembly.ElementForce[e1].Max_Fx_compression.ToString());
                
                infolist.Add("effective length dir1 =" + tmpefflength_dir1.ToString());
                infolist.Add("effective length dir2 =" + tmpefflength_dir2.ToString());
                infolist.Add("slenderness dir1 =" + tmpslender_dir1.ToString());
                infolist.Add("slenderness dir2 =" + tmpslender_dir2.ToString());
                infolist.Add("euler force dir1 =" + tmp_euler_dir1.ToString());
                infolist.Add("euler force dir2 =" + tmp_euler_dir2.ToString());
                infolist.Add("relative slenderness dir1 =" + tmp_rel_slender_dir1.ToString());
                infolist.Add("relative slenderness dir2 =" + tmp_rel_slender_dir2.ToString());
                infolist.Add("instability factor dir1 =" + tmp_ins_factor_dir1.ToString());
                infolist.Add("instability factor dir2 =" + tmp_ins_factor_dir2.ToString());
                infolist.Add("buckling strength dir1=" + tmp_buc_strength_dir1.ToString());
                infolist.Add("buckling strength dir2 =" + tmp_buc_strength_dir2.ToString());
                infolist.Add("utilization =" + tmp_util_compression.ToString());
                

                #endregion
            }

            structuralAssembly.ElementReport.Clear();
            int indexReportFromKaramba = -1;
            foreach (var e2 in structuralAssembly.Elements)
            {
                indexReportFromKaramba = indexReportFromKaramba + 1;
                structuralAssembly.ElementReport.Add(e2, report_list[indexReportFromKaramba]);
            }


            // --- output ---
            DA.SetData(0, new GH_StructuralAssembly(structuralAssembly));
            DA.SetData(1, report_list);
            DA.SetDataTree(2, infoTree);

        }

        #region methods
        private double EffectiveLength(int direction, double Length)
        {
            double buckling_coefficient = 1;
            double effective_length = Length * buckling_coefficient;

            if (direction == 1)
            {
                buckling_coefficient = 1;
                effective_length = Length * buckling_coefficient;
            }
            if (direction == 2)
            {
                buckling_coefficient = 1;
                effective_length = Length * buckling_coefficient;
            }

            return effective_length;
        }

        private double SlendernessRatio(double width ,double height, int direction, double length)
        {
            double area = width * height;
            double momentOfInertia0 = width * height * height * height / 12;
            double radiusOfGyration0 = Math.Sqrt(momentOfInertia0/area);

            double momentOfInertia1 = width * width * width * height / 12;
            double radiusOfGyration1 = Math.Sqrt(momentOfInertia1 / area);

            double effective_lenght = EffectiveLength(direction, length);
            double slenderness = 0;

            if (direction == 1)
            {
                slenderness = effective_lenght / radiusOfGyration0;
            }
            if (direction == 2)
            {
                slenderness = effective_lenght / radiusOfGyration1;
            }

            return slenderness;
        }

        private double EulerForce(double width, double height, MaterialProperty md1, int direction, double length)
        {

           
            double euler_force = Math.Pow(Math.PI, 2) * md1.EE0g05 / SlendernessRatio(width, height, direction, length);
            return euler_force;
        }

        private double SlendernessRelative(double width, double height, MaterialProperty md1, int direction, double length)
        {
            // EC5 6.21 , 6.22 relative slenderness
            double slenderness_relative = (SlendernessRatio(width, height , direction, length) / Math.PI) * (Math.Sqrt(md1.Fc0gk / md1.EE0g05));
            return slenderness_relative;
        }

        private double InstabilityFactor(double width, double height, MaterialProperty md1, int direction, double length)
        {
            double instability_faktor_kc;
            double lambda_rel = SlendernessRelative(width, height, md1, direction, length);

            double betaC = 0.1;                     // for the beams with curvature smaller than L/300 for Timber, and L/500 for glulam 
            if (md1.Name == "glulam")
                betaC = 0.1;                        // EC5 equation 6.29 , straightness measured midway between supports should be lower than L/500
            if (md1.Name == "timber")
                betaC = 0.2;                        // EC5 equation 6.29 , straightness measured midway between supports should be lower than L/300

            double ky = 0.5 * (1 + betaC * (lambda_rel - 0.3) + Math.Pow(lambda_rel, 2));                 //EC5 equation 6.27
            instability_faktor_kc = 1 / (ky + Math.Sqrt(Math.Pow(ky, 2) - Math.Pow(lambda_rel, 2)));    //EC5 equation 6.25

            return instability_faktor_kc;
        }

        private double BucklingStrength(double width, double height, MaterialProperty md1, int direction, double length)
        {
            double buckling_strength;
            buckling_strength = md1.Fc0gk * InstabilityFactor(width, height, md1, direction, length);
            return buckling_strength;
        }

        private double CompressionUtilization(double width, double height, MaterialProperty md1, Force force, double length)
        {
            double area = width * height;
            double utilization = 0;
            double utilization_dir1 = 0;
            double utilization_dir2 = 0;
            double stress = force.Max_Fx_compression / area;                        // design compressive stress
            double strength = md1.Kmod * md1.Ksys * md1.Fc0gk / md1.GammaM;          // design compressive strength parallel to the grain

            double relative_slenderness_dir1 = SlendernessRelative(width,height, md1, 1, length);
            double relative_slenderness_dir2 = SlendernessRelative(width,height, md1, 2, length);

            if (relative_slenderness_dir1 <= 0.3 && relative_slenderness_dir2 <= 0.3)
            {
                utilization = stress / strength;
            }
            if (relative_slenderness_dir1 > 0.3 && relative_slenderness_dir2 <= 0.3)
            {
                utilization_dir1 = stress / (relative_slenderness_dir1 * strength);
                utilization_dir2 = stress / strength;

                //choose the bigger utilization to be the element utilization
                utilization = utilization_dir1;
                if (utilization_dir1 < utilization_dir2)
                {
                    utilization = utilization_dir2;
                }

            }
            if (relative_slenderness_dir1 <= 0.3 && relative_slenderness_dir2 > 0.3)
            {
                utilization_dir1 = stress / strength;
                utilization_dir2 = stress / (relative_slenderness_dir2 * strength);

                //choose the bigger utilization to be the element utilization
                utilization = utilization_dir1;
                if (utilization_dir1 < utilization_dir2)
                {
                    utilization = utilization_dir2;
                }
            }
            if (relative_slenderness_dir1 > 0.3 && relative_slenderness_dir2 > 0.3)
            {
                utilization_dir1 = stress / (relative_slenderness_dir1 * strength);
                utilization_dir2 = stress / (relative_slenderness_dir1 * strength);

                //choose the bigger utilization to be the element utilization
                utilization = utilization_dir1;
                if (utilization_dir1 < utilization_dir2)
                {
                    utilization = utilization_dir2;
                }
            }
            return utilization;

        }

        private double CompressionUtilizationAngle(double width, double height, MaterialProperty md1, Force force, double length)
        {
            double area = width * height;
            double utilization = 0;
            double utilization_dir1 = 0;
            double utilization_dir2 = 0;
            double k_c_90 = 1;                   // recommended value [1] page 172

            double stress = force.Max_Fx_compression * Math.Cos(md1.GrainAngle) / area;                        // design compressive stress
            double stressangle = stress * Math.Sin(md1.GrainAngle);                                            // design compressive stress according to grain angle
            double strength = md1.Kmod * md1.Ksys * md1.Fc0gk / md1.GammaM;         // design compressive strength parallel to the grain
            // design compressive strength considering grain angle
            double strengthangle = strength / (strength / (k_c_90 * md1.Fc90gk) * Math.Pow(Math.Sin(md1.GrainAngle), 2) + Math.Pow(Math.Cos(md1.GrainAngle), 2));
            double relative_slenderness_dir1 = SlendernessRelative(width, height, md1, 1, length);
            double relative_slenderness_dir2 = SlendernessRelative(width, height, md1, 2, length);

            utilization = stressangle / strengthangle;

            return utilization;
        }

        private double TensionUtilization(double width, double height, MaterialProperty md1, Force force)
        {
            double utilization = 0;
            double area = width * height;
            #region kh coefficient
            double var1;
            double kh = 1.0;


            double h = height;
            if (height < width)
            {
                h = width;
            }

            if (md1.Name == "Timber")
            {
                var1 = Math.Pow(150 / h, 0.2);
                kh = 1.3;
                if (var1 < 1.3)
                {
                    kh = var1;
                }
            }
            else if (md1.Name == "Glulam")
            {
                var1 = Math.Pow(600 / h, 0.1);
                kh = 1.1;
                if (var1 < 1.1)
                {
                    kh = var1;
                }
            }
            #endregion

            double stress = force.Max_Fx_tension / area;                                                                            // design tension stress
            double strength = md1.Kmod * md1.Ksys * md1.Ft0gk * kh / md1.GammaM;
            // design tension strength

            utilization = stress / strength;

            return utilization;
        }

        public double BendingUtilization(double width, double height, MaterialProperty md1, Force force)
        {
            double area = width * height;
            double momentOfInertia0 = width * height * height * height / 12;
            double radiusOfGyration0 = Math.Sqrt(momentOfInertia0 / area);

            double momentOfInertia1 = width * width * width * height / 12;
            double radiusOfGyration1 = Math.Sqrt(momentOfInertia1 / area);

            double utilization = 0;
            double utilization1 = 0;
            double utilization2 = 0;

            #region kh coefficient
            double var1;
            double kh = 1.0;


            double h = height;
            if (height < width)
            {
                h = width;
            }

            if (md1.Name == "Timber")
            {
                var1 = Math.Pow(150 / h, 0.2);
                kh = 1.3;
                if (var1 < 1.3)
                {
                    kh = var1;
                }
            }
            else if (md1.Name == "Glulam")
            {
                var1 = Math.Pow(600 / h, 0.1);
                kh = 1.1;
                if (var1 < 1.1)
                {
                    kh = var1;
                }
            }
            #endregion

            double stress_1 = force.Max_My_bending * (height / 2) / momentOfInertia0;
            double stress_2 = force.Max_Mz_bending * (width / 2) / momentOfInertia1;            // design bending stress

            double strength1 = Math.Abs(md1.Kmod * md1.Ksys * md1.Fmgk * kh / md1.GammaM);                            // design tension strength
            double strength2 = Math.Abs(md1.Kmod * md1.Ksys * md1.Fmgk * kh / md1.GammaM);                            // design tension strength

            double km = 0.7; // for rectangular timber cross section

            utilization1 = (stress_1 / strength1) + (km * stress_2 / strength2);
            utilization2 = (km * stress_1 / strength1) + (stress_2 / strength2);

            utilization = utilization1;
            if (utilization2 > utilization1)
            {
                utilization = utilization2;
            }

            return utilization;
        }

        public double CombinedBendingAndAxial(double width, double height, MaterialProperty md1, Force force, double length)
        {
            double area = width * height;
            double momentOfInertia0 = width * height * height * height / 12;
            double radiusOfGyration0 = Math.Sqrt(momentOfInertia0 / area);

            double momentOfInertia1 = width * width * width * height / 12;
            double radiusOfGyration1 = Math.Sqrt(momentOfInertia1 / area);

            double utilization = 0;
            double utilization1 = 0;
            double utilization2 = 0;
            double km = 0.7; // for rectangular timber cross section

            #region kh coefficient
            double var1;
            double kh = 1.0;


            double h = height;
            if (height < width)
            {
                h = width;
            }

            if (md1.Name == "Timber")
            {
                var1 = Math.Pow(150 / h, 0.2);
                kh = 1.3;
                if (var1 < 1.3)
                {
                    kh = var1;
                }
            }
            else if (md1.Name == "Glulam")
            {
                var1 = Math.Pow(600 / h, 0.1);
                kh = 1.1;
                if (var1 < 1.1)
                {
                    kh = var1;
                }
            }
            #endregion

            double forceNx = force.Max_Fx_compression;
            if (force.Max_Fx_compression < Math.Abs(force.Max_Fx_tension))
            {
                forceNx = force.Max_Fx_tension;
            }

            double stress_1 = force.Max_My_bending * (height / 2) / momentOfInertia0;
            double stress_2 = force.Max_Mz_bending * (width / 2) / momentOfInertia1;            // design bending stress

            double strength1 = Math.Abs(md1.Kmod * md1.Ksys * md1.Fmgk * kh / md1.GammaM);                            // design tension strength
            double strength2 = Math.Abs(md1.Kmod * md1.Ksys * md1.Fmgk * kh / md1.GammaM);                             // design tension strength

            double stress = forceNx / area;                        // design compressive stress
            double strength = md1.Kmod * md1.Ksys * md1.Fc0gk / md1.GammaM;          // design compressive strength parallel to the grain
            if (forceNx >= 0)
            {
                stress = forceNx / area;                        // design compressive stress
                strength = Math.Abs(md1.Kmod * md1.Ksys * md1.Ft0gk * kh / md1.GammaM);
            }

            double relative_slenderness_dir1 = SlendernessRelative(width,height, md1, 1, length);
            double relative_slenderness_dir2 = SlendernessRelative(width, height, md1, 2, length);

            utilization1 = stress / (strength * InstabilityFactor(width, height, md1, 1, length)) + stress_1 / strength1 + km * stress_2 / strength2;
            utilization2 = stress / (strength * InstabilityFactor(width, height, md1, 2, length)) + km * stress_1 / strength1 + stress_2 / strength2;

            utilization = utilization1;
            if (utilization2 > utilization1)
            {
                utilization = utilization2;
            }

            if (relative_slenderness_dir1 <= 0.3 && relative_slenderness_dir2 <= 0.3)
            {
                utilization1 = Math.Pow(stress / strength, 2) + stress_1 / strength1 + km * stress_2 / strength2;
                utilization2 = Math.Pow(stress / strength, 2) + km * stress_1 / strength1 + stress_2 / strength2;

                utilization = utilization1;
                if (utilization2 > utilization1)
                {
                    utilization = utilization2;
                }

            }

            return utilization;
        }
        #endregion

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
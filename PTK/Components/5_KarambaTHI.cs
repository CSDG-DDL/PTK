using feb;
using GH_IO.Serialization;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Karamba;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;


namespace PTK
{
    public class PTK_KarambaExport : GH_Component
    {
        public PTK_KarambaExport()
          : base("Karamba Analysis", "Karamba Analysis",
              "Creates Model information of Karamba",
              CommonProps.category, CommonProps.subcate5)
        {
            Message = CommonProps.initialMessage;
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddParameter(new Param_StructuralAssembly(), "Structural Assembly", "SA", "Structural Assembly", GH_ParamAccess.item);
            pManager.AddTextParameter("Units", "SI", "The units of geometry either mm or m", GH_ParamAccess.item, "mm");
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.RegisterParam(new Karamba.Models.Param_Model(), "Karamba Model", "KM", "Karamba Model", GH_ParamAccess.item);
            pManager.RegisterParam(new Karamba.Models.Param_Model(), "Analyzed Assembly Meters", "AAm", "", GH_ParamAccess.item);
            pManager.AddNumberParameter("NewDisplacement", "D", "Maximum displacement in [m]", GH_ParamAccess.item);
            pManager.RegisterParam(new Param_StructuralAssembly(), "Structural Assembly", "SA", "Structural Assembly", GH_ParamAccess.item);

        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {


            // --- variables ---
            string unitsInput = "mm";
            string singular_system_msg = "singular stiffness matrix";
            GH_StructuralAssembly gStrAssembly = null;
            StructuralAssembly structuralAssembly = null;
            List<double> maxDisps;
            List<double> gravityForces;
            List<double> elasticEnergy;
            string warning;

            // --- input --- 
            if (!DA.GetData(0, ref gStrAssembly)) { return; }
            structuralAssembly = gStrAssembly.Value;
            if (!DA.GetData(1, ref unitsInput)) { return; }

            // --- solve ---
            var karambaModel = PTK.KarambaConversion.BuildModelMeters(structuralAssembly);

            if (unitsInput == "m")
            {
                karambaModel = null;
                // --- solve ---
                //karambaModel = PTK.KarambaConversion.BuildModelMmeters(structuralAssembly);
            }

            Karamba.Models.Model analyzedModel;

            //clone model to avoid side effects
            analyzedModel = (Karamba.Models.Model)karambaModel.Clone();

            //clone its elements to avoid side effects
            analyzedModel.cloneElements();

            //clone the feb model
            analyzedModel.deepCloneFEModel();
            feb.Deform deform = new feb.Deform(analyzedModel.febmodel);


            feb.Response response = new feb.Response(deform);

            try
            {
                //calculate displacements
                response.updateNodalDisplacements();

                //calculate the member forces
                response.updateMemberForces();
            }
            catch
            {
                // s end an e r r o r mes sage i n c a s e some thing went wrong
                throw new Exception(singular_system_msg);
            }

            Karamba.Models.GH_Model outKrmbModel = new Karamba.Models.GH_Model(analyzedModel);
            var gm_list = new List<Karamba.Models.GH_Model> { outKrmbModel };

            double maxGlobalDisplacement = 0;
            maxGlobalDisplacement = response.maxNodalDisplacement();

            /// the sorting data from analysis
            /// 
            List<string> id_list = new List<string>();
            foreach (var e in analyzedModel.elems)
            {
                id_list.Add(e.id);      // loop over !!karamba!! elements, to take their id
            }


            double maximum_distance_bt_points = 1;
            int maximum_num_points = 6;

            List<List<List<List<Double>>>> results = new List<List<List<List<double>>>>();
            Karamba.Results.Component_BeamForces.solve(
                analyzedModel,
                id_list,
                -1,
                maximum_distance_bt_points,
                maximum_num_points,
                out results
                );



            ///rewriting the results from karamba to PTK
            Dictionary<int, List<Force>> dictElements = new Dictionary<int, List<Force>>();
            List<String> check1 = new List<string>();
            
            string tmpS;

            int load_case_id = -1;
            int element_id;

            foreach (var result_element_list in results)
            {
                /// This loop is over load cases
                load_case_id++;
                element_id = -1;

                foreach (var result_point_list in result_element_list)
                {
                    /// Loop over elements
                    element_id++;
                    int id_points = -1;
                    List<Force> tmpListForce = new List<Force>();

                    foreach (var force_component_list in result_point_list)
                    {
                        /// Loop over result points in the element
                        Force tmpForce = new Force();
                        id_points++;
                        tmpForce.FX = force_component_list[0];
                        tmpForce.FY = force_component_list[1];
                        tmpForce.FZ = force_component_list[2];
                        tmpForce.MX = force_component_list[3];
                        tmpForce.MY = force_component_list[4];
                        tmpForce.MZ = force_component_list[5];
                        tmpForce.karambaElementID = element_id;
                        tmpForce.loadcase = load_case_id;

                        tmpForce.position = Convert.ToDouble(id_points) / (Convert.ToDouble(result_point_list.Count - 1));
                        tmpListForce.Add(tmpForce);
                    }

                    dictElements.Add(element_id, tmpListForce);
                    
                }
            }
            dictElements.Count();
            

            structuralAssembly.ElementForce.Clear();


            // structuralAssemblyNew = structuralAssembly.DeepCopy();
            // structuralAssemblyNew.Elements.Clear();
            List<Element1D> elementListNew = new List<Element1D>();
            List<Force> forceListNew = new List<Force>();
            Assembly assemblyNew = new Assembly();

            int iFK = -1;
            //Add the data to structural analysis
            foreach (var e1 in structuralAssembly.Elements)
            {
                iFK++;
                //structuralAssembly.ElementForce.Add(e1, listOfForcesToElements[indexForceFromKaramba]);
                StructuralData newStructuralData = new StructuralData();
                newStructuralData.StructuralForces = new StructuralForce(dictElements[iFK]);

                //get maximum compression force
                newStructuralData.StructuralForces.maxCompressionForce = new MaxCompression(GetMaximumForce(dictElements[iFK], "FXC"));
                newStructuralData.StructuralForces.maxCompressionForce.positionPoint = e1.BaseCurve.PointAtNormalizedLength(newStructuralData.StructuralForces.maxCompressionForce.position);
                //get maximum tension force
                newStructuralData.StructuralForces.maxTensionForce = new MaxTension(GetMaximumForce(dictElements[iFK], "FXT"));
                newStructuralData.StructuralForces.maxTensionForce.positionPoint = e1.BaseCurve.PointAtNormalizedLength(newStructuralData.StructuralForces.maxTensionForce.position);
                //get maximum shear force direction 1 (Y)
                newStructuralData.StructuralForces.maxShearDir1 = new MaxShearDir1(GetMaximumForce(dictElements[iFK], "FY"));
                newStructuralData.StructuralForces.maxShearDir1.positionPoint = e1.BaseCurve.PointAtNormalizedLength(newStructuralData.StructuralForces.maxShearDir1.position);
                //get maximum shear force direction 2 (Z)
                newStructuralData.StructuralForces.maxShearDir2 = new MaxShearDir2(GetMaximumForce(dictElements[iFK], "FZ"));
                newStructuralData.StructuralForces.maxShearDir2.positionPoint = e1.BaseCurve.PointAtNormalizedLength(newStructuralData.StructuralForces.maxShearDir2.position);
                //get maximum bending MX (Torsion)
                newStructuralData.StructuralForces.maxTorsion = new MaxTorsion(GetMaximumForce(dictElements[iFK], "MX"));
                newStructuralData.StructuralForces.maxTorsion.positionPoint = e1.BaseCurve.PointAtNormalizedLength(newStructuralData.StructuralForces.maxTorsion.position);
                //get maximum bending MY
                newStructuralData.StructuralForces.maxBendingDir1 = new MaxBendingDir1(GetMaximumForce(dictElements[iFK], "MY"));
                newStructuralData.StructuralForces.maxBendingDir1.positionPoint = e1.BaseCurve.PointAtNormalizedLength(newStructuralData.StructuralForces.maxBendingDir1.position);
                //get maximum bending MZ
                newStructuralData.StructuralForces.maxBendingDir2 = new MaxBendingDir2(GetMaximumForce(dictElements[iFK], "MZ"));
                newStructuralData.StructuralForces.maxBendingDir2.positionPoint = e1.BaseCurve.PointAtNormalizedLength(newStructuralData.StructuralForces.maxBendingDir2.position);

                //get effective length in both direction
                newStructuralData.effectiveLength1 = EffectiveLength(0, e1.BaseCurve.GetLength());
                newStructuralData.effectiveLength2 = EffectiveLength(1, e1.BaseCurve.GetLength());

                newStructuralData.slendernessRatio1 = SlendernessRatio(e1.Composite.WidthSimplified, e1.Composite.HeightSimplified, 0, e1.BaseCurve.GetLength());
                newStructuralData.slendernessRatio2 = SlendernessRatio(e1.Composite.WidthSimplified, e1.Composite.HeightSimplified, 1, e1.BaseCurve.GetLength());

                newStructuralData.eulerForce1 = EulerForce(e1.Composite.WidthSimplified, e1.Composite.HeightSimplified, e1.Composite.MaterialProperty, 0, e1.BaseCurve.GetLength());
                newStructuralData.eulerForce2 = EulerForce(e1.Composite.WidthSimplified, e1.Composite.HeightSimplified, e1.Composite.MaterialProperty, 1, e1.BaseCurve.GetLength());

                newStructuralData.slendernessRelative1 = SlendernessRelative(e1.Composite.WidthSimplified, e1.Composite.HeightSimplified, e1.Composite.MaterialProperty, 0, e1.BaseCurve.GetLength());
                newStructuralData.slendernessRelative2 = SlendernessRelative(e1.Composite.WidthSimplified, e1.Composite.HeightSimplified, e1.Composite.MaterialProperty, 1, e1.BaseCurve.GetLength());

                newStructuralData.instabilityFactor1 = InstabilityFactor(e1.Composite.WidthSimplified, e1.Composite.HeightSimplified, e1.Composite.MaterialProperty, 0, e1.BaseCurve.GetLength());
                newStructuralData.instabilityFactor2 = InstabilityFactor(e1.Composite.WidthSimplified, e1.Composite.HeightSimplified, e1.Composite.MaterialProperty, 1, e1.BaseCurve.GetLength());

                newStructuralData.BucklingStrength1 = BucklingStrength(e1.Composite.WidthSimplified, e1.Composite.HeightSimplified, e1.Composite.MaterialProperty, 0, e1.BaseCurve.GetLength());
                newStructuralData.BucklingStrength2 = BucklingStrength(e1.Composite.WidthSimplified, e1.Composite.HeightSimplified, e1.Composite.MaterialProperty, 1, e1.BaseCurve.GetLength());
                
                
                //create new assembly
                assemblyNew.AddElement(new Element1D(e1, newStructuralData ));
            }
            assemblyNew.GenerateDetails();

            StructuralAssembly structuralAssemblyNew = new StructuralAssembly(assemblyNew);

            foreach (var s1 in structuralAssembly.Supports)
            {
                structuralAssemblyNew.AddSupport(s1);
            }
            foreach (var l1 in structuralAssembly.Loads)
            {
                structuralAssemblyNew.AddLoad(l1);
            }

            foreach (DetailingGroupRulesDefinition DG in structuralAssembly.DetailingGroupDefinitions)
            {
                structuralAssemblyNew.AddDetailingGroup(DG.GenerateDetailingGroup(structuralAssemblyNew.Details));
            }


            // --- output ---
            DA.SetData(0, new Karamba.Models.GH_Model(karambaModel));
            DA.SetData(1, gm_list[0]);
            DA.SetData(2, maxGlobalDisplacement);
            DA.SetData(3, new GH_StructuralAssembly(structuralAssemblyNew));


        }


        private Force GetMaximumForce(List<Force> forcesInTheElement, string forceType)
        {
            Force searchedForce = new Force();

            if (forceType == "FXC")
            {
                double maxForce = 0;
                foreach (var eachForce in forcesInTheElement)
                {

                    if (eachForce.FX > maxForce)
                    {
                        searchedForce = eachForce;
                    }
                    maxForce = eachForce.FX;
                }
            }
            else if (forceType == "FXT")
            {
                double maxForce = 0;
                foreach (var eachForce in forcesInTheElement)
                {
                    if (eachForce.FX < maxForce)
                    {
                        searchedForce = eachForce;
                    }
                    maxForce = eachForce.FX;
                }
            }
            else if (forceType == "FY")
            {
                double maxForce = 0;
                foreach (var eachForce in forcesInTheElement)
                {

                    if (Math.Abs(eachForce.FY) > maxForce)
                    {
                        searchedForce = eachForce;
                    }
                    maxForce = Math.Abs(eachForce.FY);
                }
            }
            else if (forceType == "FZ")
            {
                double maxForce = 0;
                foreach (var eachForce in forcesInTheElement)
                {

                    if (Math.Abs(eachForce.FZ) > maxForce)
                    {
                        searchedForce = eachForce;
                    }
                    maxForce = Math.Abs(eachForce.FZ);
                }
            }
            else if (forceType == "MX")
            {
                double maxForce = 0;
                foreach (var eachForce in forcesInTheElement)
                {

                    if (Math.Abs(eachForce.MX) > maxForce)
                    {
                        searchedForce = eachForce;
                    }
                    maxForce = Math.Abs(eachForce.MX);
                }
            }
            else if (forceType == "MY")
            {
                double maxForce = 0;
                foreach (var eachForce in forcesInTheElement)
                {

                    if (Math.Abs(eachForce.MY) > maxForce)
                    {
                        searchedForce = eachForce;
                    }
                    maxForce = Math.Abs(eachForce.MY);
                }
            }
            else if (forceType == "MZ")
            {
                double maxForce = 0;
                foreach (var eachForce in forcesInTheElement)
                {

                    if (Math.Abs(eachForce.MZ) > maxForce)
                    {
                        searchedForce = eachForce;
                    }
                    maxForce = Math.Abs(eachForce.MZ);
                }
            }
            return searchedForce;
        }

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

        private double SlendernessRatio(double width, double height, int direction, double length)
        {
            double area = width * height;
            double momentOfInertia0 = width * height * height * height / 12;
            double radiusOfGyration0 = Math.Sqrt(momentOfInertia0 / area);

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
            double slenderness_relative = (SlendernessRatio(width, height, direction, length) / Math.PI) * (Math.Sqrt(md1.Fc0gk / md1.EE0g05));
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
        
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return PTK.Properties.Resources.Karamba;
            }
        }

        
        public override Guid ComponentGuid
        {
            get { return new Guid("7c6860fa-ee7b-4580-9f04-7bab9e325d8b"); }
        }
    }
}

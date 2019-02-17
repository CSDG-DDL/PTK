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
            pManager.AddTextParameter( "Units", "SI", "The units of geometry either mm or m", GH_ParamAccess.item,"mm");
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.RegisterParam(new Karamba.Models.Param_Model(), "Karamba Model", "KM", "Karamba Model", GH_ParamAccess.item);
            pManager.RegisterParam(new Karamba.Models.Param_Model(), "Analyzed Assembly Meters", "AAm", "", GH_ParamAccess.item);
            pManager.AddNumberParameter("NewDisplacement", "D", "Maximum displacement in [m]", GH_ParamAccess.item);
            pManager.RegisterParam(new Param_StructuralAssembly(), "oldStructural Assembly", "SA", "Structural Assembly", GH_ParamAccess.item);
            pManager.RegisterParam(new Param_StructuralAssembly(), "newStructural Assembly", "nSA", "nStructural Assembly", GH_ParamAccess.item);
            pManager.AddTextParameter("Checkout", "c", "Maximum displacement in [m]", GH_ParamAccess.list);
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

            if (unitsInput=="m")
            {
                karambaModel = null;
            // --- solve ---
            karambaModel = PTK.KarambaConversion.BuildModelMmeters(structuralAssembly);
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
            int maximum_num_points = 20;

            List<List<List<List<Double>>>> results = new List<List<List<List<double>>>>();
            Karamba.Results.Component_BeamForces.solve(
                analyzedModel,
                id_list,
                -1,
                maximum_distance_bt_points,
                maximum_num_points,
                out results
                );


            int load_case_id = -1;
            int element_id;

            List<Force> listOfForcesToElements = new List<Force>();
            List<String> check1 = new List<string>();
            string tmpS;

            foreach (var result_element_list in results)
            {
                load_case_id++;
                /// This loop is over load cases
                /// Please be aware of that:
                /// if you have more than 1 load case the maximum values will be simplified to one case
                /// so if you have 3 loadcases, 
                /// in PTK_element should be PTK_Force from only one case which give the biggest values
                element_id = -1;

                foreach (var result_point_list in result_element_list)
                {
                    /// this loop is over elements in the model

                    // the variables for maximum values of the forces
                    double N1_tension_max = 0;
                    double N1_compression_max = 0;
                    double N2_max = 0;
                    double N3_max = 0;
                    double M1_max = 0;
                    double M2_max = 0;
                    double M3_max = 0;

                    element_id++;


                    /* there is six places for each forces, we are looking for maximum forces on the beam,
                    since the maximum compression can appear in the middle of the beam, the other 5 forces for this place
                    should also be remembered. In the future maybe there we should also add the load case in which the max appeared */
                    var N1_compression_list_element = new List<double>() { 0, 0, 0, 0, 0, 0 };
                    var N1_tension_list_element = new List<double>() { 0, 0, 0, 0, 0, 0 };

                    var N2_list_element = new List<double>() { 0, 0, 0, 0, 0, 0 };
                    var N3_list_element = new List<double>() { 0, 0, 0, 0, 0, 0 };

                    var M1_list_element = new List<double>() { 0, 0, 0, 0, 0, 0 };
                    var M2_list_element = new List<double>() { 0, 0, 0, 0, 0, 0 };
                    var M3_list_element = new List<double>() { 0, 0, 0, 0, 0, 0 };

                    int id_points = -1;
                    Force tmpForces = new Force(
                        N1_compression_list_element,
                        N1_tension_list_element,
                        N2_list_element,
                        N3_list_element,
                        M1_list_element,
                        M2_list_element,
                        M3_list_element
                        );



                    foreach (var force_component_list in result_point_list)
                    {
                        /// Loop over result points
                        id_points++;
                        
                        if (N1_compression_max >= force_component_list[0])  // if the new compression(+ sign) force is bigger then the previous one, save it
                        {
                            N1_compression_max = force_component_list[0];
                            N1_compression_list_element = force_component_list;

                            //フィールドへの直接SETになっている、非推奨
                            tmpForces.Loadcase_Max_Fx_compression = load_case_id;

                        }
                        if (N1_tension_max <= force_component_list[0])      // if the new tension(- sign) force is smaller than the previous one, save it
                        {
                            N1_tension_max = force_component_list[0];
                            N1_tension_list_element = force_component_list;

                            tmpForces.Loadcase_Max_Fx_tension = load_case_id;
                        }
                        if (N2_max <= Math.Abs(force_component_list[1]))    // we do not care about shear1 sign, so just if absolute value is bigger
                        {
                            N2_max = force_component_list[1];
                            N2_list_element = force_component_list;

                            tmpForces.Loadcase_Max_Fy_shear = load_case_id;
                        }
                        if (N3_max <= Math.Abs(force_component_list[2]))    // we do not care about shear2 sign, so just if absolute value is bigger
                        {
                            N3_max = force_component_list[2];
                            N3_list_element = force_component_list;

                            tmpForces.Loadcase_Max_Fz_shear = load_case_id;
                        }
                        if (M1_max <= Math.Abs(force_component_list[3]))    // we do not care about moment1 sign, so just if absolute value is bigger
                        {
                            M1_max = force_component_list[3];
                            M1_list_element = force_component_list;

                            tmpForces.Loadcase_Max_Mx_torsion = load_case_id;
                        }
                        if (M2_max <= Math.Abs(force_component_list[4]))    // we do not care about moment2 sign, so just if absolute value is bigger
                        {
                            M2_max = force_component_list[4];
                            M2_list_element = force_component_list;

                            tmpForces.Loadcase_Max_My_bending = load_case_id;
                        }
                        if (M3_max <= Math.Abs(force_component_list[5]))    // we do not care about moment3 sign, so just if absolute value is bigger
                        {
                            M3_max = force_component_list[5];
                            M3_list_element = force_component_list;

                            tmpForces.Loadcase_Max_Mz_bending = load_case_id;   
                            }

                    }

                    tmpForces.FXc = N1_compression_list_element;
                    tmpForces.FXt = N1_tension_list_element;
                    tmpForces.FY = N2_list_element;
                    tmpForces.FZ = N3_list_element;
                    tmpForces.MX = M1_list_element;
                    tmpForces.MY = M2_list_element;
                    tmpForces.MZ = M3_list_element;

                    //フィールドへの直接SETになっている、非推奨
                    //structuralAssembly.ElementForce.Add();
                    Force tmpforce1 = new Force(N1_compression_list_element, N1_tension_list_element, N2_list_element, N3_list_element, M1_list_element, M2_list_element, M3_list_element);

                    tmpforce1.Max_Fx_compression = Math.Abs(N1_compression_max);
                    tmpforce1.Max_Fx_tension = Math.Abs(N1_tension_max);
                    tmpforce1.Max_Fy_shear = Math.Abs(N2_max);
                    tmpforce1.Max_Fz_shear = Math.Abs(N3_max);
                    tmpforce1.Max_Mx_torsion = Math.Abs(M1_max);
                    tmpforce1.Max_My_bending = Math.Abs(M2_max);
                    tmpforce1.Max_Mz_bending = Math.Abs(M3_max);

                    listOfForcesToElements.Add( tmpforce1 );
                    // the maximum forces in the elements
                    tmpS = " Fxc= "+ tmpforce1.Max_Fx_compression 
                        + " Fxt= " + tmpforce1.Max_Fx_tension 
                        + " Fy= " + tmpforce1.Max_Fy_shear 
                        + " Fz= " + tmpforce1.Max_Fz_shear 
                        + " Mx= " + tmpforce1.Max_Mx_torsion 
                        + " My= " + tmpforce1.Max_My_bending 
                        + " Mz= " + tmpforce1.Max_Mz_bending;

                    check1.Add(tmpS);

                }

            }

            Dictionary<Element1D, Force> forceDictionary = new Dictionary<Element1D, Force>();
            structuralAssembly.ElementForce.Clear();
            int indexForceFromKaramba = -1;
            StructuralAssembly structuralAssemblyNew = new StructuralAssembly();
            // structuralAssemblyNew = structuralAssembly.DeepCopy();
            // structuralAssemblyNew.Elements.Clear();
            List<Element1D> elementListNew = new List<Element1D>();
            List<Force> forceListNew = new List<Force>();

            check1.Add("Add the data to structural analysis");
            foreach (var e1 in structuralAssembly.Elements)
            {
                indexForceFromKaramba = indexForceFromKaramba + 1;
                structuralAssembly.ElementForce.Add(e1, listOfForcesToElements[indexForceFromKaramba]);

                // creating new structural assembly with updated forces in elements
                // forceListNew.Add(listOfForcesToElements[indexForceFromKaramba]);
                // bew 
                elementListNew.Add(new Element1D(e1, listOfForcesToElements[indexForceFromKaramba])); //rescribning elements with new forces
                // forceListNew.Clear();
                // structuralAssemblyNew.Elements.Add(new Element1D(e1, forceListNew));
                structuralAssemblyNew.AddElement(new Element1D(e1, listOfForcesToElements[indexForceFromKaramba]));

                //just to check
                tmpS = " element= " + indexForceFromKaramba + "  added to elementForce in structural assembly";
                check1.Add(tmpS);
                tmpS = listOfForcesToElements[indexForceFromKaramba].Max_Fx_compression.ToString() ;
                check1.Add(tmpS);


            }



            
            // --- output ---
            DA.SetData(0, new Karamba.Models.GH_Model(karambaModel));
            DA.SetData(1, gm_list[0]);
            DA.SetData(2, maxGlobalDisplacement);
            DA.SetData(3, new GH_StructuralAssembly(structuralAssembly));
            DA.SetData(4, new GH_StructuralAssembly(structuralAssemblyNew));
            DA.SetDataList(5, check1);
            
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

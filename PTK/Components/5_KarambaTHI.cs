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
            Dictionary<int, List<Force>> listOfForcesToElements = new Dictionary<int, List<Force>>();
            List<String> check1 = new List<string>();
            List<Force> tmpListForce = new List<Force>();
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
                    
                    foreach (var force_component_list in result_point_list)
                    {
                        /// Loop over result points in the element
                        Force tmpForce = new Force( );
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
                    
                    listOfForcesToElements.Add(element_id, tmpListForce);
                    tmpListForce.Clear();
                }
            }
            listOfForcesToElements.Count();
            structuralAssembly.ElementForce.Clear();
            
            
            // structuralAssemblyNew = structuralAssembly.DeepCopy();
            // structuralAssemblyNew.Elements.Clear();
            List<Element1D> elementListNew = new List<Element1D>();
            List<Force> forceListNew = new List<Force>();
            Assembly assemblyNew = new Assembly();

            int indexForceFromKaramba = -1;
            //Add the data to structural analysis
            foreach (var e1 in structuralAssembly.Elements)
            {
                indexForceFromKaramba++;
                //structuralAssembly.ElementForce.Add(e1, listOfForcesToElements[indexForceFromKaramba]);
                
                // 
                //structuralAssemblyNew.AddElement(new Element1D(e1, listOfForcesToElements[indexForceFromKaramba]));
                assemblyNew.AddElement(new Element1D(e1, new StructuralData() ));
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
            DA.SetData(3, new GH_StructuralAssembly(structuralAssembly));
            DA.SetData(4, new GH_StructuralAssembly(structuralAssemblyNew));
            
            
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

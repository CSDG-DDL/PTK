using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace PTK.Components
{
    public class PTK_DefinitionDetailingGroup : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the _10_DefinitionDetailingGroup class.
        /// </summary>
        public PTK_DefinitionDetailingGroup()
          : base("DetailingGroupDefinition", "DG",
              "Define the detailing groups by adding true and false properties",
              CommonProps.category, CommonProps.subcate10)
        {
        }

        /// <summary>
        /// Overrides the exposure level in the components category 
        /// </summary>
        public override GH_Exposure Exposure
        {
            get
            { return GH_Exposure.hidden; }
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("DetailingGroup Name", "N", "Add Detailing Group Name. Used as key to select detailing groups", GH_ParamAccess.item,"N/A");
            pManager.AddGenericParameter("True SearchCriteria ", "=", "Add SearchCriteria that are true for the details in the detailing group", GH_ParamAccess.list);
            pManager.AddGenericParameter("False SearchCriteria", "≠", "Add SearchCriteria that are false for the details in the detailing group", GH_ParamAccess.list);
            pManager.AddGenericParameter("NodePlaneRule", "NP","Add a NodePlaneRule to generate nodeplane according to its detail", GH_ParamAccess.item);
            pManager[0].Optional = true;
            pManager[1].Optional = true;
            pManager[2].Optional = true;
            pManager[3].Optional = true;


        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("DetailingGroup", "=", "Add rules that are true for the details in the detailing group", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            //Variables 
            string Name = "N/A";
            List<Rules.Rule> TrueRulesObjects = new List<Rules.Rule>();
            List<Rules.Rule> FalseRulesObjects = new List<Rules.Rule>();

            List<CheckGroupDelegate> TrueRules = new List<CheckGroupDelegate>();
            List<CheckGroupDelegate> FalseRules = new List<CheckGroupDelegate>();
            PlaneRules.PlaneRule PlaneRule = new PlaneRules.PlaneRule();

            //Input 
            DA.GetData(0, ref Name);
            DA.GetDataList(1, TrueRulesObjects);
            DA.GetDataList(2, FalseRulesObjects);
            if (!DA.GetData(3, ref PlaneRule))
            {
                //Solve 
                PlaneRules.NormalVectorPlane NodeRule = new PlaneRules.NormalVectorPlane(new Vector3d(0,0,1), "");
                PlaneRule = new PlaneRules.PlaneRule(NodeRule.GenerateDetailingGroupPlane);

            }
            



            //Extracting delegates to list from objects
            foreach (Rules.Rule R in TrueRulesObjects)
            {
                TrueRules.Add(R.checkdelegate);
            }
            foreach (Rules.Rule R in FalseRulesObjects)
            {
                FalseRules.Add(R.checkdelegate);
            }

            //Solve 
            DetailingGroupRulesDefinition Definition = new DetailingGroupRulesDefinition(Name, TrueRules, FalseRules, PlaneRule);


            //Output
            DA.SetData(0, Definition);



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
                return Properties.Resources.SearchDetail;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("653f25bf-2c12-409b-91f1-2f373ad154f2"); }
        }
    }
}
using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace PTK.Components
{
    public class _10_Rule_ElementForce : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the _10_RuleElementCompressionForce class.
        /// </summary>
        public _10_Rule_ElementForce()
          : base("Element force", "F",
              "Makes you select details based on force range",
              CommonProps.category, CommonProps.subcate10)
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            // Forces
            pManager.AddIntervalParameter("Min/max comp force", "CFx", "The compression force range", GH_ParamAccess.item, new Interval(0,0) );
            pManager.AddIntervalParameter("Min/max tension force", "TFx", "The tension force range", GH_ParamAccess.item, new Interval(0, 0));
            pManager.AddIntervalParameter("Min/max shear Y force", "Fy", "The shear force in Y direction range", GH_ParamAccess.item , new Interval(0, 0));
            pManager.AddIntervalParameter("Min/max shear Z force", "Fz", "The shear force in Z direction range", GH_ParamAccess.item, new Interval(0, 0));
            // Moments
            pManager.AddIntervalParameter("Min/max comp force", "Mx", "The torsion moment Mx", GH_ParamAccess.item, new Interval(0, 0));
            pManager.AddIntervalParameter("Min/max comp force", "My", "The bending moment in Y direction My", GH_ParamAccess.item, new Interval(0, 0));
            pManager.AddIntervalParameter("Min/max comp force", "Mz", "The bending moment in Z direction Mz", GH_ParamAccess.item, new Interval(0, 0));
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("DetailDescription", "DD", "Outputed detailDescription", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {

            //Variables
            Interval forceinterval0 = new Interval();
            Interval forceinterval1 = new Interval();
            Interval forceinterval2 = new Interval();
            Interval forceinterval3 = new Interval();
            Interval forceinterval4 = new Interval();
            Interval forceinterval5 = new Interval();
            Interval forceinterval6 = new Interval();

            //Input
            if (!DA.GetData(0, ref forceinterval0)){ return; };
            if (!DA.GetData(1, ref forceinterval1)){ return; };
            if (!DA.GetData(2, ref forceinterval2)){ return; };
            if (!DA.GetData(3, ref forceinterval3)){ return; };
            if (!DA.GetData(4, ref forceinterval4)){ return; };
            if (!DA.GetData(5, ref forceinterval5)){ return; };
            if (!DA.GetData(6, ref forceinterval6)){ return; };

            //Solve
            List<Rules.Rule> listRules = new List<Rules.Rule>();
            

            if (!forceinterval0.IsSingleton)
            {
                Rules.ElementForce Rule0 = new Rules.ElementForce(forceinterval0, 0);
                listRules.Add(new Rules.Rule(new CheckGroupDelegate(Rule0.check)));
            }

            if (!forceinterval1.IsSingleton)
            {
                Rules.ElementForce Rule1 = new Rules.ElementForce(forceinterval1, 1);
                listRules.Add(new Rules.Rule(new CheckGroupDelegate(Rule1.check)));
            }

            if (!forceinterval2.IsSingleton)
            {
                Rules.ElementForce Rule2 = new Rules.ElementForce(forceinterval2, 2);
                listRules.Add(new Rules.Rule(new CheckGroupDelegate(Rule2.check)));
            }

            if (!forceinterval3.IsSingleton)
            {
                Rules.ElementForce Rule3 = new Rules.ElementForce(forceinterval3, 3);
                listRules.Add(new Rules.Rule(new CheckGroupDelegate(Rule3.check)));
            }

            if (!forceinterval4.IsSingleton)
            {
                Rules.ElementForce Rule4 = new Rules.ElementForce(forceinterval4, 4);
                listRules.Add(new Rules.Rule(new CheckGroupDelegate(Rule4.check)));
            }

            if (!forceinterval5.IsSingleton)
            {
                Rules.ElementForce Rule5 = new Rules.ElementForce(forceinterval5, 5);
                listRules.Add(new Rules.Rule(new CheckGroupDelegate(Rule5.check)));
            }

            if (!forceinterval6.IsSingleton)
            {
                Rules.ElementForce Rule6 = new Rules.ElementForce(forceinterval6, 6);
                listRules.Add(new Rules.Rule(new CheckGroupDelegate(Rule6.check)));
            }



            //Output
            //DA.SetData(0, new Rules.Rule(new CheckGroupDelegate(Rule0.check)));
            DA.SetDataList(0, listRules);
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
                return null;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("f099152b-5123-4d2f-a950-ad111fe027c2"); }
        }
    }
}
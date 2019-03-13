using System;
using System.Collections.Generic;
using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Parameters.Hints;
using Rhino.Geometry;

namespace PTK.Components
{
    public class _13_Utilities_GroupCurvesByLenght : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public _13_Utilities_GroupCurvesByLenght()
          : base("Group Curves by Lenght", "Crv Lenghts",
              "Puts the curves separate branches according to the thresholds inputed",
              CommonProps.category, CommonProps.subcate10)
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("Curves", "C", "Curves to group", GH_ParamAccess.list);
            pManager.AddNumberParameter("Thresholds", "N", "Numbers that define threshold ranges", GH_ParamAccess.list,0);
            pManager.AddBooleanParameter("Strict", "S",
                "If true, the grouping is strict to and curves outside the range will not be added.",
                GH_ParamAccess.item, false);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("Grouped Curves", "C ", "Grouped Curves. BANG can be used to extract each path", GH_ParamAccess.tree);
            pManager.AddNumberParameter("Lenght domains", "L", "The Lenghts domains for each group", GH_ParamAccess.tree);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            //Variables
            List<Curve> curves = new List<Curve>();
            List<double> numbers = new List<double>();
            List<double> lenghts = new List<double>();
            DataTree<Curve> sortedCurves = new DataTree<Curve>();
            DataTree<double> domains = new DataTree<double>();
            bool strict = false;

            //Input
            DA.GetDataList(0, curves);
            DA.GetDataList(1, numbers);
            DA.GetData(2, ref strict);

            //Solve
            if (!strict)
            {
                numbers.Sort();
                double nMin = numbers[0];
                double nMax = numbers[numbers.Count - 1];

                for (int k = 0; k < curves.Count; k++)
                {
                    double lenght = curves[k].GetLength();
                    lenghts.Add(lenght);
                }
                lenghts.Sort();

                double lMin = lenghts[0];
                double lMax = lenghts[lenghts.Count - 1];

                if (lMin < nMin)
                    numbers.Add(lMin);
                if (lMax > nMax)
                    numbers.Add(lMax);
            }
       

            numbers.Sort();
                
            for (int i = 0; i < numbers.Count - 1; i++)
            {
                GH_Path pth = new GH_Path(i);
                double min = numbers[i];
                double max = numbers[i + 1];

                domains.Add(min,pth);
                domains.Add(max,pth);

                for (int j = 0; j < curves.Count; j++)
                {
                    double lenght = curves[j].GetLength();
                    if (min < lenght && lenght < max)
                    {
                        sortedCurves.Add(curves[j], pth);
                    }
                }
            }
            


            //Output
            DA.SetDataTree(0, sortedCurves);
            DA.SetDataTree(1, domains);
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
            get { return new Guid("332fd150-544e-46c1-bf68-551574d6ed73"); }
        }
    }
}
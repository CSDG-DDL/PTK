using System;
using System.Collections.Generic;
using System.Drawing;
using Grasshopper.Kernel;
using Rhino.Geometry;
using System.Linq;
using Grasshopper;


namespace PTK.Components
{
    public class SelectDetailingGroup : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public SelectDetailingGroup()
          : base("SelectDetailingGroup", "DG",
              "Use the group name to select a detailing group",
              CommonProps.category, CommonProps.subcate3)
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("PTK Assembly", "PTK A", "PTK Assembly", GH_ParamAccess.item);
            pManager.AddTextParameter("DetailingGroupName", "", "", GH_ParamAccess.item);  
            pManager.AddIntegerParameter("Sorting rule", "SR", "0=Structural, 1=Alphabetical, 2=ElementLength", GH_ParamAccess.item, 0);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("N", "NodeTree", "", GH_ParamAccess.tree);
            pManager.AddGenericParameter("ElementTree", "", "", GH_ParamAccess.tree);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            #region variables

            Assembly assembly = new Assembly();
            GH_Assembly ghAssembly = new GH_Assembly();

            

            string Name = "";
            int priorityKey = 0;




            DA.GetData(0, ref ghAssembly);
            DA.GetData(1, ref Name);
            DA.GetData(2, ref priorityKey);

            assembly = ghAssembly.Value;










            //Until now, the slider is a hypothetical object.
            // This command makes it 'real' and adds it to the canvas.









            if (assembly.DetailingGroups.Find(t => t.Name == Name) != null)
            {
                List<Detail> Details = assembly.DetailingGroups.Find(t => t.Name == Name).Details;

                List<Node> Nodes = new List<Node>();


                DataTree<ElementInDetail> ElementTree = new DataTree<ElementInDetail>();
                DataTree<Node> NodeTree = new DataTree<Node>();



                int branchindex = 0;
                foreach (Detail Detail in Details)
                {
                    Detail.GenerateUnifiedElementVectors();
                    List<ElementInDetail> ElementWrapper = new List<ElementInDetail>();

                    for (int i = 0; i < Detail.Elements.Count; i++)
                    {
                        ElementWrapper.Add(new ElementInDetail(Detail.Elements[i], Detail.UnifiedVectors[i]));
                    }


                    //Sorting the element outputs based on 
                    if (priorityKey == 0)  // sorts by Structural priority
                    {
                        ElementWrapper = ElementWrapper.OrderBy(t => t.Element.Priority).ToList();


                    }

                    if (priorityKey == 1) //Sorts alphabetically based on tag
                    {
                        ElementWrapper = ElementWrapper.OrderBy(t => t.Element.Tag).ToList();

                    }

                    if (priorityKey == 2) //Sorts by length
                    {
                        ElementWrapper = ElementWrapper.OrderBy(t => t.Element.BaseCurve.GetLength()).ToList();

                    }




                    ElementTree.AddRange(ElementWrapper, new Grasshopper.Kernel.Data.GH_Path(branchindex));
                    NodeTree.Add(Detail.Node, new Grasshopper.Kernel.Data.GH_Path(branchindex));


                    branchindex++;

                }




                DA.SetDataTree(0, NodeTree);
                DA.SetDataTree(1, ElementTree);
            }

            


            #endregion



            #region solve


            #endregion

            #region output
            #endregion

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
            get { return new Guid("018213a3-efa0-45b0-b444-33259ad18f81"); }
        }
    }
}
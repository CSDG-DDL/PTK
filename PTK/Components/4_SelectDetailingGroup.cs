using System;
using System.Collections.Generic;
using System.Drawing;
using Grasshopper.Kernel;
using Rhino.Geometry;
using System.Linq;
using Grasshopper;


namespace PTK.Components
{
    public class PTK_SelectDetailingGroup : GH_Component
    {
        public PTK_SelectDetailingGroup()
          : base("SelectDetailingGroup", "DG",
              "Use the group name to select a detailing group",
              CommonProps.category, CommonProps.subcate3)
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddParameter(new Param_Assembly(), "Assembly", "A", "Assembly", GH_ParamAccess.item);
            pManager.AddTextParameter("DetailingGroupName", "DN", "DetailingGroupName", GH_ParamAccess.item, "DetailingGroupName");  
            pManager.AddIntegerParameter("Sorting rule", "SR", "0=Structural, 1=Alphabetical, 2=ElementLength", GH_ParamAccess.item, 0);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            
            pManager.AddGenericParameter("Node", "N", "Node", GH_ParamAccess.tree);
            pManager.AddPlaneParameter("Nodeplanes", "P", "", GH_ParamAccess.tree);
            pManager.AddGenericParameter("Elements", "E", "Elements", GH_ParamAccess.tree);
            pManager.AddVectorParameter("UnifiedElementVectors", "V", "", GH_ParamAccess.tree);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // --- variables ---
            GH_Assembly ghAssembly = new GH_Assembly();
            Assembly assembly = new Assembly();
            string name = "";
            int priorityKey = 0;

            // --- input --- 
            if (!DA.GetData(0, ref ghAssembly)) { return; }
            assembly = ghAssembly.Value;
            if (!DA.GetData(1, ref name)) { return; }
            if (!DA.GetData(2, ref priorityKey)) { return; }

            //Until now, the slider is a hypothetical object.
            // This command makes it 'real' and adds it to the canvas.

            // --- solve ---
            if (assembly.DetailingGroups.Any(t => t.Name == name))
            {
                
                List<Detail> details = assembly.DetailingGroups.Find(t => t.Name == name).Details;
                List<Plane> Planes = assembly.DetailingGroups.Find(t => t.Name == name).NodeGroupPlanes;

                DataTree<Node> Nodes = new DataTree<Node>();
                DataTree<Plane> NodePlanes = new DataTree<Plane>();
                DataTree<GH_Element1D> Elements = new DataTree<GH_Element1D>();
                DataTree<Vector3d> UnifiedVectors = new DataTree<Vector3d>();
                

                for(int i =0; i<details.Count;i++)
                {
                    
                    details[i].GenerateUnifiedElementVectors();
                    details[i].SortElement(priorityKey);

                    Grasshopper.Kernel.Data.GH_Path Path = new Grasshopper.Kernel.Data.GH_Path(i);
                    Node tempNode = details[i].Node;

                    Nodes.Add(tempNode, Path);
                    NodePlanes.Add(Planes[i], Path);

                    

                    Elements.AddRange(details[i].Elements.ConvertAll(e => new GH_Element1D(e)), Path); 
                    UnifiedVectors.AddRange(details[i].Elements.ConvertAll(e => details[i].ElementsUnifiedVectorsMap[e]), Path);
                    



                }


                // --- output ---
                DA.SetDataTree(0, Nodes);
                DA.SetDataTree(1, NodePlanes);
                DA.SetDataTree(2, Elements);
                DA.SetDataTree(3, UnifiedVectors);

                //// --- solve ---
                //GH_Node node = new GH_Node(detail.Node);
                //List<GH_Element1D> elements = detail.Elements.ConvertAll(e => new GH_Element1D(e));
                //List<Vector3d> vectors = detail.Elements.ConvertAll(e => detail.ElementsUnifiedVectorsMap[e]);
                //string type = detail.Type.ToString();









            }
        }

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return Properties.Resources.SearchDetail;
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("018213a3-efa0-45b0-b444-33259ad18f81"); }
        }
    }
}
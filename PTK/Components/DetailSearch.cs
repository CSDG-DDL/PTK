﻿using System;
using System.Collections.Generic;
using System.Drawing;
using Grasshopper.Kernel;
using Rhino.Geometry;
using System.Linq;
using Grasshopper;

namespace PTK.Components
{
    public class DetailSearch : GH_Component
    {
        List<List<Plane>> PlanesLists = new List<List<Plane>>();
        List<Color> Colors = new List<Color>();
        List<String> names = new List<String>();
        int height = 14;
        bool showText = false;

        /// <summary>
        /// Initializes a new instance of the DetailSearch class.
        /// </summary>
        public DetailSearch()
          : base("DetailSearch", "DS",
              "Search For A Detail different search rules",
              CommonProps.category, CommonProps.subcate10)
        {
        }
        /// <summary>
        /// Overrides the exposure level in the components category 
        /// </summary>
        public override GH_Exposure Exposure
        {
        get
        { return GH_Exposure.primary; }
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
        pManager.AddParameter(new Param_Assembly(), "Assembly", "A", "Assembly", GH_ParamAccess.item);
        pManager.AddTextParameter("DetailingGroupName", "DN", "DetailingGroupName", GH_ParamAccess.item, "DetailingGroupName"); 
        pManager.AddGenericParameter("True Rules", "=", "Add rules that are true for the details in the detailing group", GH_ParamAccess.list);
        pManager.AddGenericParameter("False Rules", "≠", "Add rules that are false for the details in the detailing group", GH_ParamAccess.list);
        pManager.AddGenericParameter("NodePlaneRule", "NP", "Add a NodePlaneRule to generate nodeplane according to its detail", GH_ParamAccess.item);
        pManager.AddIntegerParameter("Sorting rule", "SR", "0=Structural, 1=Alphabetical, 2=ElementLength, 3=Clockwice(Based on detailPlane)", GH_ParamAccess.item, 0);
        pManager.AddColourParameter("DetailPreviewColor", "C", "Color of detailpreview", GH_ParamAccess.item);
        pManager.AddBooleanParameter("PreviewName?", "N", "True if you want to preview name of detail", GH_ParamAccess.item, false);
        //pManager.AddIntegerParameter("PreviewSize", "S", "Optional Size of the detailpreview", GH_ParamAccess.item, 14);
        pManager[1].Optional = true;
        pManager[2].Optional = true;
        pManager[3].Optional = true;
        pManager[4].Optional = true;
        pManager[5].Optional = true;
        pManager[6].Optional = true;
        pManager[7].Optional = true;
        //pManager[8].Optional = true;




        }

    protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
    {

        pManager.AddGenericParameter("Node", "N", "Node", GH_ParamAccess.tree);
        pManager.AddPlaneParameter("Nodeplanes", "P", "", GH_ParamAccess.tree);
        pManager.AddGenericParameter("Elements", "E", "Elements", GH_ParamAccess.tree);
        pManager.AddVectorParameter("UnifiedElementVectors", "V", "", GH_ParamAccess.tree);
        pManager.HideParameter(1);



        }

    protected override void SolveInstance(IGH_DataAccess DA)
    {
            //Variables 
            string Name = "N/A";
            List<Rules.Rule> TrueRulesObjects = new List<Rules.Rule>();
            List<Rules.Rule> FalseRulesObjects = new List<Rules.Rule>();

            List<CheckGroupDelegate> TrueRules = new List<CheckGroupDelegate>();
            List<CheckGroupDelegate> FalseRules = new List<CheckGroupDelegate>();
            PlaneRules.PlaneRule PlaneRule = new PlaneRules.PlaneRule();

            GH_Assembly ghAssembly = new GH_Assembly();
            Assembly assembly = new Assembly();
            int priorityKey = 0;
            Color color = new Color();
            bool preview = false;
            //int size = 14;





            // --- input --- 






            //Input 
            if (!DA.GetData(0, ref ghAssembly)) { return; }
            assembly = ghAssembly.Value;
            DA.GetData(1, ref Name);
            DA.GetDataList(2, TrueRulesObjects);
            DA.GetDataList(3, FalseRulesObjects);
            if (!DA.GetData(4, ref PlaneRule))
            {
                //Solve 
                PlaneRules.NormalVectorPlane NodeRule = new PlaneRules.NormalVectorPlane(new Vector3d(0, 0, 1), "");
                PlaneRule = new PlaneRules.PlaneRule(NodeRule.GenerateDetailingGroupPlane);

            }
            if (!DA.GetData(5, ref priorityKey)) { return; }
            if(!DA.GetData(6, ref color))
            {
                color = Color.Red;
            }
            DA.GetData(7, ref preview);
            //DA.GetData(8, ref size);
            Colors.Add(color);

            if (preview)
            {
                names.Add(Name);
            }
            else
            {
                names.Add(".'");
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

            ////////////////////////////////////////////


            DetailingGroup DetailingGroup = Definition.GenerateDetailingGroup(assembly.Details);



            PlanesLists.Add(DetailingGroup.NodeGroupPlanes);
            //height = size;
            showText = preview;


            // --- solve ---
            if (true)
            {

                List<Detail> details = DetailingGroup.Details;
                List<Plane> Planes = DetailingGroup.NodeGroupPlanes;

                DataTree<GH_Node> Nodes = new DataTree<GH_Node>();
                DataTree<Plane> NodePlanes = new DataTree<Plane>();
                DataTree<GH_Element1D> Elements = new DataTree<GH_Element1D>();
                DataTree<Vector3d> UnifiedVectors = new DataTree<Vector3d>();


                for (int i = 0; i < details.Count; i++)
                {
                    Grasshopper.Kernel.Data.GH_Path Path = new Grasshopper.Kernel.Data.GH_Path(i);

                    details[i].GenerateUnifiedElementVectors();

                    if (priorityKey == 3)
                    {
                        List<double> Angles = new List<double>();

                        foreach (Element1D elem in details[i].Elements)
                        {
                            Angles.Add(Vector3d.VectorAngle(Planes[i].XAxis, details[i].ElementsUnifiedVectorsMap[elem], Planes[i]));


                        }

                        Angles = Angles;

                        var OrderedElems = details[i].Elements.Zip(Angles, (item1, item2) => new KeyValuePair<Element1D, double>(item1, item2)).OrderBy(pair => pair.Value);

                        Angles = OrderedElems.Select(pair => pair.Value).ToList();

                        List<Element1D> temp = OrderedElems.Select(pair => pair.Key).ToList();
                        Elements.AddRange(temp.ConvertAll(e => new GH_Element1D(e)), Path);

                        UnifiedVectors.AddRange(temp.ConvertAll(e => details[i].ElementsUnifiedVectorsMap[e]), Path);





                    }

                    else
                    {
                        details[i].SortElement(priorityKey);
                        Elements.AddRange(details[i].Elements.ConvertAll(e => new GH_Element1D(e)), Path);

                        UnifiedVectors.AddRange(details[i].Elements.ConvertAll(e => details[i].ElementsUnifiedVectorsMap[e]), Path);
                    }





                    GH_Node tempNode = new GH_Node(details[i].Node);

                    Nodes.Add(tempNode, Path); //ADDED THIS + Changed datatreees to node
                    NodePlanes.Add(Planes[i], Path);




                }

 

                //}

                // --- output ---
                DA.SetDataTree(0, Nodes);
                DA.SetDataTree(1, NodePlanes);
                DA.SetDataTree(2, Elements);
                DA.SetDataTree(3, UnifiedVectors);



            }
    }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        /// 

    public override void ExpireSolution(bool recompute)
    {
        Colors.Clear();
        PlanesLists.Clear();
        names.Clear();
        base.ExpireSolution(recompute);
    }

    //public override BoundingBox ClippingBox => models.Keys.ToList()[0].GetBoundingBox(false);
    public override void DrawViewportMeshes(IGH_PreviewArgs args)
    {
        for (int i =0; i<PlanesLists.Count;i++ )
        {
                for (int j = 0; j < PlanesLists[i].Count; j++)
                {
                    if (showText)
                    {
                        args.Display.DrawDot(PlanesLists[i][j].Origin, names[i], Colors[i], Color.White);

                        //    args.Display.Draw3dText(names[i], Colors[i], PlanesLists[i][j], height, "Arial");
                        //    args.Display.Draw2dText(names[i], Colors[i], PlanesLists[i][j].Origin, true, height, "Arial");
                    }

                    else
                        args.Display.DrawDot(PlanesLists[i][j].Origin, names[i], Colors[i], Colors[i]);

                    //args.Display.DrawPoint(PlanesLists[i][j].Origin, Rhino.Display.PointStyle.ControlPoint, height, Colors[i]);
                    //args.Display.DrawPoint(PlanesLists[i][j].Origin, Rhino.Display.PointStyle.Simple, Convert.ToInt16( CommonProps.tolerances*1000), Colors[i]);
                }



            }
        //base.DrawViewportMeshes(args);
    }


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
            get { return new Guid("5915a5bd-0d82-4f51-a9ce-8154bfc23c38"); }
        }
    }
}
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;

using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PTK
{

    public class PTK_3StructualAssembly : GH_Component
    {

        public PTK_3StructualAssembly()
          : base("Structual Assembly", "Str Assembly",
              "StructualAssembly",
              CommonProps.category, CommonProps.subcate3)
        {
            Message = CommonProps.initialMessage;
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddParameter(new Param_Assembly(), "Assembly", "A", "Assembly", GH_ParamAccess.item);
            pManager.AddParameter(new Param_Support(), "Supports", "S", "Supports", GH_ParamAccess.list);
            pManager.AddParameter(new Param_Load(), "Loads", "L", "Loads", GH_ParamAccess.list);

            pManager[1].Optional = true;
            pManager[2].Optional = true;
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.RegisterParam(new Param_Assembly(), "StructuralAssembly", "SA", "StructuralAssembly", GH_ParamAccess.item);
            // pManager.RegisterParam(new Param_Node(), "Nodes", "N", "Nodes included in the Assembly", GH_ParamAccess.list);
            pManager.AddLineParameter("Lines", "Lns", "only for V.0.3", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            /////////////////////////////////////////////////////////////////////////////////
            // variables
            /////////////////////////////////////////////////////////////////////////////////
            
            GH_Assembly gAssembly = null;
            Assembly assembly = null;
            List<GH_StructuralElement> gStrElems = new List<GH_StructuralElement>();
            List<StructuralElement> strElems = null;
            List<GH_Support> gSups = new List<GH_Support>();
            List<Support> sups = null;
            List<GH_Load> gLoads = new List<GH_Load>();
            List<Load> loads = null;

            // below: tempLines will be removed after V.0.3
            // just a temporal line element exporter to connect to Karamba
            List<Line> tempLines = new List<Line>(); 


            /////////////////////////////////////////////////////////////////////////////////
            // input
            /////////////////////////////////////////////////////////////////////////////////

            if (!DA.GetData(0, ref gAssembly))
            {
                assembly = new Assembly();
            }
            else
            {
                assembly = gAssembly.Value;
            }

            if (!DA.GetDataList(1, gSups))
            {
                sups = new List<Support>();
            }
            else
            {
                sups = gSups.ConvertAll(s => s.Value);
            }

            if (!DA.GetDataList(2, gLoads))
            {
                loads = new List<Load>();
            }
            else
            {
                loads = gLoads.ConvertAll(l => l.Value);
            }


            /////////////////////////////////////////////////////////////////////////////////
            // solve
            /////////////////////////////////////////////////////////////////////////////////

            StructuralAssembly strAssembly = new StructuralAssembly(assembly);

            foreach(Element1D e in assembly.Elements)
            {
                strAssembly.AddSElement(new StructuralElement(e));

                var paramList = strAssembly.Assembly.SearchNodeParamsAtElement(e);

                for (int i = 0; i < paramList.Count - 1; i++)
                {
                    Point3d spt = e.BaseCurve.PointAt(paramList[i]);
                    Point3d ept = e.BaseCurve.PointAt(paramList[i + 1]);
                    tempLines.Add(new Line(spt, ept));
                }
            }
            /*
            foreach(StructuralElement sElem in strAssembly.SElements)
            {
                strAssembly.AddSElement(sElem);
            }
            */
            foreach (Support s in sups)
            {
                strAssembly.AddSupport(s);
            }
            foreach (Load l in loads)
            {
                strAssembly.AddLoad(l);
            }

            Assembly upcastedAssembly = strAssembly;

            /////////////////////////////////////////////////////////////////////////////////
            // output
            /////////////////////////////////////////////////////////////////////////////////

            DA.SetData(0, new GH_Assembly(upcastedAssembly));

            // below: temporal output at V.0.3
            DA.SetDataList(1, tempLines);

        }

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return PTK.Properties.Resources.StrAssemble;
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("1DCC99B5-2BAE-4783-A431-02C402B09C28"); }
        }
    }
}

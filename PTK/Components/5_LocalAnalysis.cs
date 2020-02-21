using System;
using System.Collections.Generic;
using System.Linq;
using Grasshopper.Kernel;
using Rhino.Geometry;

namespace PTK
{
    public class PTK_LocalAnalysis : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the PTK_C_03 class.
        /// </summary>
        public PTK_LocalAnalysis()
          : base("Local Analysis (PTK)", "Local Analysis",
              "Local Analysis",
              CommonProps.category, CommonProps.subcate5)
        {
            Message = CommonProps.initialMessage;
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddIntegerParameter("n1", "n1", "number of dowels in the row", GH_ParamAccess.item,4);
            pManager.AddIntegerParameter("n2", "n2", "number of rows", GH_ParamAccess.item,6);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("report", "rep", "PTK Local Analysis", GH_ParamAccess.list);
            pManager.AddNumberParameter("FvRd", "FRd", "The force capacity form the group of dowels", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            int nx = 0;
            int ny = 0;

            DA.SetData(0,  nx);
            DA.SetData(1,  ny);

            List<string> outputs = new List<string>();

            double gammaM = 1.3;
            double kmod = 1.0; //modification factor, due to load duration and moisture content

            double ang = 0; //angle
            double sin = System.Math.Sin(ang);
            double cos = System.Math.Cos(ang);

            double d = 12; //mm dowel dimension
            double fukd = 800; //N/mm2 steel dowel prpoerties
            double nsp = 4; // number of steel plates
            double t = 8; //mm steel plate thickness

            double fyp = 355; //steel plate properties
            double fup = 510; //steel plate properties

            double ti = 110; //distance between steel plates
            double ty = 130; //distanbe between steel plate and edge of the beam

            double ht = 500; //height of the timber beam
            double at = 0; //angle of the grains

            double ncol = Convert.ToDouble(nx); //number of dowels in the row
            double nrow = Convert.ToDouble(ny); //number of dowel rows

            double a1 = 100;
            double a2 = 60;
            double a3t = 100;

            double qkl = 390; //characteristic density of timber

            double fhk = 0.082 * (1 - 0.01 * d) * qkl; //embedment strength for predilled holes EC5 eq 8.16
            double k90 = 1.35 + (0.015 * d); //factor for softwood
            double fhak = fhk / (k90 * sin + cos); //embedment strength for predilled holes EC5 eq 8.16



            outputs.Add("fhk = " + fhk);
            outputs.Add("k90 = " + k90);
            outputs.Add("fhak = " + fhak);

            //find the minimal t
            double t1 = new List<double>() { ti, ty }.Min();
            double t2 = new List<double>() { ti, ty }.Max();
            double fh1k = fhk; //embedment strength for predilled holes EC5 eq 8.16
            double fh2k = fhk; //embedment strength for predilled holes EC5 eq 8.16
            outputs.Add("t1 = " + t1);
            outputs.Add("t2 = " + t2);
            //joints in double shear should be calculated according to f,g,h mode type
            double MyRk = 0.3 * fukd * Math.Pow(d, 2.6); //characteristic yield moment

            //characterisitc load carrying capacity prer
            double FvRk_f = fhk * ti * d;
            double FvRk_g = fhk * ti * d * (Math.Sqrt(2 + 4 * MyRk / (fhk * ti * ti * d) - 1));
            double FvRk_h = 2.3 * Math.Sqrt(MyRk * fh1k * d);
            double FvRk_il = 0.5 * fh2k * ti * d;
            double FvRk_k = 1.15 * Math.Sqrt(2 * MyRk * fh2k * d);
            double FvRk_m = 2.3 * Math.Sqrt(MyRk * fh2k * d);
            List<double> Fvrks = new List<double>();
            //Fvrks.Add(FvRk_f);
            //Fvrks.Add(FvRk_g);
            Fvrks.Add(FvRk_h);
            //Fvrks.Add(FvRk_il);
            //Fvrks.Add(FvRk_k);
            Fvrks.Add(FvRk_m);
            double FvRk = Fvrks.Min();

            //outputs.Add("MyRk = " + MyRk);
            //outputs.Add("FvRk_f = " + FvRk_f);
            //outputs.Add("FvRk_g = " + FvRk_g);
            outputs.Add("FvRk_h = " + FvRk_h);
            //outputs.Add("FvRk_il = " + FvRk_il);
            //outputs.Add("FvRk_k = " + FvRk_k);
            outputs.Add("FvRk_m = " + FvRk_m);
            outputs.Add("FvRk = " + FvRk);

            double tnetto = ti * (nsp - 1) + 2 * ty;
            double tbrutto = tnetto + (t + 2) * nsp;

            outputs.Add("tnetto = " + tnetto);
            outputs.Add("tbrutto = " + tbrutto);

            double n1 = ncol;
            double n2 = Math.Pow(ncol, 0.9) * Math.Pow(a1 / (13 * d), 0.25);
            var ns = new List<double>();
            ns.Add(n1);
            ns.Add(n2);
            double nef = ns.Min();

            outputs.Add("nef=" + nef);

            double Fvk = FvRk * nsp * 2;
            outputs.Add("Fvk=" + Fvk);

            double FvkR = FvRk * nsp * 2 * nef;
            outputs.Add("FvkR=" + FvkR);

            double FvRd = FvkR * nrow * kmod / gammaM;
            outputs.Add("FvRd=" + FvRd);

            //publish data
            DA.SetDataList(0, outputs);
            DA.SetData(1, FvRd);
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
                return PTK.Properties.Resources.LocalAnalysis;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("9b623fe7-191b-4163-800c-2cb85fef0c2b"); }
        }
    }
}
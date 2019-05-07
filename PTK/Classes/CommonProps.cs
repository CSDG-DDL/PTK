using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;

namespace PTK
{
    public static class CommonProps
    {
        public static double tolerances = Rhino.RhinoDoc.ActiveDoc.ModelAbsoluteTolerance;
          
        public static readonly string category = "Reindeer";
        public static readonly string subcate0 = "   Param";
        public static readonly string subcate1 = "Material"; //Is moved into Element, Exposure 3
        public static readonly string subcate2 = "  Element";
        public static readonly string subcate3 = " Assembly";
        public static readonly string subcate4 = "Model";
        public static readonly string subcate5 = "Structual";
        public static readonly string subcate6 = "Visualize";
        public static readonly string subcate7 = "Export";
        public static readonly string subcate8 = " Deconstruct";
        public static readonly string subcate9 = "Tool";
        public static readonly string subcate10 = "DetailGroupRules";
        public static readonly string subcate11 = "TimberDetailing";
        public static readonly string subcate12 = "ElementGlobalAlignment"; //Is moved into Element, Exporsure 2
        public static readonly string initialMessage = "PTK Ver.0.5";
    }

    // ----------------
    //     Delegate    
    // ----------------
    public delegate Plane GenerateNodeGroupPlane(Detail detail);
    public delegate bool CheckGroupDelegate(Detail detail);
    public delegate Vector3d ElementAlignment(Curve curve);
    public delegate PerformedProcess PerformTimberProcessDelegate(BTLPartGeometry _BTLPartGeometry, ManufactureMode _mode);


    // -------------
    //     enums    
    // -------------

    public enum DecimalSeparator
    {
        error,period,comma
    }
    public enum AlignmentAnchorVert
    {
        Center,Top,Bottom
    }
    public enum AlignmentAnchorHori
    {
        Center, Left, Right
    }
    public enum DetailType
    {
        NULL, LType, TType, XType
    }
    public enum ManufactureMode
    {
        BTL, NURBS, BOTH, NONE
    }

    // -------------
    //     Function    
    // -------------

    

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTK
{
    public class PTK_StructuralAnalysis
    {
        /// <summary>
        /// This class hold the results of utilization calculation
        /// It has no methods, it is just holding the ouptu data from dimensioning componenent
        /// </summary>
        #region fields
        public string elementIDname = "element ID=";
        public int elementID { get; set; }

        public string elementText0 = "element length=";
        public double elementLength { get; set; }
        public string elementText1 = "element height=";
        public double elementHeight { get; set; }
        public string elementText2 = "element width=";
        public double elementWidth { get; set; }

        public string elementText3 = "area of the cross section=";
        public double elementArea { get; set; }
        public string elementText4 = "moment of inertia in 1. direction=";
        public double elementMomentOfInertiaDir1 { get; set; }
        public string elementText5 = "moment of inertia in 2. direction=";
        public double elementMomentOfInertiaDir2 { get; set; }
        public string elementText6 = "radius of gyration in 1. direction=";
        public double elementRadiusOfGyrationDir1 { get; set; }
        public string elementText7 = "radius of gyration in 2. direction=";
        public double elementRadiusOfGyrationDir2 { get; set; }

        public string elementText8 = "maximum axial force in element=";
        public double elementAxialForce { get; set; }
        public string elementText9 = "maximum shear force in 1. direction in element=";
        public double elementShearForce1 { get; set; }
        public string elementText10 = "maximum shear force in 2. direction in element=";
        public double elementShearForce2 { get; set; }
        public string elementText11 = "maximum torsion in element=";
        public double elementTorsion { get; set; }
        public string elementText12 = "maximum bending in 1. direction in element=";
        public double elementBending1 { get; set; }
        public string elementText13 = "maximum bending in 2. direction in element=";
        public double elementBending2 { get; set; }

        public string elementText14 = "element effective length in 1.direction=";
        public double elementEffectiveLengthDir1 { get; set; }
        public string elementText15 = "element effective length in 2.direction=";
        public double elementEffectiveLengthDir2 { get; set; }
        public string elementText16 = "element slenderness in 1.direction=";
        public double elementSlendernessRatioDir1 { get; set; }
        public string elementText17 = "element slenderness in 2.direction=";
        public double elementSlendernessRatioDir2 { get; set; }
        public string elementText18 = "element euler force in 1.direction=";
        public double elementEulerForceDir1 { get; set; }
        public string elementText19 = "element euler force in 2.direction=";
        public double elementEulerForceDir2 { get; set; }
        public string elementText20 = "element relative slenderness in 1.direction=";
        public double elementSlendernessRelativeDir1 { get; set; }
        public string elementText21 = "element relative slenderness in 2.direction=";
        public double elementSlendernessRelativeDir2 { get; set; }
        public string elementText22 = "element instability factor in 1.direction=";
        public double elementInstabilityFactorDir1 { get; set; }
        public string elementText23 = "element instability factor in 2.direction=";
        public double elementInstabilityFactorDir2 { get; set; }
        public string elementText24 = "element buckling strength in 1.direction=";
        public double elementBucklingStrengthDir1 { get; set; }
        public string elementText25 = "element buckling strength in 2.direction=";
        public double elementBucklingStrengthDir2 { get; set; }

        public string elementText26 = "element utilization according to compression=";
        public double elementCompressionUtilization { get; set; }
        public string elementText27 = "element utilization according to compression (considering grain angle)=";
        public double elementCompressionUtilizationAngle { get; set; }
        public string elementText28 = "element utilization according to tension=";
        public double elementTensionUtilization { get; set; }
        public string elementText29 = "element utilization according to bending in 1. and 2. direction=";
        public double elementBendingUtilization { get; set; }
        public string elementText30 = "element utilization according to combined bending and axial force=";
        public double elementCombinedBendingAndAxial { get; set; }

        #endregion

        #region constructors
        public PTK_StructuralAnalysis(int _elementID)
        {
            elementID = _elementID;               // 
        }
        public PTK_StructuralAnalysis()
        {
            // 
        }
        #endregion
    }

}
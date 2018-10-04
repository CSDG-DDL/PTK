using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace PTK
{
    public static class CommonFunctions
    {
        //Function for correcting differences in decimal notation
        public static string ConvertCommaToPeriodDecimal(string _txt, bool _reverse = false)
        {
            if (!_reverse)
            {
                return _txt.Replace(',', '.');  //Comma to Period
            }
            else
            {
                return _txt.Replace('.', ',');  //Period to Comma
            }
        }

        //Return the Decimal Separator in the use environment
        public static DecimalSeparator FindDecimalSeparator()
        {
            string txtFindLocale = string.Format("{0}", 1.1);

            if (txtFindLocale == "1.1") return DecimalSeparator.period;
            else if (txtFindLocale == "1,1") return DecimalSeparator.comma;
            else return DecimalSeparator.error;
        }

        //Return scale ratio from scale of Rhino to specified scale
        public static double ConversionUnit(Rhino.UnitSystem _toUnitSystem)
        {
            Rhino.RhinoDoc doc = Rhino.RhinoDoc.ActiveDoc;
            Rhino.UnitSystem fromUnitSystem = doc.ModelUnitSystem;
            return Rhino.RhinoMath.UnitScale(fromUnitSystem, _toUnitSystem);
        }

    }
}
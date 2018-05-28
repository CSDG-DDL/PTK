﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTK
{
    public static class CommonProps
    {
        #region fields
        public static double tolerances = 0.001;
        public static string category = "PTK";
        public static string subcat1 = "Assemble";
        public static string subcat2 = "Materialize";
        public static string subcat3 = "Detail";
        public static string subcat4 = "Structure";
        public static string subcat5 = "Utility";
        public static string initialMessage = "PTK Ver.0.5";
        // public const string 
        #endregion

        #region constructors
        #endregion

        #region properties
        #endregion

        #region methods
        // simple decimal separator checker
        public static string FindDecimalSeparator()
        {
            string txtFindLocale = string.Format("{0}", 1.1);
            
            string message = "";

            if (txtFindLocale == "1.1") message += "period";
            else if (txtFindLocale == "1,1") message += "comma";
            else message += txtFindLocale;

            return message;
        }
        #endregion
        
    }


}

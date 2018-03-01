﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTK
{
    public class Material
    {

        #region fields
        private static int MaterialCount = 2000;  //ID Corresponds to the name of the component
        private string materialName;
        private int materialId;
        private double youngModulus;
        private double density;
        private double price;
        private string currency;
        #endregion

        #region constructors

        public Material() { }

        public Material(string _materialName, double _materialId, double _younModulus, double _density)
        {

        }


        #endregion

        #region properties
        public string Materialname
        {
            get
            {
                return materialName;
            }
            set
            {
                materialName = value;
            }
        }
        public double YoungModulus
        {
            get
            {
                return youngModulus;
            }
            set
            {
                youngModulus = value;
            }
        }
        public double Density
        {
            get
            {
                return density;
            }
            set
            {
                density = value;
            }
        }
        public double Price
        {
            get
            {
                return price;
            }
            set
            {
                price = value;
            }
        }
        public string Currency
        {
            get
            {
                return currency;
            }
            set
            {
                currency = value;
            }
        }

        #endregion

        #region methods

        #endregion
    }
}


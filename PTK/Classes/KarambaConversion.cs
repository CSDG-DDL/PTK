using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Karamba;
using Rhino.Geometry;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;


namespace PTK
{
    public static class KarambaConversion
    {
        public static Karamba.Models.Model BuildModel(StructuralAssembly _strAssembly)
        {
            var points = new List<Rhino.Geometry.Point3d>();
            var materialMap = new Dictionary<MaterialProperty, Karamba.Materials.FemMaterial>();
            var crosecMap = new Dictionary<CrossSection, Karamba.CrossSections.CroSec>();
            var supports = new List<Karamba.Supports.Support>();
            var loads = new List<Karamba.Loads.Load>();
            var elems = new List<Karamba.Elements.GrassElement>();
            var elemset = new List<Karamba.Utilities.ElemSet>();


            points = _strAssembly.Assembly.Nodes.ConvertAll(n => n.Point);
            // * CommonProps.ConversionUnit(Rhino.UnitSystem.Meters));
            
            foreach(KeyValuePair<CrossSection,MaterialProperty> kvp in _strAssembly.Assembly.CrossSectionMap)
            {
                if (!materialMap.ContainsKey(kvp.Value))
                {
                    materialMap.Add(kvp.Value, MakeFemMaterial(kvp.Value));
                }
                crosecMap.Add(kvp.Key, MakeCrossSection(kvp.Key, materialMap[kvp.Value]));
            }

            foreach(Support s in _strAssembly.Supports)
            {
                var sup = new Karamba.Supports.Support(s.FixingPlane.Origin, s.Conditions, s.FixingPlane);
                sup.loadcase = s.LoadCase;
                supports.Add(sup);
            }

            foreach(Load l in _strAssembly.Loads)
            {
                if(l is PointLoad pl)
                {
                    var load = new Karamba.Loads.PointLoad(pl.Point, pl.ForceVector, pl.MomentVector, pl.LoadCase, true);
                    loads.Add(load);
                }
                else if(l is GravityLoad gl)
                {
                    var load = new Karamba.Loads.GravityLoad(gl.GravityVector, gl.LoadCase);
                    loads.Add(load);
                }
            }

            foreach(Element1D e in _strAssembly.Assembly.Elements)
            {
                var paramList = _strAssembly.Assembly.SearchNodeParamsAtElement(e);
                for (int i = 0; i <= paramList.Count-2; i++ )
                {
                    var elem = new Karamba.Elements.GrassBeam(e.BaseCurve.PointAt(paramList[i]), e.BaseCurve.PointAt(paramList[i + 1]));
                    //var s = crosecMap[e.Element.Sections[0]];
                    //s.ecce_loc = new Vector3d(e.Element.Align.OffsetY, e.Element.Align.OffsetZ,0);
                    elem.crosec = crosecMap[e.CrossSections[0]]; 
                    //At present it is supposed to be one section material
                    //elem.z_ori
                    elems.Add(elem);
                }
            }

            double limitDist = 0.005;
            var modelBuilder = new Karamba.Models.ModelBuilder(limitDist);
            var model = modelBuilder.build(
                points,
                materialMap.Values.ToList(),
                crosecMap.Values.ToList(),
                supports,
                loads,
                elems,
                elemset
            );

            return model;
        }

        private static Karamba.Materials.FemMaterial MakeFemMaterial(MaterialProperty _matProp)
        {
            var fm = new Karamba.Materials.FemMaterial_Isotrop(
                "familyName",
                _matProp.Name,
                _matProp.EE0gmean,
                _matProp.EE0gmean*0.5,
                _matProp.GGgmean,
                _matProp.Rhogk*10,
                _matProp.Ft0gk,
                0.0000001,
                null/*color*/
                );

            return fm;
        }

        private static Karamba.CrossSections.CroSec MakeCrossSection(CrossSection _sec, Karamba.Materials.FemMaterial _mat)
        {
            Karamba.CrossSections.CroSec sec;
            if (_sec is RectangleCroSec rectSec)
            {
                sec = new Karamba.CrossSections.CroSec_Trapezoid(
                    "familyName",
                    _sec.Name,
                    "Country",
                    null/*color*/,
                    _mat,
                    rectSec.GetHeight() * CommonFunctions.ConversionUnit(Rhino.UnitSystem.Centimeters),
                    rectSec.GetWidth() * CommonFunctions.ConversionUnit(Rhino.UnitSystem.Centimeters),
                    rectSec.GetWidth() * CommonFunctions.ConversionUnit(Rhino.UnitSystem.Centimeters)
                    );
                //Karamba.Utilities.UnitsConversionFactory unitsConversionFactory = Karamba.Utilities.UnitsConversionFactories.Conv();
                //sec.ecce_loc = unitsConversionFactory.cm();
            }
            else
            {
                sec = null;
            }
            return sec;
        }
    }
}

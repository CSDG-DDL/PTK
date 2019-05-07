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
        public static Karamba.Models.Model BuildModelMeters(StructuralAssembly _strAssembly)
        {
            var points = new List<Point3d>();
            var materialMap = new Dictionary<MaterialProperty, Karamba.Materials.FemMaterial>();
            var crosecMap = new Dictionary<CompositeNew, Karamba.CrossSections.CroSec>();
            var supports = new List<Karamba.Supports.Support>();
            var loads = new List<Karamba.Loads.Load>();
            var elems = new List<Karamba.Elements.GrassElement>();
            var elemset = new List<Karamba.Utilities.ElemSet>();


            points = _strAssembly.Nodes.ConvertAll(n => n.Point );

            foreach (var elem in _strAssembly.Elements)
            {
                if (!materialMap.ContainsKey(elem.Composite.MaterialProperty))
                {
                    materialMap.Add(elem.Composite.MaterialProperty, MakeFemMaterial(elem.Composite.MaterialProperty));
                }
                crosecMap.Add(elem.Composite , MakeCrossSection(elem.Composite, materialMap[elem.Composite.MaterialProperty]));

            }
            

            foreach(Support s in _strAssembly.Supports)
            {
                var sup = new Karamba.Supports.Support(s.FixingPlane.Origin,
                    s.Conditions,
                    new Plane(s.FixingPlane.Origin , s.FixingPlane.XAxis, s.FixingPlane.YAxis));
                sup.loadcase = s.LoadCase;
                supports.Add(sup);
            }

            foreach(Load l in _strAssembly.Loads)
            {
                if(l is PointLoad pl)
                {
                    var load = new Karamba.Loads.PointLoad(
                        pl.Point, 
                        pl.ForceVector, pl.MomentVector, pl.LoadCase, true);
                    loads.Add(load);
                }
                else if(l is GravityLoad gl)
                {
                    var load = new Karamba.Loads.GravityLoad(gl.GravityVector, gl.LoadCase);
                    loads.Add(load);
                }
            }

            foreach(Element1D e in _strAssembly.Elements)
            {
                var paramList = _strAssembly.SearchNodeParamsAtElement(e);
                for (int i = 0; i <= paramList.Count-2; i++ )
                {
                    var elem = new Karamba.Elements.GrassBeam(
                        e.BaseCurve.PointAt(paramList[i]) , 
                        e.BaseCurve.PointAt(paramList[i + 1]) 
                        );
                    //複合断面暫定対応
                    if (e.Composite is CompositeNew comp)
                    {
                        elem.crosec = crosecMap[comp];
                    }
                    else
                    {
                        elem.crosec = crosecMap[e.Composite];
                    }
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
        public static Karamba.Models.Model BuildModelMilimeters(StructuralAssembly _strAssembly)
        {
            var points = new List<Point3d>();
            var materialMap = new Dictionary<MaterialProperty, Karamba.Materials.FemMaterial>();
            var crosecMap = new Dictionary<CompositeNew, Karamba.CrossSections.CroSec>();
            var supports = new List<Karamba.Supports.Support>();
            var loads = new List<Karamba.Loads.Load>();
            var elems = new List<Karamba.Elements.GrassElement>();
            var elemset = new List<Karamba.Utilities.ElemSet>();


            points = _strAssembly.Nodes.ConvertAll(n => n.Point * CommonFunctions.ConversionUnit(Rhino.UnitSystem.Millimeters));

            foreach (var elem in _strAssembly.Elements)
            {
                if (!materialMap.ContainsKey(elem.Composite.MaterialProperty))
                {
                    materialMap.Add(elem.Composite.MaterialProperty, MakeFemMaterial(elem.Composite.MaterialProperty));
                }
                crosecMap.Add(elem.Composite, MakeCrossSection(elem.Composite, materialMap[elem.Composite.MaterialProperty]));

            }

            foreach (Support s in _strAssembly.Supports)
            {
                var sup = new Karamba.Supports.Support(s.FixingPlane.Origin * CommonFunctions.ConversionUnit(Rhino.UnitSystem.Millimeters),
                    s.Conditions,
                    new Plane(s.FixingPlane.Origin * CommonFunctions.ConversionUnit(Rhino.UnitSystem.Millimeters), s.FixingPlane.XAxis, s.FixingPlane.YAxis));
                sup.loadcase = s.LoadCase;
                supports.Add(sup);
            }

            foreach (Load l in _strAssembly.Loads)
            {
                if (l is PointLoad pl)
                {
                    var load = new Karamba.Loads.PointLoad(
                        pl.Point * CommonFunctions.ConversionUnit(Rhino.UnitSystem.Millimeters),
                        pl.ForceVector, pl.MomentVector, pl.LoadCase, true);
                    loads.Add(load);
                }
                else if (l is GravityLoad gl)
                {
                    var load = new Karamba.Loads.GravityLoad(gl.GravityVector, gl.LoadCase);
                    loads.Add(load);
                }
            }

            foreach (Element1D e in _strAssembly.Elements)
            {
                var paramList = _strAssembly.SearchNodeParamsAtElement(e);
                for (int i = 0; i <= paramList.Count - 2; i++)
                {
                    var elem = new Karamba.Elements.GrassBeam(
                        e.BaseCurve.PointAt(paramList[i]) * CommonFunctions.ConversionUnit(Rhino.UnitSystem.Millimeters),
                        e.BaseCurve.PointAt(paramList[i + 1]) * CommonFunctions.ConversionUnit(Rhino.UnitSystem.Millimeters)
                        );
                    //複合断面暫定対応
                    if (e.Composite is CompositeNew comp)
                    {
                        elem.crosec = crosecMap[comp];
                    }
                    else
                    {
                        elem.crosec = crosecMap[e.Composite];
                    }
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
                _matProp.EE0gmean *0.1*10000,          /* N/mm2 To kN/cm2 */
                _matProp.EE0gmean*0.5 *0.1*10000,      /* N/mm2 To kN/cm2 */
                _matProp.GGgmean *0.1*10000,           /* N/mm2 To kN/cm2 */
                _matProp.Rhogk *0.01,                   /* kg/m3 To kN/m3  コレは同じ*/
                _matProp.Ft0gk *0.1*10000,             /* N/mm2 To kN/cm2 */
                0.0000001,
                null/*color*/
                );

            return fm;
        }

        private static Karamba.CrossSections.CroSec MakeCrossSection(CompositeNew _sec, Karamba.Materials.FemMaterial _mat)
        {
            Karamba.CrossSections.CroSec sec;
            if (_sec is CompositeNew rectSec)
            {
                sec = new Karamba.CrossSections.CroSec_Trapezoid(
                    "familyName",
                    rectSec.CompositeName,
                    "Country",
                    null/*color*/,
                    _mat,
                    rectSec.HeightSimplified * CommonFunctions.ConversionUnit(Rhino.UnitSystem.Millimeters),
                    rectSec.WidthSimplified * CommonFunctions.ConversionUnit(Rhino.UnitSystem.Millimeters),
                    rectSec.WidthSimplified * CommonFunctions.ConversionUnit(Rhino.UnitSystem.Millimeters)
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

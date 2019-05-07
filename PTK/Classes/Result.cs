using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grasshopper.Kernel;
using Rhino.Geometry;

namespace PTK.Classes
{
    public class Result
    {
        // --- field ---
        public Point3d positionPoint { get; set; }
        public double position { get; set; }
        public int karambaElementID { get; set; }
        public int loadcase { get; set; }

        public double utilCompression { get; set; }
        public double utilTension { get; set; }
        public double utilShear { get; set; }
        public double utilBending { get; set; }
        public double utilTorsion { get; set; }


        // --- constructors --- 

        public Result()
        {
            // empty class

        }
        public Result(double _position, int _karambaElementID, int _loadcase, double _utilCompression, 
                double _utilTension, double _utilShear, double _utilBending, double _utilTorsion)
        {
            position = _position;
            karambaElementID = _karambaElementID;
            loadcase = _loadcase;
            utilCompression = _utilCompression;
            utilTension = _utilTension;
            utilShear = _utilShear;
            utilBending = _utilBending;
            utilTorsion = _utilTorsion;
        }

    }
}

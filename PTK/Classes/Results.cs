using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;

namespace PTK
{
    public class Results
    {
        //forces from FEM
        private int loadCase { get; set; }
        public List<Force> forces { get; set; }

        public Force maxCompression { get; set; }
        public Force maxTension { get; set; }
        public Force maxShearDir1 { get; set; }
        public Force maxShearDir2 { get; set; }
        public Force maxBendingDir1 { get; set; }
        public Force maxBendingDir2 { get; set; }
        public Force maxTorsion { get; set; }

        public Results()
        {
            //empty constructor
        }
        public Results(int _loadCase)
        {
            loadCase = _loadCase;
        }



    }
}

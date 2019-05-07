using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTK
{
    public class StructuralResult
    {
        // Utilization
        public List<Result> results;
        public double CompressionUtilization;
        public double CompressionUtilizationAngle;
        public double TensionUtilization;
        public double BendingUtilization;
        public double CombinedBendingAndAxial;

        public StructuralResult()
        {
            //empty constructor
        }

    }
}

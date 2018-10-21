using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grasshopper.Kernel;
using Rhino.Geometry;

namespace PTK
{
    public class DetailingGroup
    {
        // --- field ---
        public string Name { get; private set; }
        public List<Detail> Details { get; private set; }
        public List<Plane> NodeGroupPlanes { get; private set; }

        // --- constructors ---
        public DetailingGroup(string _name)
        {
            Name = _name;
            Details = new List<Detail>();
            ///Details[0].ElementsPriorityMap.Keys.ToList()[0].
        }
        public DetailingGroup(string _name, List<Detail> _details, List<Plane> _nodeGroupPlanes)
        {
            Name = _name;
            Details = _details;
            NodeGroupPlanes = _nodeGroupPlanes;
            ///Details[0].ElementsPriorityMap.Keys.ToList()[0].
        }
    }
}

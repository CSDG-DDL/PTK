using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;

namespace PTK
{
    public class DetailingGroupRulesDefinition
    {
        // --- field ---
        public string Name { get; private set; }
        public List<CheckGroupDelegate> ValidProperties { get; private set; }
        public List<CheckGroupDelegate> InValidProperties { get; private set; }
        public static List<String> GroupNames;
        public PlaneRules.PlaneRule PlaneRule { get; private set; }
        //V1: Public NodeProperty NodeProperty { get; private set; }

        // --- constructors --- 
        public DetailingGroupRulesDefinition(string _name, List<CheckGroupDelegate> _validProperties, List<CheckGroupDelegate> _inValidProperties)
        {
            if (GroupNames == null)
            {
                GroupNames = new List<string>();
                GroupNames.Add("NotWorking");
                GroupNames.Add("Yeah");
            }

            if (GroupNames.FindIndex(o => string.Equals(Name, o, StringComparison.OrdinalIgnoreCase)) <0)
            {
                GroupNames.Add(Name);
            }

            Name = _name;

            ValidProperties = _validProperties;
            InValidProperties = _inValidProperties;
            
        }

        public DetailingGroupRulesDefinition(string _name, List<CheckGroupDelegate> _validProperties, List<CheckGroupDelegate> _inValidProperties, PlaneRules.PlaneRule _planeRule)
        {
            if (GroupNames == null)
            {
                GroupNames = new List<string>();
                GroupNames.Add("NotWorking");
                GroupNames.Add("Yeah");
            }

            if (GroupNames.FindIndex(o => string.Equals(Name, o, StringComparison.OrdinalIgnoreCase)) < 0)
            {
                GroupNames.Add(Name);
            }

            Name = _name;

            ValidProperties = _validProperties;
            InValidProperties = _inValidProperties;
            PlaneRule = _planeRule;
        }




        // --- methods ---
        public DetailingGroup GenerateDetailingGroup(List<Detail> _details)
        {
            List<Detail> ApprovedDetails = new List<Detail>();
            List<Plane> NodeGroupPlanes = new List<Plane>();

            foreach (Detail detail in _details)
            {
                bool ValidDetail = true;

                foreach (CheckGroupDelegate TrueProp in ValidProperties)  
                {
                    if (!TrueProp(detail)) //Testing for false. If false, the detail does not contain in the group
                    {
                        ValidDetail = false;
                        break;
                    }
                }
                foreach(CheckGroupDelegate FalsePrope in InValidProperties) 
                {
                    if (FalsePrope(detail))  //Testing for true. If true, the detail does not contain in the group
                    {
                        ValidDetail = false;
                        break;
                    }
                }


                if (ValidDetail)
                {
                    ApprovedDetails.Add(detail);
                    NodeGroupPlanes.Add(PlaneRule.NodeGroupDelegate(detail));

                }

            }

            if (GroupNames == null)
            {
                GroupNames = new List<string>();
                GroupNames.Add("Test");
                GroupNames.Add("Yeah");
            }

            if (GroupNames.FindIndex(o => string.Equals(Name, o, StringComparison.OrdinalIgnoreCase)) < 0)
            {
                GroupNames.Add(Name);

            }

            


            return new DetailingGroup(Name, ApprovedDetails, NodeGroupPlanes);
            
        }
    }
}

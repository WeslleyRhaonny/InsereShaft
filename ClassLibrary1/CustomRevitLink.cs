using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
namespace ClassLibrary1
{
    public class CustomRevitLink
    {
        public string Name { get; }
        public RevitLinkType LinkType { get; }
        public List<RevitLinkInstance> LinkInstances { get; }

        public CustomRevitLink(string name, RevitLinkType linkType)
        {
            Name = name;
            LinkType = linkType;
            LinkInstances = new List<RevitLinkInstance>();
        }

        public void AddLinkInstance(RevitLinkInstance linkInstance)
        {
            LinkInstances.Add(linkInstance);
        }
    }
}

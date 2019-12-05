using System;
using System.Linq;
using Xbim.Ifc;
using Xbim.Ifc4.Kernel;

namespace ifc2gltf
{
    public static class IfcInfo
    {
        public static void PrintSemantics(IfcStore model)
        {
            var ifcProject = model.FederatedInstances.OfType<IfcProject>().FirstOrDefault();
            // print buildings, site, storeys, spaces...
            Console.WriteLine("Buildings: " + ifcProject.Buildings.Count());
            var building = ifcProject.Buildings.FirstOrDefault();
            Console.WriteLine("description: " + building.BuildingAddress.Description);
            var storeys = building.BuildingStoreys;
            var site = ifcProject.Sites.FirstOrDefault();
            Console.WriteLine("Location: " + site.RefLongitude.Value.AsDouble + ", " + site.RefLatitude.Value.AsDouble);

            Console.WriteLine("storeys: " + storeys.Count());
            foreach (var storey in storeys)
            {
                Console.WriteLine(storey.LongName);

                foreach (var space in storey.Spaces)
                {
                    Console.WriteLine(space.LongName);
                }
            }
        }

    }
}

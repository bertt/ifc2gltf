using System.IO;
using Xbim.Common.Geometry;
using Xbim.Common.XbimExtensions;
using Xbim.Ifc;
using Xbim.ModelGeometry.Scene;
using System;
using System.Linq;
using Xbim.Ifc4.Kernel;

namespace ConsoleApp7
{
    class Program
    {
        static void Main(string[] args)
        {
            const string file = @"spaces_all.ifc";
            var model = IfcStore.Open(file);
            var ifcProject = model.FederatedInstances.OfType<IfcProject>().FirstOrDefault();
            
            // print buildings, storeys, spaces...
            Console.WriteLine("Buildings: " + ifcProject.Buildings.Count());
            var building = ifcProject.Buildings.FirstOrDefault();
            Console.WriteLine("description: " + building.BuildingAddress.Description);
            var storeys = building.BuildingStoreys;
            Console.WriteLine("storeys: " + storeys.Count());
            foreach(var storey in storeys)
            {
                Console.WriteLine(storey.LongName);

                foreach(var space in storey.Spaces)
                {
                    Console.WriteLine(space.LongName);
                }
            }

            // read geometries...
            var context = new Xbim3DModelContext(model);
            context.CreateContext();
            var geoms = context.ShapeGeometries().ToList();
            Console.WriteLine("Geometries:" + geoms.Count);
            foreach(var geom in geoms)
            {
                var data = ((IXbimShapeGeometryData)geom).ShapeData;

                using (var stream = new MemoryStream(data))
                {
                    using (var reader = new BinaryReader(stream))
                    {
                        var mesh = reader.ReadShapeTriangulation();
                        Console.WriteLine("Faces: " + mesh.Faces.Count + " vertices: " + mesh.Vertices.Count);

                        // todo: create glTF...
                    }
                }
            }

        }
    }
}

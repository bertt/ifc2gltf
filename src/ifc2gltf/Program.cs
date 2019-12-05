using System.IO;
using Xbim.Common.Geometry;
using Xbim.Common.XbimExtensions;
using Xbim.Ifc;
using Xbim.ModelGeometry.Scene;
using System;
using System.Linq;
using Xbim.Ifc4.Kernel;
using SharpGLTF.Geometry;
using VERTEX = SharpGLTF.Geometry.VertexTypes.VertexPosition;
using SharpGLTF.Materials;
using System.Numerics;
using System.Diagnostics;

namespace ConsoleApp7
{
    class Program
    {
        static void Main(string[] args)
        {
            const string file = @"spaces_all.ifc";
            var model = IfcStore.Open(file);
            // PrintSemantics(model);

            // read geometries...
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var context = new Xbim3DModelContext(model);
            context.CreateContext();
            var geoms = context.ShapeGeometries().ToList();
            Console.WriteLine("Geometries:" + geoms.Count);

            var mesh = new MeshBuilder<VERTEX>("mesh");
            var material1 = new MaterialBuilder()
               .WithDoubleSide(true)
               .WithMetallicRoughnessShader()
               .WithChannelParam("BaseColor", new Vector4(1, 0, 0, 1));


            foreach (var geom in geoms)
            {
                var data = ((IXbimShapeGeometryData)geom).ShapeData;

                using (var stream = new MemoryStream(data))
                {
                    using (var reader = new BinaryReader(stream))
                    {
                        var meshBim = reader.ReadShapeTriangulation();
                        // Console.WriteLine("Faces: " + meshBim.Faces.Count + " vertices: " + meshBim.Vertices.Count);

                        foreach (var face in meshBim.Faces)
                        {
                            var indeces = face.Indices;

                            for (var triangle = 0; triangle < face.TriangleCount; triangle++)
                            {
                                var start = triangle * 3;
                                var p0 = meshBim.Vertices[indeces[start]];
                                var p1 = meshBim.Vertices[indeces[start + 1]];
                                var p2 = meshBim.Vertices[indeces[start + 2]];

                                var prim = mesh.UsePrimitive(material1);
                                prim.AddTriangle(
                                    new VERTEX((float)p0.X, (float)p0.Y, (float)p0.Z * 10),
                                    new VERTEX((float)p1.X, (float)p1.Y, (float)p1.Z * 10),
                                    new VERTEX((float)p2.X, (float)p2.Y, (float)p2.Z * 10));
                            }
                        }
                    }
                }
            }

            var scene = new SharpGLTF.Scenes.SceneBuilder();
            scene.AddMesh(mesh, Matrix4x4.Identity);
            var gltfmodel = scene.ToSchema2();
            gltfmodel.SaveGLB("mesh.glb");
            gltfmodel.SaveGLTF("mesh.gltf");
            stopwatch.Stop();
            Console.WriteLine("Elapsed: " + stopwatch.Elapsed);
        }

        private static void PrintSemantics(IfcStore model)
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

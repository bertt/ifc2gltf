using SharpGLTF.Geometry;
using SharpGLTF.Materials;
using SharpGLTF.Scenes;
using System;
using System.IO;
using System.Linq;
using System.Numerics;
using Xbim.Common.Geometry;
using Xbim.Common.XbimExtensions;
using Xbim.Ifc;
using Xbim.ModelGeometry.Scene;
using VERTEX = SharpGLTF.Geometry.VertexTypes.VertexPosition;

namespace ifc2gltf
{
    public static class GltfConverter
    {
        public static SceneBuilder ToGltf(IfcStore model)
        {
            var context = new Xbim3DModelContext(model);
            context.CreateContext();
            var geoms = context.ShapeGeometries().ToList();
            Console.WriteLine("Number of geometries: " + geoms.Count);

            var mesh = new MeshBuilder<VERTEX>("mesh");
            var material1 = new MaterialBuilder()
               .WithDoubleSide(true)
               .WithMetallicRoughnessShader()
               .WithChannelParam("BaseColor", new Vector4(1, 0, 0, 1));


            foreach (var geom in geoms)
            {
                var data = ((IXbimShapeGeometryData)geom).ShapeData;

                using var stream = new MemoryStream(data);
                using var reader = new BinaryReader(stream);
                var meshBim = reader.ReadShapeTriangulation();

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


            var scene = new SceneBuilder();
            scene.AddRigidMesh(mesh, Matrix4x4.Identity);
            return scene;
        }
    }
}

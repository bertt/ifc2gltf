using SharpGLTF.Geometry;
using SharpGLTF.Materials;
using SharpGLTF.Scenes;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using Xbim.Common.Geometry;
using Xbim.Common.XbimExtensions;
using Xbim.Ifc;
using Xbim.Ifc4.Interfaces;
using Xbim.ModelGeometry.Scene;
using VERTEX = SharpGLTF.Geometry.VertexTypes.VertexPosition;

namespace ifc2gltf
{
    public static class GltfConverter
    {
        private static XbimShapeTriangulation GetMeshes (Xbim3DModelContext context, IIfcProduct product)
        {
            XbimShapeTriangulation ifcMesh = null;;
            var productShape =
                context.ShapeInstancesOf(product)
                    .Where(p => p.RepresentationType != XbimGeometryRepresentationType.OpeningsAndAdditionsExcluded)
                .Distinct();

            if (productShape.Any())
            {
                var shapeInstance = productShape.FirstOrDefault();
                var shapeGeometry = context.ShapeGeometry(shapeInstance.ShapeGeometryLabel);

                byte[] data = ((IXbimShapeGeometryData)shapeGeometry).ShapeData;

                //If you want to get all the faces and triangulation use this
                using (var stream = new MemoryStream(data))
                {
                    using (var reader = new BinaryReader(stream))
                    {
                        ifcMesh = reader.ReadShapeTriangulation();
                    }
                }
            }

            return ifcMesh;
        }

        public static SceneBuilder ToGltf(IfcStore model)
        {
            var context = new Xbim3DModelContext(model);
            context.CreateContext();

            var ifcProject = model.Instances.OfType<IIfcProject>().FirstOrDefault();
            var ifcSite = model.Instances.OfType<IIfcSite>().FirstOrDefault();
            var ifcBuilding = ifcSite.Buildings.FirstOrDefault();
            var ifcStoreys = ifcBuilding.BuildingStoreys; // 3 in case of office

            var spaceMeshes = new List<XbimShapeTriangulation>();
            foreach(var storey in ifcStoreys)
            {
                var spaces = storey.Spaces;
                foreach(var space in spaces)
                {
                    var ifcmesh= GetMeshes(context, space);
                    spaceMeshes.Add(ifcmesh);
                }
            }

            var mesh = new MeshBuilder<VERTEX>("mesh");
            var material1 = new MaterialBuilder()
               .WithDoubleSide(true)
               .WithMetallicRoughnessShader()
               .WithChannelParam("BaseColor", new Vector4(1, 0, 0, 1));


            foreach (var meshBim in spaceMeshes)
            {

                var faces = (List<XbimFaceTriangulation>)meshBim.Faces;
                var vertices = (List<XbimPoint3D>)meshBim.Vertices;

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
                            new VERTEX((float)p0.X, (float)p0.Z, (float)p0.Y),
                            new VERTEX((float)p1.X, (float)p1.Z, (float)p1.Y),
                            new VERTEX((float)p2.X, (float)p2.Z, (float)p2.Y));
                    }
                }
            }

            var scene = new SceneBuilder();
            scene.AddRigidMesh(mesh, Matrix4x4.Identity);
            return scene;
        }
    }
}

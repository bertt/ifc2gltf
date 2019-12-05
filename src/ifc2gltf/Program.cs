using Xbim.Ifc;
using System;
using System.Diagnostics;
using CommandLine;
using System.IO;

namespace ifc2gltf
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("tool ifc2gltf");
            Parser.Default.ParseArguments<Options>(args).WithParsed(o =>
            {
                Console.WriteLine("Input file: " + o.Input);

                if (string.IsNullOrEmpty(o.Output))
                {
                    o.Output = Path.GetFileNameWithoutExtension(o.Input) + ".glb";
                }

                Console.WriteLine("Output file: " + o.Output);
                var model = IfcStore.Open(o.Input);
                // IfcInfo.PrintSemantics(model);
                var stopwatch = new Stopwatch();

                stopwatch.Start();
                Console.WriteLine("Start converting to glTF...");
                var scene = GltfConverter.ToGltf(model);
                var gltfmodel = scene.ToSchema2();
                gltfmodel.SaveGLB(o.Output);
                stopwatch.Stop();
                Console.WriteLine("Converting to glTF finished.");
                Console.WriteLine("Elapsed: " + stopwatch.Elapsed);
            });
        }
    }
}
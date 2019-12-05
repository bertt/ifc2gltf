using CommandLine;

namespace ifc2gltf
{
    public class Options
    {
        [Option('i', "input", Required = true, HelpText = "Input file")]
        public string Input { get; set; }

        [Option('o', "output", Required = false, HelpText = "Output file")]
        public string Output { get; set; }
    }
}

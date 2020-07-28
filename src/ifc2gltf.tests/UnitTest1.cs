using NUnit.Framework;

namespace ifc2gltf.tests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            var expectedGlb = "./Office.glb";
            var model = SharpGLTF.Schema2.ModelRoot.Load(expectedGlb);

            // todo: actual glb

            Assert.Pass();
        }
    }
}
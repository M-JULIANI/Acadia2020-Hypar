using Xunit;
using System.Collections.Generic;
using Elements.Geometry;
using Elements.Serialization.glTF;

namespace ProceduralMassGen
{
    
    public class TestExecute
    {
        [Fact]
        public void RunTest()
        {
            var polyline = new Polygon(new List<Vector3>{new Vector3(-2, -3), new Vector3(5,-4), new Vector3(3,4), new Vector3(-5,6)});
            var input = new ProceduralMassGenInputs(polyline, 1.0, 1.0, 1.0, "", "", null, "", "", "");

            var output = ProceduralMassGen.Execute(null, input);
            output.Model.ToGlTF("../../../myOutput.gltf", false);
            output.Model.ToGlTF("../../../myOutput.glb", true);

        }
    }
}
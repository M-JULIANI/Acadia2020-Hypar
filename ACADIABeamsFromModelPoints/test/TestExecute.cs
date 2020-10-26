 
using Xunit;
using System.Collections.Generic;
using Elements.Serialization.glTF;
using System.IO;
using Newtonsoft.Json;
using Elements;

namespace ACADIABeamsFromModelPoints
{
public class TestExecute
    {
        [Fact]
        public void RunTest()
        {
            var modPtJson = File.ReadAllText("../../../modelPoints.json");
            var modPoints = JsonConvert.DeserializeObject<Model>(modPtJson);
           
            var input = new ACADIABeamsFromModelPointsInputs(20.0,"", "", null, "", "", "");
            var output = ACADIABeamsFromModelPoints.Execute(new Dictionary<string, Model>{{"BlobData", modPoints}}, input);
            output.Model.ToGlTF("../../../myOutput.gltf", false);
            output.Model.ToGlTF("../../../myOutput.glb", true);

        }
    }





}
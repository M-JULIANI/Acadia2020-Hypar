using Elements;
using Elements.Geometry;
using Elements.Geometry.Profiles;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ACADIABeamsFromModelPoints
{
    public static class ACADIABeamsFromModelPoints
    {
        /// <summary>
        /// The ACADIABeamsFromModelPoints function.
        /// </summary>
        /// <param name="model">The input model.</param>
        /// <param name="input">The arguments to the execution.</param>
        /// <returns>A ACADIABeamsFromModelPointsOutputs instance containing computed results and the model with any new elements.</returns>
        public static ACADIABeamsFromModelPointsOutputs Execute(Dictionary<string, Model> inputModels, ACADIABeamsFromModelPointsInputs input)
        {

            if (!inputModels.TryGetValue("BlobData", out var internalPtsModel)) { throw new Exception("womp womp model points"); }
            var pts = internalPtsModel.AllElementsOfType<ModelPoints>().Where(n => n.Name == "BlobCentroids");
            var modelPts = pts.ToArray()[0];

            var locations = modelPts.Locations;
            var output = new ACADIABeamsFromModelPointsOutputs(input.Threshold);
            foreach(var l in locations)
            {
              var indices = IndecesToConnectWith(l, input.Threshold, modelPts);

              for(int i=0; i<indices.Count; i++)
              {
                var line = new Line(l, locations[indices[i]]);
                var beam = new Beam(line, 
                 WideFlangeProfileServer.Instance.GetProfileByType(WideFlangeProfileType.W12x279),
                 BuiltInMaterials.Steel,
                 0,
                 0,
                 0,
                 null,
                 false);
                 output.Model.AddElement(beam);
              }
            }
            return output;
        }

        public static List<int> IndecesToConnectWith(Vector3 origin, double threshold, ModelPoints points)
        {
          List<int> connectionIndeces = new List<int>();
            var locations = points.Locations;
            int count = 0;
            foreach (var p in locations)
            {
                var dist = p.DistanceTo(origin);
                if (dist < threshold && dist>1.0)
                    connectionIndeces.Add(count);
              
              count++;
            }

            return connectionIndeces;
        }
    }


}
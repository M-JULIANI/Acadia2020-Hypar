using Elements;
using Elements.Geometry;
using System.Collections.Generic;
using System.Linq;
using System;
using Elements.Analysis;

namespace ACADIAHeatMap
{
      public static class ACADIAHeatMap
    {
        /// <summary>
        /// The ACADIAHeatMap function.
        /// </summary>
        /// <param name="model">The input model.</param>
        /// <param name="input">The arguments to the execution.</param>
        /// <returns>A ACADIAHeatMapOutputs instance containing computed results and the model with any new elements.</returns>
        public static ACADIAHeatMapOutputs Execute(Dictionary<string, Model> inputModels, ACADIAHeatMapInputs input)
        {
            if(!inputModels.TryGetValue("Envelope", out var envelopeModel)) { throw new Exception("womp womp envelope"); }
            var env = envelopeModel.AllElementsOfType<Envelope>().ToArray()[0];
            var profile = env.Profile.Perimeter;

            if(!inputModels.TryGetValue("BlobData", out var internalPtsModel)) { throw new Exception("womp womp model points"); }
            var pts = internalPtsModel.AllElementsOfType<ModelPoints>().Where(n=>n.Name == "BlobCentroids");
            var modelPts =  pts.ToArray()[0];
            
            var distances = "fake distances";
             var output = new ACADIAHeatMapOutputs(distances);

 // The analyze function computes the distance
            // to the attractor.
            var analyze = new Func<Vector3, double>((v) =>
            {
              var dist = ClosestPointDist(v, modelPts);
             distances += dist.ToString();
             distances+=",";

              return dist;
            });


            // Construct a color scale from a small number
            // of colors.
            var colorScale = new ColorScale(new List<Color>() { Colors.Cyan, Colors.Purple, Colors.Orange }, 10);
            var analysisMesh = new AnalysisMesh(profile, input.CellSize, input.CellSize, colorScale, analyze);
            
            var zDelta = analysisMesh.Transform.ZAxis.Z - profile.Centroid().Z;
            analysisMesh.Analyze();

            analysisMesh.Transform.Move(zDelta);
            

            output.Model.AddElement(analysisMesh);


            return output;
        }

        public static double ClosestPointDist(Vector3 origin, ModelPoints points)
        {
          double min = double.MaxValue;
          var locations = points.Locations;
          foreach(var p in locations){
            var dist = p.DistanceTo(origin);
            if(dist<min)
              min = dist;
        }

          return min;
        }

        public static Vector3 MeshCentroid(MeshElement mesh)
        {
          var verts = mesh.Mesh.Vertices;
          var centroid = AverageVert(verts);
          return centroid;
        }

        public static Vector3 AverageVert(List<Vertex> vertices)
        {
          Vector3 average = new Vector3(0,0,0);
          for(int i=0; i<vertices.Count; i++)
          {
            average += new Vector3(vertices[i].Position.X, vertices[i].Position.Y, vertices[i].Position.Z);
          }
          average /= vertices.Count;

          return average;
        }
      }
}
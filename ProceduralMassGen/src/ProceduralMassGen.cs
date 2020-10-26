using Elements;
using Elements.Spatial;
using Elements.Geometry;
using System.Collections.Generic;
using GeometryEx;
using System.Linq;
using Elements.Analysis;
using System;


namespace ProceduralMassGen
{
    public static class ProceduralMassGen
    {

       

        /// <summary>
        /// The ProceduralMassGen function.
        /// </summary>
        /// <param name="model">The input model.</param>
        /// <param name="input">The arguments to the execution.</param>
        /// <returns>A ProceduralMassGenOutputs instance containing computed results and the model with any new elements.</returns>
        public static ProceduralMassGenOutputs Execute(Dictionary<string, Model> inputModels, ProceduralMassGenInputs input)
        {
            var boundaries = input.Perimeter.Offset(5.0, EndType.Butt);
            var boundary = boundaries[0];
             var volume = input.MinBuildHeight * input.MaxBuildHeight;
             var output = new ProceduralMassGenOutputs(volume);

            var mc = new ModelCurve(boundary);
            output.Model.AddElement(mc);


/////////////// analysis mesh code
            // Construct a mass from which we will measure
            // distance to the analysis mesh's cells.
            var center = boundary.Centroid();
            var mass = new Mass(Polygon.Rectangle(1, 1));
            mass.Transform.Move(center);
            output.Model.AddElement(mass);

            // The analyze function computes the distance
            // to the attractor.
            var analyze = new Func<Vector3, double>((v) =>
            {
                return center.DistanceTo(v);
            });


            // Construct a color scale from a small number
            // of colors.
            var colorScale = new ColorScale(new List<Color>() { Colors.Cyan, Colors.Purple, Colors.Orange }, 10);

            var analysisMesh = new AnalysisMesh(boundary, 2.0, 2.0, colorScale, analyze);
            analysisMesh.Analyze();

            output.Model.AddElement(analysisMesh);
/////////////// analysis mesh code

            // var height = 1.0;dotnet bui
            // var mass = new Mass(boundary, height);


            // //var profile = new Profile(boundaries);

            // //var cG = new CoordinateGrid(input.Perimeter, 10.0, 10.0, 10.0);


            // //var modelCurves = grid.GetCells().Select(c => new ModelCurve(c.GetCellGeometry()));


            // //var pts = cG.Available;

            // /// Your code here.
            // //var height = 1.0;

            // var volume = input.MinBuildHeight * input.MaxBuildHeight;
            // var output = new ProceduralMassGenOutputs(volume);
            // output.Model.AddElement(mass);


            // var grid = new Grid2d(boundary, null);
            // grid.U.DivideByApproximateLength(1.0);
            // grid.V.DivideByApproximateLength(1.0);
            // //var cells = grid.GetCells();

            // var newCells = grid.GetCells().Select(c=>c).ToArray();

            // var cellsBelonging = new List<Grid2d>();
            // foreach(var c in newCells)
            // {
            //     var vertices = from n in c.GetCellGeometry().ToPolyline().Vertices group n by n into nGroup where nGroup.Count() ==1 select nGroup.Key;
            //     var vertArray = vertices.ToArray();

            //     var poly = new Polygon(vertArray);
            //     if(boundary.Contains(poly.Centroid()))
            //     {
            //         cellsBelonging.Add(c);
            //     }
            // }

            // var modelCurves = cellsBelonging.Select(c => new ModelCurve(c.GetCellGeometry()));

            // foreach (var m in modelCurves)
            //      output.Model.AddElement(m);

            //for (int i = 0; i < pts.Count; i++)
            //{
            //    var rectangle = Polygon.Rectangle(8, 8);
            //    var mass = new Mass(rectangle, height);
            //    output.Model.AddElement(mass);
            //}

            return output;
        }
    }
}
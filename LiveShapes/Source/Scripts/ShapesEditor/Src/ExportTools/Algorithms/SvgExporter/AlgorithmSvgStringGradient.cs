using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LiveShapes.Source.Scripts.ShapesEditor.Src.ExportTools.Algorithms.SvgExporter
{
    public static class AlgorithmSvgStringGradient
    {
        public const string gradientId = "GradientOverlay";
        public static void AddGradient(SvgData svgData, ref string svg)
        {
            if (!svgData.HasGradient || svgData.FillGradient == null)
                return;

            switch (svgData.FillGradientId)
            {
                case 0:
                    AddLinearGradient(svgData, ref svg);
                    break;
                case 1:
                    AddRadialGradient(svgData, ref svg);
                    break;
            }
        }

        private static void AddLinearGradient(SvgData svgData, ref string svg)
        {
            var v1 = svgData.FillGradientPivot;
            var middle = (svgData.FillGradientAnchor - svgData.FillGradientPivot) / 2;
            var v2 = v1 + middle + new Vector2(-middle.y, middle.x);

            svg += $"<linearGradient id=\"{gradientId}\" x1=\"{v1.x}\" y1=\"{v1.y}\" x2=\"{v2.x}\" y2=\"{v2.y}\"" +
                   $" gradientUnits=\"userSpaceOnUse\">";
            
            svg += "\n";
            AddGradientStops(svgData, ref svg);
            svg += $"</linearGradient>";
            svg += "\n";
        }
        
        private static void AddRadialGradient(SvgData svgData, ref string svg)
        {
            var v1 = svgData.FillGradientPivot;
            var middle = (svgData.FillGradientAnchor - svgData.FillGradientPivot) / 2;
            var center = v1 + middle;
            var v2 = v1 + middle + new Vector2(-middle.y, middle.x);
            
            var v3 = (v2 - v1) / 2;
            var rad = Vector2.Distance(middle, v3);
            
            svg += $"<radialGradient id=\"{gradientId}\" cx=\"{center.x}\" cy=\"{center.y}\" r=\"{rad}\"" +
                   $" gradientUnits=\"userSpaceOnUse\">";
            svg += "\n";
            AddGradientStops(svgData, ref svg);
            svg += $"</radialGradient>";
            svg += "\n";
        }

        private static void AddGradientStops(SvgData svgData, ref string svg)
        {
            var gradient = svgData.FillGradient;
            var stops = CreateStopsPlaces(gradient);
            for (var i = 0; i < stops.Count; i++)
            {
                var color = gradient.Evaluate(stops[i]);
                svg += $"<stop offset=\"{stops[i] * 100}%\"" +
                       $" stop-color=\"{AlgorithmsOther.ConvertColorToHex(color)}\"" +
                       $" stop-opacity=\"{color.a}\"/>";
                svg += "\n";
            }
        }

        private static List<float> CreateStopsPlaces(Gradient gradient)
        {
            var stops = new List<float>();

            for (var i = 0; i < gradient.colorKeys.Length; i++)
                stops.Add(gradient.colorKeys[i].time);

            for (var i = 0; i < gradient.alphaKeys.Length; i++)
                stops.Add(gradient.alphaKeys[i].time);

            for (var i = 0; i < stops.Count; i++)
            for (var d = i + 1; d < stops.Count; d++)
                if (Mathf.Abs(stops[i] - stops[d]) < 0.001f)
                    stops.RemoveAt(d);
            
            return stops.OrderBy(e => e).ToList();
        }
    }
}

namespace LiveShapes.Source.Scripts.ShapesEditor.Src.ExportTools.Algorithms.SvgExporter
{
    public static class AlgorithmSvgStringPath
    {
        public static void AddPath(SvgData svgData, ref string svg)
        {
            svg += $"<path d=\"{GenerateShapeCode(svgData)}\"" +
                   $" fill=\"{GetShapeFillCode(svgData)}\"";
            
            svg += GetShapeFillOpacityCode(svgData);

            if (svgData.HasStroke)
                svg += $" stroke=\"{AlgorithmsOther.ConvertColorToHex(svgData.StrokeColor)}\"" +
                       $" stroke-width=\"{svgData.StrokeWidth}\"" +
                       $" stroke-opacity=\"{svgData.StrokeColor.a}\"";
            
            svg += "/>";
            svg += "\n";
        }

        private static string GetShapeFillCode(SvgData svgData)
        {
            if (svgData.HasGradient)
                return $"url(#{AlgorithmSvgStringGradient.gradientId})";
            else
                return AlgorithmsOther.ConvertColorToHex(svgData.FillColor);
        }
        
        private static string GetShapeFillOpacityCode(SvgData svgData)
        {
            if (svgData.HasGradient)
                return string.Empty;
            else
                return $" fill-opacity=\"{svgData.FillColor.a}\"";
        }

        public static string GenerateShapeCode(SvgData svgData)
        {
            var path = string.Empty;
            
            if (svgData.Points.Length < 3)
                return path;
            
            path += $"M{svgData.Points[0].x} {svgData.Points[0].y} ";
            for (var i = 0; i < svgData.Points.Length - 3; i += 3)
            {
                path += $"C {svgData.Points[i + 1].x} {svgData.Points[i + 1].y}";
                path += $" {svgData.Points[i + 2].x} {svgData.Points[i + 2].y}";
                path += $" {svgData.Points[i + 3].x} {svgData.Points[i + 3].y}";
            }
            
            path += " Z";
            
            return path;
        }
    }
}

namespace LiveShapes.Source.Scripts.ShapesEditor.Src.ExportTools.Algorithms.SvgExporter
{
    public static class AlgorithmSvgStringStroke
    {

        public static void AddStroke(SvgData svgData, ref string svg)
        {
            if (!svgData.HasStroke)
                return;
            
            svg += $"<path d=\"{AlgorithmSvgStringPath.GenerateShapeCode(svgData)}\" fill=\"transparent\"";
            
            svg += $" stroke=\"{AlgorithmsOther.ConvertColorToHex(svgData.StrokeColor)}\"" +
                       $" stroke-width=\"{svgData.StrokeWidth}\"" +
                       $" stroke-opacity=\"{svgData.StrokeColor.a}\"" +
                       $" clip-path=\"url(#StrokeMask)\"";
            
            svg += "/>";
            svg += "\n";
        }
    }
}

namespace LiveShapes.Source.Scripts.ShapesEditor.Src.ExportTools.Algorithms.SvgExporter
{
    public static class AlgorithmSvgStringDeclaring
    {
        public static void AddDeclaring(SvgData svgData, ref string svg)
        {
            svg += $"<svg width=\"{svgData.ShapeSize.x}\" height=\"{svgData.ShapeSize.y}\"" +
                   $" xmlns=\"http://www.w3.org/2000/svg\">";
            svg += "\n";
        }
        
        public static void AddDefsDeclaring(ref string svg)
        {
            svg += "<defs>";
            svg += "\n";
        }
        
        public static void FinishDefsDeclaring(ref string svg)
        {
            svg += "</defs>";
            svg += "\n";
        }
        
        public static void FinishDeclaring(ref string svg)
        {
            svg += "</svg>";
        }
    }
}

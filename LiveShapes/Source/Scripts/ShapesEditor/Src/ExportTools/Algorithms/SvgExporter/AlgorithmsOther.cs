using UnityEngine;

namespace LiveShapes.Source.Scripts.ShapesEditor.Src.ExportTools.Algorithms.SvgExporter
{
    public static class AlgorithmsOther
    {
        public static string ConvertColorToHex(Color32 color)
        {
            return "#" + ConvertToHexPart(color.r) + ConvertToHexPart(color.g) + ConvertToHexPart(color.b);

            string ConvertToHexPart(int chanel)
            {
                var hex = chanel.ToString("X");
                return hex.Length == 1 ? "0" + hex : hex;
            }
        }
    }
}

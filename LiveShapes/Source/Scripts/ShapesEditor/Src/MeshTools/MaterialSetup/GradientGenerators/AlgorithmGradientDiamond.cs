using LiveShapes.Source.Scripts.ShapesEditor.Src.Other;
using UnityEngine;

namespace LiveShapes.Source.Scripts.ShapesEditor.Src.MeshTools.MaterialSetup.GradientGenerators
{
    public static class AlgorithmGradientDiamond
    {
        public static Texture2D Generate(Gradient gradient, int textureSize)
        {
            var texture = new Texture2D(textureSize, textureSize)
            {
                wrapMode = TextureWrapMode.Clamp,
            };

            var half = textureSize / 2;
            var center = new Vector2(half, half);
            var sizeX2 = textureSize * textureSize;
            for (var x = textureSize / 2; x < textureSize; x++)
            {
                for (var y = textureSize / 2; y < textureSize; y++)
                {
                    var pos = new Vector2(x, y);
                    pos -= center;
                    pos = new Vector2(Mathf.Abs(pos.x), Mathf.Abs(pos.y));
                    var pos3 = Vector2.one * (pos.x + pos.y);
                    var distance = LiveMath.VectorDistancePowTwo(pos3, Vector2.zero);
                    var color = gradient.Evaluate(distance / sizeX2);
                    texture.SetPixel(x, y, color);
                }
            }
            
            for (var x = 0; x < textureSize / 2; x++)
            {
                var getColors = texture.GetPixels(textureSize - x - 1, half, 1, half);
                texture.SetPixels(x, half, 1, half, getColors);
            }
            
            for (var y = 0; y < half; y++)
            {
                var getColors = texture.GetPixels(0, textureSize - y - 1, textureSize, 1);
                texture.SetPixels(0, y, textureSize, 1, getColors);
            }

            texture.Apply();
            
            return texture;
        }
    }
}

using LiveShapes.Source.Scripts.ShapesEditor.Src.Other;
using UnityEngine;

namespace LiveShapes.Source.Scripts.ShapesEditor.Src.MeshTools.MaterialSetup.GradientGenerators
{
    public static class AlgorithmGradientAngle
    {
        public static Texture2D Generate(Gradient gradient, int textureSize)
        {
            var texture = new Texture2D(textureSize, textureSize)
            {
                wrapMode = TextureWrapMode.Clamp,
            };

            var half = textureSize / 2;
            var center = new Vector2Int(half, half);
            var upperPosition = new Vector2(half, textureSize);
            for (var x = 0; x < textureSize; x++)
            {
                for (var y = 0; y < textureSize; y++)
                {
                    var angle = LiveMath.AngleOfTriangle(upperPosition, center, new Vector2(x, y));
                    var color = gradient.Evaluate(angle / 360f);
                    texture.SetPixel(x, y, color);
                }
            }
            
            texture.SetPixel(center.x, center.y, gradient.Evaluate(0f));
            
            texture.Apply();
            
            return texture;
        }
    }
}

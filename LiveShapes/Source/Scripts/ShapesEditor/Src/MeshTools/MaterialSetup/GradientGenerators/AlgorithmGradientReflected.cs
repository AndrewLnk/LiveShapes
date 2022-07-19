using UnityEngine;

namespace LiveShapes.Source.Scripts.ShapesEditor.Src.MeshTools.MaterialSetup.GradientGenerators
{
    public static class AlgorithmGradientReflected
    {
        public static Texture2D Generate(Gradient gradient, int textureSize)
        {
            var texture = new Texture2D(textureSize, textureSize)
            {
                wrapMode = TextureWrapMode.Clamp,
            };

            var half = textureSize / 2;
            
            for (var x = 0; x <= half; x++)
            {
                var color = gradient.Evaluate(((float) x / textureSize) * 2);
                var colors = new Color[textureSize];
                for (var y = 0; y < colors.Length; y++) colors[y] = color;
                texture.SetPixels(x, 0, 1, textureSize, colors);
            }
            
            for (var x = half; x < textureSize; x++)
            {
                var getColors = texture.GetPixels(textureSize - x, 0, 1, textureSize);
                texture.SetPixels(x, 0, 1, textureSize, getColors);
            }

            texture.Apply();
            
            return texture;
        }
    }
}

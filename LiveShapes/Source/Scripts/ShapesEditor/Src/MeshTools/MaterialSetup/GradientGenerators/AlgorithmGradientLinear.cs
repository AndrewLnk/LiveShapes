using UnityEngine;

namespace LiveShapes.Source.Scripts.ShapesEditor.Src.MeshTools.MaterialSetup.GradientGenerators
{
    public static class AlgorithmGradientLinear
    {
        public static Texture2D Generate(Gradient gradient, int textureSize)
        {
            var texture = new Texture2D(textureSize, textureSize)
            {
                wrapMode = TextureWrapMode.Clamp,
            };

            for (var x = 0; x < textureSize; x++)
            {
                var color = gradient.Evaluate((float) x / textureSize);
                var colors = new Color[textureSize];
                for (var y = 0; y < colors.Length; y++) colors[y] = color;
                texture.SetPixels(x, 0, 1, textureSize, colors);
            }
            
            texture.Apply();
            
            return texture;
        }
    }
}

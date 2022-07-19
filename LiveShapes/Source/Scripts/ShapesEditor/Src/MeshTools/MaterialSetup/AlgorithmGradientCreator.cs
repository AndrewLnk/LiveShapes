using UnityEngine;

namespace LiveShapes.Source.Scripts.ShapesEditor.Src.MeshTools.MaterialSetup
{
    public static class AlgorithmGradientCreator
    {
        public static Texture2D CreatTexture2D(Gradient gradient, int resolutionId)
        {
            var textureSize = GetTextureSize(resolutionId);
            var texture = new Texture2D(textureSize, 1, TextureFormat.RGBA32, false, false)
            {
                wrapMode = TextureWrapMode.Clamp,
                alphaIsTransparency = true
            };

            for (var x = 0; x < textureSize; x++)
            {
                var color = gradient.Evaluate((float) x / textureSize);
                texture.SetPixel(x, 0, color);
            }

            texture.Apply();

            return texture;
        }

        private static int GetTextureSize(int resolutionId)
        {
            switch (resolutionId)
            {
                case 0 : return 32;
                case 1 : return 64;
                case 2 : return 128;
                case 3 : return 256;
                case 4 : return 512;
                case 5 : return 1024;
                default: return 32;
            }
        }
    }
}

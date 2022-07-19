using System.IO;
using UnityEditor;

namespace LiveShapes.Source.Scripts.ShapesEditor.Src.ExportTools.Algorithms
{
    public static class AlgorithmUpdateBuilderByFile
    {
        public static void Update(ExportToFileBuilder builder, ref string path)
        {
            if (!File.Exists(path))
            {
                path += "/";
                return;
            }

            var filePart = path.Substring(path.LastIndexOf('/'));

            var formatPosition = filePart.LastIndexOf('.') - 1;
            if (formatPosition < 0) formatPosition = filePart.Length - 1;
            var fileName = filePart.Substring(1, formatPosition);
            
            builder.FileName = fileName;

            formatPosition = filePart.LastIndexOf('.');
            if (formatPosition > 0)
            {
                var fileFormat = filePart.Substring(formatPosition).ToLower();
                switch (fileFormat)
                {
                    case ".ls":
                        builder.ConvertFormatId = 0;
                        SetupPngSize(builder, path);
                        break;
                    case ".png":
                        builder.ConvertFormatId = 1;
                        SetupPngSize(builder, path);
                        break;
                    case ".svg":
                        builder.ConvertFormatId = 2;
                        break;
                    case ".mesh":
                        builder.ConvertFormatId = 3;
                        break;
                }
            }

            path = path.Substring(0, path.LastIndexOf('/') + 1);
        }

        private static void SetupPngSize(ExportToFileBuilder builder, string path)
        {
            var textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;

            if (textureImporter == null)
                return;

            switch (textureImporter.maxTextureSize)
            {
                case 32:
                    builder.SpriteSize = 0;
                    break;
                case 64:
                    builder.SpriteSize = 1;
                    break;
                case 128:
                    builder.SpriteSize = 2;
                    break;
                case 256:
                    builder.SpriteSize = 3;
                    break;
                case 512:
                    builder.SpriteSize = 4;
                    break;
                case 1024:
                    builder.SpriteSize = 5;
                    break;
                case 2048:
                    builder.SpriteSize = 6;
                    break;
            }
        }
    }
}

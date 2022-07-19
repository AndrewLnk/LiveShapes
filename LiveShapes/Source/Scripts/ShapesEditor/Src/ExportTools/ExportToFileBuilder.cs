using System;
using System.IO;
using System.Text;
using LiveShapes.Source.Scripts.ShapesEditor.Src.ExportTools.Algorithms;
using LiveShapes.Source.Scripts.ShapesEditor.Src.ExportTools.Algorithms.SvgExporter;
using LiveShapes.Source.Scripts.ShapesEditor.Src.MainComponents;
using LiveShapes.Source.Scripts.ShapesEditor.Src.MeshTools.MaterialSetup;
using LiveShapes.Source.Scripts.ShapesEditor.Src.MeshTools.MeshSetup;
using LiveShapes.Source.Scripts.ShapesEditor.Src.Other;
using LiveShapes.Source.Scripts.ShapesEditor.Src.Other.LiveShapesData;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace LiveShapes.Source.Scripts.ShapesEditor.Src.ExportTools
{
    public class ExportToFileBuilder
    {
        public string ExportAssetPath { get; private set; }
        public string FileName = "New Shape";
        
        public int ConvertFormatId;
        public readonly string[] FormatNames = {"To LiveShape", "To Png", "To Svg (by curve)", "To Mesh", };

        public int SpriteSize;
        public readonly string[] SpriteSizeNames = { "32 px", "64 px", "128 px", "256 px", "512 px", "1024 px", "2048 px" };
        private readonly int[] spriteSizes = { 32, 64, 128, 256, 512, 1024, 2048 };
        public float StrokeWidth
        {
            get => strokeWidth;
            set => strokeWidth = Mathf.Clamp(value, 0, 100);
        }
        
        private float strokeWidth;
        public Color32 StrokeColor = Color.white;

        private readonly CurveObject curveObject;
        private readonly LiveMeshBuilder liveMeshBuilder;
        public readonly LiveMaterialBuilder MaterialBuilder;

        public ExportToFileBuilder(CurveObject curveObject, LiveMeshBuilder liveMeshBuilder, LiveMaterialBuilder materialBuilder)
        {
            this.curveObject = curveObject;
            this.liveMeshBuilder = liveMeshBuilder;
            MaterialBuilder = materialBuilder;

            ExportAssetPath = SavedParams.ExportPath;
            if (string.IsNullOrEmpty(ExportAssetPath))
                TryUpdatePath();
        }

        public void TryUpdatePath()
        {
            var path = GetSelectedPathOrFallback();
            
            if (string.IsNullOrEmpty(path))
                return;

            AlgorithmUpdateBuilderByFile.Update(this, ref path);
            ExportAssetPath = path;
        }
        
        public void TryUpdatePath(string path)
        {
            if (string.IsNullOrEmpty(path))
                return;

            AlgorithmUpdateBuilderByFile.Update(this, ref path);
            ExportAssetPath = path;
        }

        public void StartExport()
        {
            switch (ConvertFormatId)
            {
                case 0:
                    if (curveObject.MeshFilter.sharedMesh == null)
                        break;
                    
                    var assetPath = $"{ExportAssetPath}{FileName}.ls";
                    CreateAsset(assetPath);
                    break;
                case 1:
                    var texture = AlgorithmCreatePng.CreateFromMesh(curveObject.MeshFilter, spriteSizes[SpriteSize],
                                                                    strokeWidth, StrokeColor);
                    var texturePath = $"{ExportAssetPath}{FileName}.png";
                    CreatePng(texturePath, texture);
                    break;
                case 2:
                    if (curveObject.Data.Positions.Length == 0)
                        break;
                    
                    var svgData = CreateSvgData();
                    var svgPath = $"{ExportAssetPath}{FileName}.svg";
                    CreateSvg(svgPath, svgData.GenerateCode());
                    break;
                case 3:
                    if (curveObject.MeshFilter.sharedMesh == null)
                        break;
                    
                    var newMesh = Object.Instantiate(curveObject.MeshFilter.sharedMesh);
                    var meshPath = $"{ExportAssetPath}{FileName}.mesh";
                    MeshUtility.Optimize(newMesh);
                    CreateMesh(meshPath, newMesh);
                    break;
            }
            
            AssetDatabase.SaveAssets();
        }

        private static void CreateMesh(string path, Mesh mesh)
        {
            if (string.IsNullOrEmpty(path))
                return;
            
            AssetDatabase.DeleteAsset(path);
            AssetDatabase.CreateAsset(mesh, path);
            AssetDatabase.ImportAsset(path);
        }
        
        private static void CreateSvg(string path, string svgCode)
        {
            if (string.IsNullOrEmpty(path))
                return;
            
            AssetDatabase.DeleteAsset(path);
            
            try
            {
                File.WriteAllBytes(path, Encoding.ASCII.GetBytes(svgCode));
            }
            catch (Exception exception)
            {
                LiveLog.LogWriteFileWarning(exception.Message);
                return;
            }
            
            AssetDatabase.ImportAsset(path);
        }
        
        private static void CreatePng(string path, Texture2D texture2D)
        {
            if (string.IsNullOrEmpty(path))
                return;
            
            AssetDatabase.DeleteAsset(path);
            
            try
            {
                File.WriteAllBytes(path, texture2D.EncodeToPNG());
            }
            catch (Exception exception)
            {
                LiveLog.LogWriteFileWarning(exception.Message);
                return;
            }
            
            AssetDatabase.ImportAsset(path);
            
            var textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;
            if (textureImporter != null)
            {
                textureImporter.textureType = TextureImporterType.Sprite;
                textureImporter.SetPlatformTextureSettings(new TextureImporterPlatformSettings()
                {
                    format = TextureImporterFormat.RGBA32
                });
            }
            AssetDatabase.ImportAsset(path);
        }
        
        private void CreateAsset(string path)
        {
            if (string.IsNullOrEmpty(path))
                return;

            var code = LiveShapeSerialization.Serialize(curveObject, liveMeshBuilder, MaterialBuilder, FileName);
            
            AssetDatabase.DeleteAsset(path);
            
            try
            {
                File.WriteAllBytes(path, Encoding.ASCII.GetBytes(code));
            }
            catch (Exception exception)
            {
                LiveLog.LogWriteFileWarning(exception.Message);
                return;
            }
            
            AssetDatabase.ImportAsset(path);
        }
        
        private static string GetSelectedPathOrFallback()
        {
            var path = "Assets";

            foreach (var obj in Selection.GetFiltered(typeof(Object), SelectionMode.Assets))
            {
                var newPath = AssetDatabase.GetAssetPath(obj);

                if (string.IsNullOrEmpty(newPath))
                    continue;

                path = newPath;
                break;
            }
            
            return path;
        }

        private SvgData CreateSvgData()
        {
            var svgData = new SvgData(curveObject.Transform.GetLocalPositionsList().ToArray());

            if (MaterialBuilder.SetupPaintingId == 0 || 
                MaterialBuilder.SetupPaintingId == 2)
            {
                svgData.SetupColor(MaterialBuilder.Color);
            }
            else if (MaterialBuilder.SetupPaintingId == 3)
            {
                svgData.SetupGradient(MaterialBuilder.Gradient, MaterialBuilder.GradientType);
                svgData.SetupUvForGradient(curveObject.UvData.LocalPosition, curveObject.UvData.LocalAnchor);
                svgData.SetupColor(Color.white);
            }
            else
            {
                svgData.SetupColor(Color.white);
            }

            if (strokeWidth > 0.00001f) 
                svgData.SetupStroke(strokeWidth, StrokeColor);

            svgData.Prepare();
            
            return svgData;
        }
    }
}

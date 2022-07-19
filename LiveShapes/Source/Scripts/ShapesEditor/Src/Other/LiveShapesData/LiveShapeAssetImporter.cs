using System.IO;
using System.Linq;
using LiveShapes.Source.Scripts.ShapesEditor.Src.MeshTools.MaterialSetup;
using LiveShapes.Source.Scripts.ShapesEditor.Src.MeshTools.MeshSetup;
using LiveShapes.Source.Scripts.ShapesEditor.Src.MeshTools.UvSetup;
using UnityEditor;

using UnityEngine;

namespace LiveShapes.Source.Scripts.ShapesEditor.Src.Other.LiveShapesData
{
    [UnityEditor.AssetImporters.ScriptedImporter(1, "ls")]
    public class LiveShapeAssetImporter : UnityEditor.AssetImporters.ScriptedImporter
    {
        public override void OnImportAsset(UnityEditor.AssetImporters.AssetImportContext ctx)
        {
            var go = new GameObject();
            ctx.AddObjectToAsset("main object", go);
            ctx.SetMainObject(go);

            var reader = new StreamReader(ctx.assetPath);
            var data = LiveShapeSerialization.Load(reader.ReadToEnd());
            
            if (data == null)
                return;
            
            var meshFilter = go.AddComponent<MeshFilter>();
            var meshBuilder = new LiveMeshBuilder(meshFilter);
            meshBuilder.GenerateMeshSteps = data.GenerateMeshSteps;
            meshBuilder.MeshOptimization = data.MeshOptimization;
            meshBuilder.Update(data.CurveLocalPositions.ToList());
            
            var uvBuilder = new LiveUvBuilder(meshFilter);
            var uvData = new UvData();
            uvData.LocalPosition = data.UvLocalPosition;
            uvData.LocalAnchor = data.UvLocalAnchor;
            uvBuilder.Update(uvData, true);
            
            var sharedMesh = meshFilter.sharedMesh;
            sharedMesh.name = "Shape";
            ctx.AddObjectToAsset("Mesh", sharedMesh);
            
            var meshRenderer = go.AddComponent<MeshRenderer>();
            var materialBuilder = new LiveMaterialBuilder(meshRenderer);
            materialBuilder.SetupPaintingId = data.SetupPainting;
            materialBuilder.Color = data.PaintingColor;
            materialBuilder.GradientType = data.GradientType;
            materialBuilder.Gradient = data.Gradient;
            materialBuilder.GradientResolution = data.GradientResolution;
            var texture = EditorUtility.InstanceIDToObject(data.Texture2DId) as Texture2D;
            materialBuilder.Texture2D = texture != null ? texture : null;
            materialBuilder.Texture2DPosition = data.Texture2DPosition;
            var material = EditorUtility.InstanceIDToObject(data.MaterialId) as Material;
            materialBuilder.Material = material != null ? material : null;
            materialBuilder.Update();
            
            var sharedMaterial = meshRenderer.sharedMaterial;
            if (sharedMaterial != null)
            {
                sharedMaterial.name = "Live Material";
                
                if (sharedMaterial.HasProperty("_MainTex") && data.SetupPainting == 3)
                {
                    var sharedTexture = sharedMaterial.mainTexture;

                    if (sharedTexture != null)
                    {
                        sharedTexture.name = "Texture";
                        ctx.AddObjectToAsset("Texture", sharedTexture);
                    }
                }
            }

            if (data.SetupPainting != 4)
            {
                ctx.AddObjectToAsset("Material", sharedMaterial);
            }

            var publicData = ScriptableObject.CreateInstance<LiveShapeDataObject>();
            publicData.name = $"{data.Name.ToLower().Replace(" ", "_")}.data";
            publicData.Setup(data);
            ctx.AddObjectToAsset("Data", publicData);
            LiveLog.ResetWarnings();
        }
    }
}

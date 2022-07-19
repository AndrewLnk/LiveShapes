using System;
using LiveShapes.Source.Scripts.ShapesEditor.Src.MainComponents;
using LiveShapes.Source.Scripts.ShapesEditor.Src.MeshTools.MaterialSetup;
using LiveShapes.Source.Scripts.ShapesEditor.Src.MeshTools.MeshSetup;
using UnityEngine;

namespace LiveShapes.Source.Scripts.ShapesEditor.Src.Other.LiveShapesData
{
    public static class LiveShapeSerialization
    {
        public static string Serialize(
            CurveObject curveObject, 
            LiveMeshBuilder meshBuilder, 
            LiveMaterialBuilder materialBuilder,
            string assetName)
        {
            var data = GetPreparedData(curveObject, meshBuilder, materialBuilder, assetName);
            return JsonUtility.ToJson(data);
        }

        public static LiveShapeData GetPreparedData(
            CurveObject curveObject, 
            LiveMeshBuilder meshBuilder, 
            LiveMaterialBuilder materialBuilder,
            string assetName)
        {
            var data = new LiveShapeData();
            data.Name = assetName;
            
            data.CurveLocalPositions = curveObject.Transform.GetLocalPositionsList().ToArray();
            data.GenerateMeshSteps = meshBuilder.GenerateMeshSteps;
            data.MeshOptimization = meshBuilder.MeshOptimization;
            data.UvLocalPosition = curveObject.UvData.LocalPosition;
            data.UvLocalAnchor = curveObject.UvData.LocalAnchor;

            data.SetupPainting = materialBuilder.SetupPaintingId;
            data.PaintingColor = materialBuilder.Color;
            data.GradientType = materialBuilder.GradientType;
            data.Gradient = materialBuilder.Gradient;
            data.GradientResolution = materialBuilder.GradientResolution;
            data.Texture2DId = materialBuilder.Texture2D != null ? materialBuilder.Texture2D.GetInstanceID() : 0;
            data.Texture2DPosition = materialBuilder.Texture2DPosition;
            data.MaterialId = materialBuilder.Material != null ? materialBuilder.Material.GetInstanceID() : 0;

            return data;
        }

        public static LiveShapeData Load(string data)
        {
            LiveShapeData result = null;
            try
            {
                result = JsonUtility.FromJson<LiveShapeData>(data);
            }
            catch (Exception e)
            {
                LiveLog.LogDeserializationFileWarning(e.Message);
            }
            
            return result;
        }
    }
}

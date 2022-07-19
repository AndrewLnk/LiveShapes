using System;
using LiveShapes.Source.Scripts.ShapesEditor.Src.MainComponents;
using LiveShapes.Source.Scripts.ShapesEditor.Src.MeshTools.MaterialSetup;
using LiveShapes.Source.Scripts.ShapesEditor.Src.MeshTools.MeshSetup;
using LiveShapes.Source.Scripts.ShapesEditor.Src.Other;
using LiveShapes.Source.Scripts.ShapesEditor.Src.Other.LiveShapesData;
using UnityEditor;
using UnityEngine;

namespace LiveShapes.Source.Scripts.ShapesEditor.Src.UndoSystem
{
    public static class AlgorithmRecordState
    {
        public static void Record(
            LiveShapeData data, 
            CurveObject curve, 
            LiveMeshBuilder meshBuilder, 
            LiveMaterialBuilder materialBuilder)
        {
            data.CurveLocalPositions = (Vector2[]) curve.Data.Positions.Clone();
            data.ShapePosition = curve.Transform.Position;
            data.ShapeRotation = curve.Transform.Angle;
            data.GenerateMeshSteps = meshBuilder.GenerateMeshSteps;
            data.MeshOptimization = meshBuilder.MeshOptimization;
            data.UvLocalPosition = curve.UvData.LocalPosition;
            data.UvLocalAnchor = curve.UvData.LocalAnchor;

            data.SetupPainting = materialBuilder.SetupPaintingId;
            data.PaintingColor = materialBuilder.Color;
            data.GradientType = materialBuilder.GradientType;
            data.Gradient = materialBuilder.Gradient;
            data.GradientResolution = materialBuilder.GradientResolution;
            data.Texture2DId = materialBuilder.Texture2D != null ? materialBuilder.Texture2D.GetInstanceID() : 0;
            data.Texture2DPosition = materialBuilder.Texture2DPosition;
            data.MaterialId = materialBuilder.Material != null ? materialBuilder.Material.GetInstanceID() : 0;
        }

        public static UndoItem GetChangeFromState(
            LiveShapeData data, 
            CurveObject curve, 
            LiveMeshBuilder meshBuilder, 
            LiveMaterialBuilder materialBuilder)
        {
            var currentMaterial = materialBuilder.Material != null ? materialBuilder.Material.GetInstanceID() : 0;
            if (data.MaterialId != currentMaterial)
                return new UndoItem() {code = 1, info2 = data.MaterialId};

            if (data.Texture2DPosition != materialBuilder.Texture2DPosition)
                return new UndoItem() {code = 2, info3 = data.Texture2DPosition};

            var currentTexture = materialBuilder.Texture2D != null ? materialBuilder.Texture2D.GetInstanceID() : 0;
            if (data.Texture2DId != currentTexture)
                return new UndoItem() {code = 3, info2 = data.Texture2DId};
            
            if (data.GradientResolution != materialBuilder.GradientResolution)
                return new UndoItem() {code = 4, info2 = data.GradientResolution};
            
            if (!data.Gradient.GradientEquals(materialBuilder.Gradient))
                return new UndoItem() {code = 5, info4 = data.Gradient.Clone()};
            
            if (data.GradientType != materialBuilder.GradientType)
                return new UndoItem() {code = 6, info2 = data.GradientType};
            
            if (data.PaintingColor != materialBuilder.Color)
                return new UndoItem() {code = 7, info5 = data.PaintingColor};
            
            if (data.SetupPainting != materialBuilder.SetupPaintingId)
                return new UndoItem() {code = 8, info2 = data.SetupPainting};
            
            if (data.UvLocalAnchor != curve.UvData.LocalAnchor)
                return new UndoItem() {code = 9, info1 = data.UvLocalAnchor};
            
            if (data.UvLocalPosition != curve.UvData.LocalPosition)
                return new UndoItem() {code = 10, info1 = data.UvLocalPosition};
            
            if (data.MeshOptimization != meshBuilder.MeshOptimization)
                return new UndoItem() {code = 11, info2 = data.MeshOptimization};
            
            if (data.GenerateMeshSteps != meshBuilder.GenerateMeshSteps)
                return new UndoItem() {code = 12, info2 = data.GenerateMeshSteps};
            
            if (data.ShapePosition != curve.Transform.Position)
                return new UndoItem() {code = 13, info1 = data.ShapePosition};
            
            if (Math.Abs(data.ShapeRotation - curve.Transform.Angle) > 0.0001f)
                return new UndoItem() {code = 14, info1 = new Vector2(curve.Transform.Angle, 0) };
            
            if (!LiveMath.ArraysEquals(data.CurveLocalPositions, curve.Data.Positions))
                return new UndoItem() {code = 15, info0 = data.CurveLocalPositions};

            return new UndoItem();
        }

        public static void ApplyState(UndoItem data, 
            CurveObject curve,
            LiveMeshBuilder meshBuilder, 
            LiveMaterialBuilder materialBuilder)
        {
            if (data.code == 1)
                materialBuilder.Material = EditorUtility.InstanceIDToObject(data.info2) as Material;

            if (data.code == 2)
                materialBuilder.Texture2DPosition = data.info3;
            
            if (data.code == 3)
                materialBuilder.Texture2D = EditorUtility.InstanceIDToObject(data.info2) as Texture2D;

            if (data.code == 4)
                materialBuilder.GradientResolution = data.info2;

            if (data.code == 5)
                materialBuilder.Gradient = data.info4;

            if (data.code == 6)
                materialBuilder.GradientType = data.info2;
            
            if (data.code == 7)
                materialBuilder.Color = data.info5;
            
            if (data.code == 8)
                materialBuilder.SetupPaintingId = data.info2;
            
            if (data.code == 9)
                curve.UvData.LocalAnchor = data.info1;
            
            if (data.code == 10)
                curve.UvData.LocalPosition = data.info1;
            
            if (data.code == 11)
                meshBuilder.MeshOptimization = data.info2;
            
            if (data.code == 12)
                meshBuilder.GenerateMeshSteps = data.info2;
            
            if (data.code == 13)
                curve.Transform.Position = data.info1;
            
            if (data.code == 14)
                curve.Transform.Angle = data.info1.x;
            
            if (data.code == 15)
                curve.Data.Positions = data.info0;
        }
    }
}

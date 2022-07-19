using System;
using System.Collections.Generic;
using LiveShapes.Source.Scripts.ShapesEditor.Src.Other;
using UnityEngine;

namespace LiveShapes.Source.Scripts.ShapesEditor.Src.MeshTools.MeshSetup
{
    public class LiveMeshBuilder
    {
        private readonly MeshFilter meshFilter;
        private List<Vector2> savedPos;
        private readonly List<Vector3> meshPos;

        private bool shouldUpdateMeshBySteps;
        private int generateSteps = 1;
        private bool shouldUpdateMeshByOptimization;
        private int meshOptimization;

        private Action meshUpdatedAction;

        public int GenerateMeshSteps
        {
            get => generateSteps;
            set
            {
                shouldUpdateMeshBySteps = generateSteps != value;
                generateSteps = value;
            }
        }
        
        public int MeshOptimization
        {
            get => meshOptimization;
            set
            {
                shouldUpdateMeshByOptimization = meshOptimization != value;
                meshOptimization = value;
            }
        }
        
        public LiveMeshBuilder(MeshFilter meshFilter)
        {
            this.meshFilter = meshFilter;
            savedPos = new List<Vector2>();
            meshPos = new List<Vector3>();
        }

        public void AddMeshUpdatedAction(Action action) => meshUpdatedAction = action;

        public void Update(List<Vector2> newPos, bool rebuildPause = false)
        {
            if (rebuildPause)
                return;
            
            if (!ShouldRebuildMesh(newPos))
                return;
            
            savedPos = new List<Vector2>(newPos);
            meshPos.Clear();
            GeneratePoints(newPos, meshPos);
            AlgorithmMeshRebuild.RebuildMesh(meshFilter, meshPos);
            shouldUpdateMeshBySteps = false;
            shouldUpdateMeshByOptimization = false;
            
            meshUpdatedAction?.Invoke();
        }

        public void GeneratePoints(List<Vector2> newPos, List<Vector3> output)
        {
            var tDelta = 1f / generateSteps;
            for (var i = 0; i < newPos.Count - 3; i += 3) // TODO 0.12 MS
            {
                
                AlgorithmMeshRebuild.RewriteListWithCurvePositions(
                    newPos[i], newPos[i + 1], newPos[i + 2], newPos[i + 3], output, tDelta);
            }
            
            AlgorithmMeshRebuild.CleanDoublePoints(output);
            var crossProductFilter = 0.000001f + meshOptimization / 100000f;
            AlgorithmMeshRebuild.CleanPointsOnLine(output, crossProductFilter);
        }
        
        public void GeneratePoints(List<Vector2> newPos, List<Vector2> output)
        {
            var tDelta = 1f / generateSteps;
            for (var i = 0; i < newPos.Count - 3; i += 3) // TODO 0.12 MS
            {
                
                AlgorithmMeshRebuild.RewriteListWithCurvePositions(
                    newPos[i], newPos[i + 1], newPos[i + 2], newPos[i + 3], output, tDelta);
            }
            
            AlgorithmMeshRebuild.CleanDoublePoints(output);
            var crossProductFilter = 0.000001f + meshOptimization / 100000f;
            AlgorithmMeshRebuild.CleanPointsOnLine(output, crossProductFilter);
        }

        private bool ShouldRebuildMesh(IReadOnlyList<Vector2> curvePositions)
        {
            if (savedPos.Count != curvePositions.Count || shouldUpdateMeshBySteps || shouldUpdateMeshByOptimization)
                return true;

            for (var i = 0; i < curvePositions.Count; i++)
            {
                if (!curvePositions[i].VectorsEquals(savedPos[i]))
                    return true;
            }

            return false;
        }
    }
}

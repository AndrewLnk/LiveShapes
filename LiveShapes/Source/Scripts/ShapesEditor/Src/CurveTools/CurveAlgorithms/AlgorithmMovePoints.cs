using System;
using System.Collections.Generic;
using UnityEngine;

namespace LiveShapes.Source.Scripts.ShapesEditor.Src.CurveTools.CurveAlgorithms
{
    public static class AlgorithmMovePoints
    {
        private const float checkNoHandle = 0.005f;

        private static bool mirroredHandleExists;
        private static float mirroredHandleDistance;
        private static int[] movablePoints;
            
        public static void PrepareToMove(Vector2[] positions, int id)
        {
            if (id < 0 || id >= positions.Length)
                return;
            
            var id2 = FindMirroredHandle(positions.Length, id);
            var id1 = FindMainPoint(id);
            mirroredHandleExists = id2 >= 0 && id2 < positions.Length && 
                                   CheckIfMirrored(positions[id2], positions[id1], positions[id]);

            var listOfPoints = new List<int>();
            
            var points = Collect(positions, id);
            while (points.MoveNext() && points.Current != -1)
                listOfPoints.Add(points.Current);
            
            movablePoints = listOfPoints.ToArray();

            if (!mirroredHandleExists)
                return;
            
            mirroredHandleDistance = Vector2.Distance(positions[id2], positions[id1]);
        }
        
        public static void Move(Vector2[] positions, int id, Vector2 delta)
        {
            if (id < 0 || id >= positions.Length)
                return;

            var id2 = FindMirroredHandle(positions.Length, id);
            var id1 = FindMainPoint(id);

            foreach (var movablePoint in movablePoints)
            {
                positions[movablePoint] -= delta;
                
                if (movablePoint != id2)
                    continue;
                
                mirroredHandleExists = false;
            }

            if (!mirroredHandleExists)
                return;
            
            positions[id2] = CorrectMirrored(
                positions[id], 
                positions[id1], 
                mirroredHandleDistance);
        }

        public static bool MirroredHandle(Vector2[] positions, int id)
        {
            if (id < 0 || id >= positions.Length)
                return false;
            
            var id2 = FindMirroredHandle(positions.Length, id);
            var id1 = FindMainPoint(id);
            var mirroredHandleLocalExists = id2 >= 0 && id2 < positions.Length && 
                CheckIfMirrored(positions[id2], positions[id1], positions[id]);

            return mirroredHandleLocalExists;
        }

        private static int FindMirroredHandle(int positionsCount, int id)
        {
            var result = id;

            if (id % 3 == 0)
                return int.MinValue;

            if ((id - 1) % 3 == 0)
                result -= 2;
            
            if ((id + 1) % 3 == 0)
                result += 2;
            
            if (result < 0)
                result += positionsCount - 1;
            
            if (result >= positionsCount)
                result -= positionsCount - 1;

            return result;
        }
        
        private static int FindMainPoint(int id)
        {
            var result = id;

            if ((id - 1) % 3 == 0)
                result--;
            
            if ((id + 1) % 3 == 0)
                result++;

            return result;
        }
        
        private static IEnumerator<int> Collect(Vector2[] positions, int pointId)
        {
            var collectedPoints = new Stack<int>();
            collectedPoints.Push(pointId);

            CollectWhenMainPoint(positions, pointId, collectedPoints);
            CollectWhenHandlePoint(positions, pointId, collectedPoints);
            
            return collectedPoints.GetEnumerator();
        }

        private static void CollectWhenMainPoint(Vector2[] positions, int pointId, Stack<int> collectedPoints)
        {
            if (pointId % 3 != 0) 
                return;
            
            if (pointId - 1 > 0)
                collectedPoints.Push(pointId - 1);
            if (pointId + 1 < positions.Length)
                collectedPoints.Push(pointId + 1);

            var lastId = positions.Length;
            if (pointId == 0)
            {
                for (var i = 1; i < 3; i++)
                    collectedPoints.Push(lastId - i);
            }

            if (pointId == lastId - 1)
            {
                for (var i = 0; i < 2; i++)
                    collectedPoints.Push(i);
            }
        }
        
        private static void CollectWhenHandlePoint(Vector2[] positions, int id, Stack<int> collectedPoints)
        {
            if (id % 3 == 0) 
                return;

            var m = FindMainPoint(id);

            if (Vector2.Distance(positions[id], positions[m]) > checkNoHandle)
                return;
                
            collectedPoints.Push(m);

            if (m == 0)
                collectedPoints.Push(positions.Length - 1);
            
            if (m == positions.Length - 1)
                collectedPoints.Push(0);
            
            var id2 = FindMirroredHandle(positions.Length, id);
            collectedPoints.Push(id2);
        }

        private static bool CheckIfMirrored(Vector2 v0, Vector2 v1, Vector2 v2)
        {
            var d1 = v0 - v1;
            var d2 = v2 - v1;
            d2.Normalize();
            d1.Normalize();
            var angle = Mathf.Acos(d1.x * d2.x + d1.y * d2.y);
            angle *= Mathf.Rad2Deg;
            return Math.Abs(Mathf.Abs(angle) - 180f) < 0.1f || float.IsNaN(angle);
        }
        
        private static Vector2 CorrectMirrored(Vector2 v0, Vector2 v1, float distance)
        {
            var d0 = v1 - v0;
            if (Mathf.Abs(d0.x) < 0.00001f)
            {
                return v1 + (v0.y <= v1.y ? Vector2.up : Vector2.down) * distance;
            }
            if (Mathf.Abs(d0.y) < 0.00001f)
            {
                return v1 + (v0.x <= v1.x ? Vector2.right : Vector2.left) * distance;
            }
            
            var g0 = Mathf.Sqrt(d0.x * d0.x + d0.y * d0.y);
            return v0 + (g0 + distance) * d0 / g0;
        }
    }
}

using System.Collections.Generic;
using UnityEngine;

namespace LiveShapes.Source.Scripts.ShapesEditor.Src.CurveTools.CurveAlgorithms
{
    public static class AlgorithmMoveCorners
    {
        private static bool mirroredHandleExists;
        private static float mirroredHandleDistance;
        private static int[] movablePoints;
            
        public static void PrepareToMove(Vector2[] positions, int id)
        {
            mirroredHandleExists = false;
            
            if (id < 0 || id >= positions.Length)
                return;
            
            var listOfPoints = new List<int>();
            
            var points = Collect(positions, id);
            while (points.MoveNext() && points.Current != -1)
                listOfPoints.Add(points.Current);
            
            movablePoints = listOfPoints.ToArray();
            
            if (id % 3 == 0)
            {
                mirroredHandleExists = true;
                foreach (var movablePoint in movablePoints)
                    positions[movablePoint] = positions[id];
            }
        }
        
        public static void Move(Vector2[] positions, int id, Vector2 delta)
        {
            if (id < 0 || id >= positions.Length)
                return;

            var id0 = CorrectIdByMiddle(id, positions.Length);
            var id1 = FindMainPoint(id0);
            var id2 = FindMirroredHandle(positions.Length, id0);

            foreach (var movablePoint in movablePoints)
            {
                if (movablePoint % 3 != 0 && movablePoint != id2)
                    positions[movablePoint] -= delta;
            }

            if (mirroredHandleExists)
                positions[id2] = CorrectMirrored(positions[id0], positions[id1]);
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

        private static IEnumerator<int> Collect(Vector2[] positions, int pointId)
        {
            var collectedPoints = new Stack<int>();
            collectedPoints.Push(pointId);

            CollectWhenMainPoint(positions, pointId, collectedPoints);

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

        private static int CorrectIdByMiddle(int id, int positionsCount)
        {
            if (id % 3 != 0)
                return id;
            
            return id >= positionsCount - 1 ? 1 : ++id;
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

        private static Vector2 CorrectMirrored(Vector2 v0, Vector2 v1)
        {
            var distance = Vector2.Distance(v0, v1);
            
            var d0 = v1 - v0;
            if (Mathf.Abs(d0.x) < 0.01f)
            {
                return v1 + (v0.y <= v1.y ? Vector2.up : Vector2.down) * distance;
            }
            if (Mathf.Abs(d0.y) < 0.01f)
            {
                return v1 + (v0.x <= v1.x ? Vector2.right : Vector2.left) * distance;
            }
            
            var g0 = Mathf.Sqrt(d0.x * d0.x + d0.y * d0.y);
            return v0 + (g0 + distance) * d0 / g0;
        }
    }
}

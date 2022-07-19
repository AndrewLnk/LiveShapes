using System.Collections.Generic;
using LiveShapes.Source.Scripts.ShapesEditor.Src.Other;
using UnityEngine;

namespace LiveShapes.Source.Scripts.ShapesEditor.Src.MeshTools.MeshSetup
{
    public static class AlgorithmTriangulation
    {
        public static List<int> TriangulateCycled(List<Vector3> positions)
        {
            if (positions.Count < 3)
                return new List<int>();
            
            var result = new List<int>();
            
            var ids = new List<int>();
            for (var i = 0; i < positions.Count; i++) // TODO 4.3 KB
                ids.Add(i);
            
            var checkIntersectionCount = 0;
            var success = true;
            while (ids.Count >= 3) // TODO 40 KB, 19 MS
            {
                if (checkIntersectionCount > ids.Count)
                {
                    LiveLog.LogWarning("Cannot generate mesh correctly due to intersecting curves!");
                    success = false;
                    break;
                }

                var orientedClockwise =
                    LiveMath.IsTriangleOrientedClockwise(positions[ids[0]], positions[ids[1]], positions[ids[2]]);

                if (orientedClockwise && IsTriangleClean(positions, ids))
                {
                    result.AddRange(new []{ids[0], ids[1], ids[2]});
                    ids.RemoveAt(1);
                    checkIntersectionCount = 0;
                }
                else
                {
                    ids.AddRange(ids.GetRange(0,1));
                    ids.RemoveRange(0,1);
                    checkIntersectionCount++;
                }
            }

            if (success) LiveLog.ResetWarnings();

            return result;
        }
        
        private static bool IsTriangleClean(IReadOnlyList<Vector3> pos, IReadOnlyList<int> ids)
        {
            for (var i = 3; i < ids.Count; i++)
            {
                if (LiveMath.CheckPointInTriangleClockWise(pos[ids[i]], pos[ids[0]], pos[ids[1]], pos[ids[2]]))
                    return false;
            }

            return true;
        }
        
        // Profiler.EndSample();
        // Profiler.BeginSample("BetterEditor 2");
    }
}

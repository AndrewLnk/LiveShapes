using UnityEngine;

namespace LiveShapes.Source.Scripts.ShapesEditor.Src.CurveTools.CurveAlgorithms
{
    public static class AlgorithmsOther
    {
        public static bool SelectFilter(Mode mode, int id)
        {
            // if (LsStatics.AllHandlesHide)
            //     return false;
            //
            // if (LsStatics.HandlesHide && id % 3 != 0)
            //     return false;

            if (mode == Mode.Add)
                return false;

            if (mode == Mode.Remove)
                return id % 3 == 0;

            if (mode == Mode.Move)
                return true;

            if (mode == Mode.Corners)
                return true;
            
            return false;
        }

        public static void UpdateColoredPoint(out int[] output,
                                               Mode mode,
                                               int selectedId,
                                               int hoveredId,
                                               int positionsCount)
        {
            output = new[] {-1, -1};
            
            switch (mode)
            {
                case Mode.Move when selectedId != -1:
                    output[0] = selectedId;
                    if (selectedId == 0) output[1] = positionsCount - 1;
                    if (selectedId == positionsCount - 1) output[1] = 0;
                    break;
                case Mode.Add:
                    break;
                case Mode.Remove when hoveredId != -1:
                    output[0] = hoveredId;
                    break;
                case Mode.Corners when selectedId != -1:
                    output[0] = selectedId;
                    if (selectedId == 0) output[1] = positionsCount - 1;
                    if (selectedId == positionsCount - 1) output[1] = 0;
                    break;
            }
        }

        public static int GetRemovalPointId(int hoveredId, float hoveredT, int positionsCount)
        {
            if (hoveredId == positionsCount - 3 && hoveredT > 0.2f)
                return positionsCount - 1;

            if (hoveredId == 0 && hoveredT < 0.2f)
                return positionsCount - 1;
            
            return hoveredT < 0.2f ? hoveredId : hoveredId + 3;
        }
        
        public static void UpdateColoredCurves(out int[] output,
            Mode mode,
            int selectedId,
            int hoveredId,
            int removalId,
            Vector2[] positions)
        {
            output = new[] {-1, -1};
            
            switch (mode)
            {
                case Mode.Move when selectedId != -1:
                    output[0] = selectedId;
                    if (selectedId == positions.Length - 1)
                        output[0] = 0;
                    output[1] = selectedId - 3;

                    if (selectedId % 3 != 0)
                    {
                        if ((selectedId + 1) % 3 == 0)
                            output[1] = selectedId - 2;
                        else
                            output[1] = selectedId - 1;
                    }

                    if (selectedId % 3 != 0)
                    {
                        if (AlgorithmMovePoints.MirroredHandle(positions, selectedId))
                        {
                            if ((selectedId + 1) % 3 == 0)
                                output[0] = selectedId + 1;
                            else
                                output[0] = selectedId - 4;

                            if (output[0] < 0)
                                output[0] = positions.Length - 4;
                            
                            if (output[0] == positions.Length - 1)
                                output[0] = 0;
                        }
                    }
                    
                    break;
                case Mode.Add when hoveredId != -1:
                    output[0] = hoveredId;
                    break;
                case Mode.Remove when removalId != -1:
                    output[0] = removalId;
                    if (removalId == positions.Length - 1)
                        output[0] = 0;
                    output[1] = removalId - 3;
                    break;
                case Mode.Corners when selectedId != -1:
                    output[0] = selectedId;
                    if (selectedId == positions.Length - 1)
                        output[0] = 0;
                    output[1] = selectedId - 3;

                    if (selectedId % 3 != 0)
                    {
                        if ((selectedId + 1) % 3 == 0)
                            output[1] = selectedId - 2;
                        else
                            output[1] = selectedId - 1;
                    }
                    break;
            }
        }
    }
}

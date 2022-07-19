using LiveShapes.Source.Scripts.ShapesEditor.Src.MainComponents;

namespace LiveShapes.Source.Scripts.ShapesEditor.Src.CurveTools.CurveAlgorithms
{
    public static class AlgorithmRemoveMainPoint
    {
        public static void Remove(CurveObject curveObject, int removalId)
        {
            var list = curveObject.Transform.GetPositionsList();
            if (removalId != 0 && removalId != list.Count - 1)
                list.RemoveRange(removalId - 1, 3);
            else if (list.Count > 4)
            {
                list.RemoveRange(list.Count - 2, 2);
                list.RemoveRange(0, 2);
                list.Add(list[0]);
                list.RemoveAt(0);
                list.Add(list[0]);
            }
            else
            {
                list.Clear();
            }

            curveObject.SetPositions(list);
        }
    }
}

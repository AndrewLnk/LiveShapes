using System;
using LiveShapes.Source.Scripts.ShapesEditor.Src.MainComponents;
using UnityEngine;

namespace LiveShapes.Source.Scripts.ShapesEditor.Src.CreateTools
{
    public class CircleMode : ICreateMode
    {
        private Func<Vector2> getPosition;
        private CurveObject curveObject;
        
        private Vector2[] Positions => curveObject.Data.Positions;

        private Vector2 center;

        public void SetupGetPositionFunc(Func<Vector2> getPositionFunc) => getPosition = getPositionFunc;

        public void SetupCurveData(CurveObject package) => curveObject = package;

        public void Start()
        {
            LsStatics.HideCurve = false;
            curveObject.Transform.ResetPositions(13);
            center = getPosition.Invoke();
            curveObject.Transform.SetupPivot(center);
        }

        public void Update()
        {
            SetupPointsByMouse();
            CalculateOtherPoints();
        }

        public void Finish()
        {
            curveObject.EditPivot = false;
        }

        private void SetupPointsByMouse()
        {
            var pos = getPosition.Invoke();
            var distance = Vector2.Distance(pos, center);
            Positions[0] = center + new Vector2(0, distance);
            Positions[3] = center + new Vector2(distance, 0);
            Positions[6] = center + new Vector2(0, -distance);
            Positions[9] = center + new Vector2(-distance, 0);
            Positions[12] = Positions[0];
        }

        private void CalculateOtherPoints()
        {
            var pos = getPosition.Invoke();
            var halfDistance = Vector2.Distance(pos, center) * 0.554622f;
            Positions[1] = Positions[0] + new Vector2(halfDistance, 0);
            Positions[2] = Positions[3] + new Vector2(0, halfDistance);
            Positions[4] = Positions[3] + new Vector2(0, -halfDistance);
            Positions[5] = Positions[6] + new Vector2(halfDistance, 0);
            Positions[7] = Positions[6] + new Vector2(-halfDistance, 0);
            Positions[8] = Positions[9] + new Vector2(0, -halfDistance);
            Positions[10] = Positions[9] + new Vector2(0, halfDistance);
            Positions[11] = Positions[12] + new Vector2(-halfDistance, 0);
        }
    }
}

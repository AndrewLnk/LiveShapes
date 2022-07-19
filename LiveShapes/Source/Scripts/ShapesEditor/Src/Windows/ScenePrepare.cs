using UnityEditor;
using UnityEngine;

namespace LiveShapes.Source.Scripts.ShapesEditor.Src.Windows
{
    public static class ScenePrepare
    {
        private static SceneView sceneView;
        private static bool savedState; 
        private static bool in2DMode;
        private static bool orthographic;
        private static bool isRotationLocked;
        private static bool drawGizmos;

        public static void SaveSceneState(SceneView view)
        {
            in2DMode = view.in2DMode;
            orthographic = view.orthographic;
            isRotationLocked = view.isRotationLocked;
            drawGizmos = view.drawGizmos;
            Tools.hidden = true;
        }
        
        
        public static void RestoreSceneState(SceneView view)
        {
            view.in2DMode = in2DMode;
            view.orthographic = orthographic;
            view.isRotationLocked = isRotationLocked;
            view.drawGizmos = drawGizmos;
            Tools.hidden = false;
        }

        public static void RestoreSavedSceneState()
        {
            if (sceneView != null)
                RestoreSceneState(sceneView);
        }

        public static void Update(SceneView view)
        {
            sceneView = view;
            sceneView.in2DMode = true;
            sceneView.orthographic = true;
            sceneView.isRotationLocked = true;
        }

        public static void TryFocusOn(Vector2[] positions)
        {
            if (!LsStatics.ShouldFocusOn)
                return;
            
            if (sceneView == null)
                return;
            
            if (positions == null)
                return;

            LsStatics.ShouldFocusOn = false;

            if (positions.Length < 2)
            {
                sceneView.Frame(new Bounds(Vector3.zero, Vector3.one * 5), false);
            }
            else
            {
                var minPos = Vector2.positiveInfinity;
                var maxPos = Vector2.negativeInfinity;

                for (var i = 0; i < positions.Length; i++)
                {
                    if (positions[i].x < minPos.x)
                        minPos.x = positions[i].x;

                    if (positions[i].y < minPos.y)
                        minPos.y = positions[i].y;
                    
                    if (positions[i].x > maxPos.x)
                        maxPos.x = positions[i].x;
                    
                    if (positions[i].y > maxPos.y)
                        maxPos.y = positions[i].y;
                }
                
                var bounds = new Bounds();
                bounds.SetMinMax(minPos, maxPos);

                sceneView.Frame(bounds, false);
            }
        }
    }
}
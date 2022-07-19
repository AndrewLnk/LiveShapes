using LiveShapes.Source.Scripts.ShapesEditor.Src;
using LiveShapes.Source.Scripts.ShapesEditor.Src.Windows;
using UnityEditor;

namespace LiveShapes.Source.Scripts.ShapesEditor.Editor
{
    public class LiveShapesEditorCreate : EditorWindow
    {
        private static EditorEntry editor;
        
        [MenuItem("Live Shapes/Editor", false, -25)]
        public static void OpenEditor()
        {
            if (editor != null)
                return;
            
            editor = (EditorEntry)CreateInstance(typeof(EditorEntry));
            LsStatics.ShouldFocusOn = true;
            LsStatics.HideUvEditor = true;

            var window = GetWindow<SceneView>();
            ScenePrepare.SaveSceneState(window);
            window.drawGizmos = false;
            window.sceneViewState.SetAllEnabled(false);
            window.isRotationLocked = true;
            window.camera.Reset();
            window.orthographic = true;
            window.in2DMode = true;
        }
    }
}

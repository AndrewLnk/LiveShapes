using LiveShapes.Source.Scripts.Components;
using LiveShapes.Source.Scripts.ShapesEditor.Src.Other.LiveShapesData;
using UnityEditor;
using UnityEngine;

namespace LiveShapes.Components.Editor
{
    [CustomEditor(typeof(LivePolygonCollider2D))]
    public class LivePolygonCollider2DEditor : UnityEditor.Editor
    {
        [MenuItem("Live Shapes/Components/Add LivePolygonCollider2D", false, 0)]
        public static void AddComponent()
        {
            var gameObject = Selection.activeGameObject;
            if (gameObject == null)
                return;

            var component = gameObject.GetComponent<LivePolygonCollider2D>();
            if (component == null)
                component = gameObject.AddComponent<LivePolygonCollider2D>();
            
            component.FindOrCreateComponents();
            UpdateData(component);
        }

        private static void UpdateData(LivePolygonCollider2D component)
        {
            var meshFilter = component.MeshFilter;
            
            if (meshFilter == null || meshFilter.sharedMesh == null)
                return;
            
            var assetPath = AssetDatabase.GetAssetPath(meshFilter.sharedMesh.GetInstanceID());

            var subAssets = AssetDatabase.LoadAllAssetRepresentationsAtPath(assetPath);
            LiveShapeDataObject data = null;
            foreach (var asset in subAssets)
            {
                if (asset is LiveShapeDataObject)
                {
                    data = asset as LiveShapeDataObject;
                    break;
                }
            }

            component.data = data;
        }
        
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var myTarget = (LivePolygonCollider2D) target;

            if (GUILayout.Button("Recalculate"))
            {
                myTarget.FindOrCreateComponents();
                UpdateData(myTarget);
                myTarget.Recalculate();
            }
        }
    }
}

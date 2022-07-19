using System;
using LiveShapes.Source.Scripts.ShapesEditor.Images;
using LiveShapes.Source.Scripts.ShapesEditor.Images.MeshPanel;
using LiveShapes.Source.Scripts.ShapesEditor.Src.MainComponents;
using LiveShapes.Source.Scripts.ShapesEditor.Src.MeshTools.MaterialSetup;
using LiveShapes.Source.Scripts.ShapesEditor.Src.MeshTools.MeshSetup;
using UnityEditor;
using UnityEngine;

namespace LiveShapes.Source.Scripts.ShapesEditor.Src.Windows.Panels
{
    public class PanelMeshTools : IPanel
    {
        public FloatingPanel FloatingPanel { get; }
        private CurveObject currentObject;
        private readonly GUIStyle textStyle;
        private readonly GUIStyle textStyleLabel;
        private readonly GUIStyle textAdditionalStyle;

        private LiveMeshBuilder meshBuilder;
        private LiveMaterialBuilder matBuilder;

        private readonly Vector2 windowSize = new Vector2(170, 312);

        public PanelMeshTools(FloatingPanel floatingPanel)
        {
            FloatingPanel = floatingPanel;
            floatingPanel.SetupWindowSize(windowSize);
            floatingPanel.SetupTitle("Mesh", SkinMeshPanel.Instance.icon);
            floatingPanel.SetupPositionFunc(
                pos => LsStatics.FloatPanelPosition = pos,
                () => LsStatics.FloatPanelPosition);
            floatingPanel.SetupClosePanelAction(() =>
            {
                LsStatics.ShowPanelMesh = false;
                LsStatics.HideUvEditor = true;
            });
            textStyle = Customization.TextStyle;
            textStyleLabel = Customization.TextLabelPanelStyle;
            textAdditionalStyle = Customization.TextPanelAdditionalStyle;
        }

        public void AddMeshBuilder(LiveMeshBuilder builder) => meshBuilder = builder;

        public void AddMaterialBuilder(LiveMaterialBuilder builder) => matBuilder = builder;

        public void UpdatePanel()
        {
            if (!LsStatics.ShowPanelMesh)
                return;

            FloatingPanel.Update();
        }

        public void Update(CurveObject curveObject)
        {
            if (!LsStatics.ShowPanelMesh)
                return;

            currentObject = curveObject;
            DrawVisibleMeshToggle();
            
            var position = 26f;
            DrawTitle("Generate Settings", 7, position);
            position += 15;
            DrawTextLabel(position, "Density");
            position += 19;
            DrawMeshDensityInput(position);
            position += 20;
            DrawTextLabel(position, "Optimization");
            position += 19;
            DrawMeshOptimizationInput(position);
            position += 25;
            DrawMeshStat(position);
            position += 32;
            DrawHorizontalLine(position);

            position += 7;
            DrawTitle("UV Settings", 7, position);
            DrawVisibleUVToggle(position, ref LsStatics.HideUvEditor, () =>
            {
                if (currentObject.EditPivot)
                    currentObject.EditPivot = false;
            });
            DrawResetUV(position);
            position += 15;

            var uvPos = curveObject.UvData.LocalPosition;
            DrawTransformInput(position, "Position", ref uvPos);
            curveObject.UvData.LocalAnchor += uvPos - curveObject.UvData.LocalPosition;
            curveObject.UvData.LocalPosition = uvPos;

            position += 40;
            var scale = curveObject.UvData.UVShapeScale;
            var savedScale = scale;
            DrawFloatInput(position, "Scale", ref scale);
            if (Math.Abs(savedScale - scale) > 0.005f)
                curveObject.UvData.UVShapeScale = scale;
            
            position += 20;
            var angle = curveObject.UvData.LocalAngle;
            DrawFloatInput(position, "Angle", ref angle);
            if (Mathf.Abs(curveObject.UvData.LocalAngle - angle) > 0.001f)
                curveObject.UvData.LocalAngle = angle;
            
            position += 24;
            DrawHorizontalLine(position);
            position += 7;
            DrawTitle("Painting", 7, position);
            DrawChooseMaterialMode(position);
            position += 19;
            SetupMaterial(position);
        }

        private void DrawTitle(string text, float positionX, float positionY)
        {
            var position = FloatingPanel.WindowPosition +
                           new Vector2(positionX, positionY);

            var rect = new Rect(position, new Vector2(50, 8));
            GUI.Box(rect, text, textStyleLabel);
        }
        
        private void DrawVisibleMeshToggle()
        {
            var rectPosition = new Vector2(FloatingPanel.WindowSize.x - 36, 3.5f);
            LGUI.VisibleToggle(FloatingPanel, rectPosition, ref LsStatics.HideMesh, null);
        }

        private void DrawVisibleUVToggle(float position, ref bool active, Action action = null)
        {
            var rectPosition = new Vector2(80, position - 1.55f);
            LGUI.VisibleToggle(FloatingPanel, rectPosition, ref active, action);
        }
        
        private void DrawResetUV(float position)
        {
            var rectPosition = new Vector2(FloatingPanel.WindowSize.x - 20, position);
            rectPosition += FloatingPanel.WindowPosition;
            var rect = new Rect(rectPosition, new Vector2(14, 14));
            var style = new GUIStyle()
            {

            };
            style.active.background = SkinMeshPanel.Instance.resetActive;
            if (GUI.Button(rect, SkinMeshPanel.Instance.reset, style))
                currentObject.ResetUv();
        }

        private void DrawHorizontalLine(float positionY)
        {
            var rect = new Rect(FloatingPanel.WindowPosition +
                                new Vector2(0, positionY),
                new Vector2(FloatingPanel.WindowSize.x, 0.5f));

            GUI.DrawTexture(rect, Texture2D.whiteTexture, ScaleMode.ScaleAndCrop, false, 1,
                Customization.PanelBordersColor, 0f, 0f);
        }

        private void DrawMeshDensityInput(float positionY)
        {
            var position = FloatingPanel.WindowPosition + new Vector2(7, positionY);
            var rect = new Rect(position, new Vector2(FloatingPanel.WindowSize.x - 14, 15f));
            meshBuilder.GenerateMeshSteps = EditorGUI.IntSlider(rect, meshBuilder.GenerateMeshSteps, 1, 100);
        }
        
        private void DrawMeshOptimizationInput(float positionY)
        {
            var position = FloatingPanel.WindowPosition + new Vector2(7, positionY);
            var rect = new Rect(position, new Vector2(FloatingPanel.WindowSize.x - 14, 15f));
            meshBuilder.MeshOptimization = EditorGUI.IntSlider(rect, meshBuilder.MeshOptimization, 0, 100);
        }

        private void DrawMeshStat(float positionY)
        {
            var position = FloatingPanel.WindowPosition + new Vector2(7, positionY);
            var rect = new Rect(position, new Vector2(FloatingPanel.WindowSize.x - 13, 25f));
            var color = Customization.PanelWhiteBorderColor;
            GUI.DrawTexture(rect, Texture2D.whiteTexture, ScaleMode.ScaleAndCrop, false, 1, color, 0f, 5f);
            rect.position += new Vector2(7, 0);

            var trisCount = currentObject.MeshFilter.sharedMesh != null
                ? currentObject.MeshFilter.sharedMesh.triangles.Length / 3
                : 0;
            GUI.Label(rect, $"Tris (optimized): {trisCount}", textStyle);
        }

        private void DrawTransformInput(float positionY, string label, ref Vector2 value) =>
            LGUI.Vector2Input(FloatingPanel, positionY, label, ref value);

        private void DrawTextLabel(float positionY, string text)
        {
            var position = FloatingPanel.WindowPosition + new Vector2(7, positionY);
            var rect = new Rect(position, new Vector2(FloatingPanel.WindowSize.x - 14, 15f));
            
            GUI.Box(rect, text, textAdditionalStyle);
        }
        
        private void DrawFloatInput(float positionY, string label, ref float value)
        {
            var position = FloatingPanel.WindowPosition + new Vector2(7, positionY);
            var rect = new Rect(position, new Vector2(FloatingPanel.WindowSize.x - 14, 15f));
            
            GUI.Box(rect, label, textAdditionalStyle);
            
            rect.position += new Vector2(40, 1);
            rect.size += new Vector2(-40, 0);
            value = EditorGUI.FloatField(rect, value);
        }

        private void DrawChooseMaterialMode(float position)
        {
            var rectPosition = new Vector2(FloatingPanel.WindowSize.x / 2 - 12, position - 2);
            rectPosition += FloatingPanel.WindowPosition;
            var rectSize = new Vector2(FloatingPanel.WindowSize.x / 2 + 5, 22);
            var rect = new Rect(rectPosition, rectSize);

            var savedSetupId = matBuilder.SetupPaintingId;
            matBuilder.SetupPaintingId = EditorGUI.Popup(rect, matBuilder.SetupPaintingId, matBuilder.ModeNames);
            CorrectWindowSize();

            if (savedSetupId != matBuilder.SetupPaintingId)
                FloatingPanel.ReplaceWindowByDelta(Vector2.zero);
        }
        
        private void SetupMaterial(float positionY)
        {
            if (matBuilder == null)
                return;
            
            var position = FloatingPanel.WindowPosition + new Vector2(7, positionY);
            var rect = new Rect(position, new Vector2(FloatingPanel.WindowSize.x - 14, 15f));

            switch (matBuilder.SetupPaintingId)
            {
                case 0:
                    SetupColorToShape(rect);
                    break;
                case 1:
                    SetupTextureToShape(rect);
                    break;
                case 2:
                    SetupColorToShape(rect);
                    rect.position += new Vector2(0, 22);
                    SetupTextureToShape(rect);
                    break;
                case 3:
                    SetupGradientToShape(rect);
                    break;
                case 4:
                    SetupMaterialToShape(rect);
                    break;
            }
        }

        private void SetupColorToShape(Rect rect)
        {
            matBuilder.Color = EditorGUI.ColorField(rect, matBuilder.Color);
        }

        private void SetupGradientToShape(Rect rect)
        {
            matBuilder.Gradient = EditorGUI.GradientField(rect, matBuilder.Gradient);
            rect.position += new Vector2(-2, 20f);

            rect.size = new Vector2(FloatingPanel.WindowSize.x / 2 + 10, rect.size.y);
            matBuilder.GradientType = EditorGUI.Popup(rect, matBuilder.GradientType, matBuilder.GradientTypeNames);

            rect.position += new Vector2(FloatingPanel.WindowSize.x / 2 + 15, 0f);
            rect.size = new Vector2(FloatingPanel.WindowSize.x / 2 - 25f, rect.size.y);
            matBuilder.GradientResolution =
                EditorGUI.Popup(rect, matBuilder.GradientResolution, matBuilder.GradResolutionNames);
        }

        private void SetupTextureToShape(Rect rect)
        {
            matBuilder.Texture2D = (Texture2D) EditorGUI.ObjectField(
                rect, "", matBuilder.Texture2D, typeof(Texture2D), false);
            
            rect.position += new Vector2(0, 3f);
            
            rect.position += new Vector2(0, 19f);
            GUI.Box(rect, "Tiling", textAdditionalStyle);
            rect.position += new Vector2(40, 1f);
            rect.size -= new Vector2(40, 0);
            var tiling = new Vector2(matBuilder.Texture2DPosition.x, matBuilder.Texture2DPosition.y);
            tiling = EditorGUI.Vector2Field(rect, "", tiling);
            
            rect.position += new Vector2(0, 19f);
            rect.position -= new Vector2(40, 0f);
            GUI.Box(rect, "Offset", textAdditionalStyle);
            rect.position += new Vector2(40, 1f);
            var offset = new Vector2(matBuilder.Texture2DPosition.z, matBuilder.Texture2DPosition.w);
            offset = EditorGUI.Vector2Field(rect, "", offset);

            matBuilder.Texture2DPosition = new Vector4(tiling.x, tiling.y, offset.x, offset.y);
        }

        private void SetupMaterialToShape(Rect rect)
        {
            matBuilder.Material = (Material) EditorGUI.ObjectField(
                rect, "", matBuilder.Material, typeof(Material), false);
        }

        private void CorrectWindowSize()
        {
            if (matBuilder == null)
                return;

            if (matBuilder.SetupPaintingId == 1)
            {
                FloatingPanel.SetupWindowSize(windowSize + new Vector2(0, 42));
                return;
            }

            if (matBuilder.SetupPaintingId == 2)
            {
                FloatingPanel.SetupWindowSize(windowSize + new Vector2(0, 64));
                return;
            }
            
            if (matBuilder.SetupPaintingId == 3)
            {
                FloatingPanel.SetupWindowSize(windowSize + new Vector2(0, 20));
                return;
            }

            FloatingPanel.SetupWindowSize(windowSize);
        }
    }
}

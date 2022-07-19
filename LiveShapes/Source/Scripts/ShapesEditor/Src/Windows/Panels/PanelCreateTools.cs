using System;
using System.Linq;
using LiveShapes.Source.Scripts.ShapesEditor.Images;
using LiveShapes.Source.Scripts.ShapesEditor.Images.CreatePanel;
using LiveShapes.Source.Scripts.ShapesEditor.Src.CreateTools;
using LiveShapes.Source.Scripts.ShapesEditor.Src.ExportTools;
using LiveShapes.Source.Scripts.ShapesEditor.Src.MainComponents;
using LiveShapes.Source.Scripts.ShapesEditor.Src.MeshTools.MaterialSetup;
using LiveShapes.Source.Scripts.ShapesEditor.Src.MeshTools.MeshSetup;
using LiveShapes.Source.Scripts.ShapesEditor.Src.MeshTools.UvSetup;
using LiveShapes.Source.Scripts.ShapesEditor.Src.Other;
using LiveShapes.Source.Scripts.ShapesEditor.Src.Other.LiveShapesData;
using UnityEditor;
using UnityEngine;

namespace LiveShapes.Source.Scripts.ShapesEditor.Src.Windows.Panels
{
    public class PanelCreateTools : IPanel
    {
        private const float buttonSize = 26f;
        public FloatingPanel FloatingPanel { get; }
        private ShapesCreator shapesCreator;
        private UvEditor uvEditor;
        private CurveObject currentObject;
        private readonly GUIStyle textStyleLabel;
        private LiveMeshBuilder liveMeshBuilder;
        private LiveMaterialBuilder liveMaterialBuilder;
        private ExportToFileBuilder exportToFileBuilder;
        
        private LiveShapeDataObject liveShapeData;

        public PanelCreateTools(FloatingPanel floatingPanel)
        {
            FloatingPanel = floatingPanel;
            floatingPanel.SetupWindowSize(new Vector2(150, 104));
            floatingPanel.SetupTitle("Create", SkinCreatePanel.Instance.icon);
            floatingPanel.SetupPositionFunc(
                pos => LsStatics.FloatPanelPosition = pos,
                        () => LsStatics.FloatPanelPosition);
            floatingPanel.SetupClosePanelAction(() => LsStatics.ShowPanelCreate = false);
            textStyleLabel = Customization.TextLabelPanelStyle;
        }

        public void AddShapesCreator(ShapesCreator creator) => shapesCreator = creator;
        public void AddUvEditor(UvEditor editor) => uvEditor = editor;
        public void AddLiveMeshBuilder(LiveMeshBuilder meshBuilder) => liveMeshBuilder = meshBuilder;
        public void AddLiveMaterialBuilder(LiveMaterialBuilder materialBuilder) => liveMaterialBuilder = materialBuilder;
        
        public void AddLiveFileBuilder(ExportToFileBuilder fileBuilder) => exportToFileBuilder = fileBuilder;

        public void UpdatePanel()
        {
            if (!LsStatics.ShowPanelCreate)
                return;
            
            FloatingPanel.Update();
        }

        public void Update(CurveObject curveObject)
        {
            if (!LsStatics.ShowPanelCreate)
                return;
            
            currentObject = curveObject;
            
            var position = 26;
            DrawTitle("Load LiveShape", 7, position);
            position += 18;
            DrawLoadFromFile(position);
            position += 23;
            DrawHorizontalLine(position);
            position += 5;
            
            for (var i = 0; i < 4; i++)
                DrawButton(i, position);
        }
        
        private void DrawButton(int id, float position)
        {
            var color = GetState(id) ? 
                Customization.PanelButtonSelectedColor : 
                Customization.PanelButtonDefaultColor;
            var outlineColor = Customization.PanelButtonOutlineColor;
            var rect = new Rect(FloatingPanel.WindowPosition, Vector2.one * buttonSize);
            rect.position += new Vector2(8f + rect.size.x * id + 9.5f * id, position);
            
            var buttonRect = new Rect(rect);
            buttonRect.size -= Vector2.one * 2.5f;
            buttonRect.position += Vector2.one * 1.5f;
            
            if (GetAvailableState(id))
                LGUI.PanelButton(buttonRect, GetAction(id), GetState(id));
            
            GUI.DrawTexture(rect, Texture2D.whiteTexture, ScaleMode.ScaleAndCrop, false, 1, color, 0f, 5f);
            GUI.DrawTexture(rect, Texture2D.whiteTexture, ScaleMode.ScaleAndCrop, false, 1, outlineColor, 1.5f, 5f);

            var iconRect = new Rect(rect);
            iconRect.size = Vector2.one * 14f;
            iconRect.position += (Vector2.one * buttonSize - iconRect.size) / 2;
            
            GUI.DrawTexture(iconRect, GetIcon(id));
            
            if (!GetAvailableState(id))
                GUI.DrawTexture(rect, Texture2D.whiteTexture, ScaleMode.ScaleAndCrop, false, 1, 
                    Customization.PanelNoAvailableLayer, 0f, 5f);
        }
        
        private Texture2D GetIcon(int buttonId)
        {
            switch (buttonId)
            {
                case 0:
                    return SkinCreatePanel.Instance.createRect;
                case 1:
                    return SkinCreatePanel.Instance.createCircle;
                case 2:
                    return SkinCreatePanel.Instance.createTriangle;
                case 3:
                    return SkinCreatePanel.Instance.removeShape;
                default:
                    return Texture2D.whiteTexture;
            }
        }

        private Action GetAction(int buttonId)
        {
            switch (buttonId)
            {
                case 3:
                    return () =>
                    {
                        currentObject.Transform.ResetPositions(0);
                        currentObject.Transform.HidePivot = true;
                        currentObject.ClearUv();
                        LsStatics.HideUvEditor = true;
                    };
                default:
                    return () => shapesCreator.DrawCurveShapeId = shapesCreator.DrawCurveShapeId == buttonId ? -1 : buttonId;
            }
        }

        private bool GetState(int buttonId)
        {
            switch (buttonId)
            {
                case 3:
                    return false;
                default:
                    return shapesCreator.DrawCurveShapeId == buttonId; 
            }
        }

        private bool GetAvailableState(int buttonId)
        {
            switch (buttonId)
            {
                case 3:
                    return currentObject.Data.Positions.Length > 0;
                default:
                    return currentObject.Data.Positions.Length == 0;
            }
        }
        
        private void DrawTitle(string text, float positionX, float positionY)
        {
            var position = FloatingPanel.WindowPosition +
                           new Vector2(positionX, positionY);

            var rect = new Rect(position, new Vector2(50, 8));
            GUI.Box(rect, text, textStyleLabel);
        }

        private void DrawLoadFromFile(float position)
        {
            var rect = new Rect(FloatingPanel.WindowPosition, new Vector2(FloatingPanel.WindowSize.x - 14 - 45, 15));
            rect.position += new Vector2(7, position);
            liveShapeData = (LiveShapeDataObject) EditorGUI.ObjectField(rect, "", liveShapeData, typeof(LiveShapeDataObject), false);
            
            rect.position = new Vector2(FloatingPanel.WindowPosition.x + FloatingPanel.WindowSize.x - 40 - 7, 
                rect.position.y - 1.5f);
            rect.size = new Vector2(40, 16);
            if (GUI.Button(rect, "Load", Customization.ButtonStyle))
            {
                if (liveShapeData == null)
                {
                    LiveLog.LogNoFileToLoadWarning();
                    return;
                }
                
                LoadFile();
            }
        }
        
        private void DrawHorizontalLine(float positionY)
        {
            var rect = new Rect(FloatingPanel.WindowPosition +
                                new Vector2(0, positionY),
                new Vector2(FloatingPanel.WindowSize.x, 0.5f));

            GUI.DrawTexture(rect, Texture2D.whiteTexture, ScaleMode.ScaleAndCrop, false, 1,
                Customization.PanelBordersColor, 0f, 0f);
        }

        private void LoadFile()
        {
            var savedPosition = currentObject.Transform.Position;
            exportToFileBuilder.TryUpdatePath(AssetDatabase.GetAssetPath(liveShapeData));
            currentObject.Transform.ResetPositions(0);
            currentObject.SetPositions(liveShapeData.CurveLocalPositions.ToList());
            liveMeshBuilder.GenerateMeshSteps = liveShapeData.GenerateMeshSteps;
            liveMeshBuilder.MeshOptimization = liveShapeData.MeshOptimization;
            currentObject.UvData.LocalPosition = liveShapeData.UvLocalPosition;
            currentObject.UvData.LocalAnchor = liveShapeData.UvLocalAnchor;
            liveMaterialBuilder.SetupPaintingId = liveShapeData.SetupPainting;
            liveMaterialBuilder.Color = liveShapeData.PaintingColor;
            liveMaterialBuilder.GradientType = liveShapeData.GradientType;
            liveMaterialBuilder.Gradient = liveShapeData.Gradient.Clone();
            liveMaterialBuilder.GradientResolution = liveShapeData.GradientResolution;
            liveMaterialBuilder.Texture2D = EditorUtility.InstanceIDToObject(liveShapeData.Texture2DId) as Texture2D;
            liveMaterialBuilder.Texture2DPosition = liveShapeData.Texture2DPosition;
            liveMaterialBuilder.Material = EditorUtility.InstanceIDToObject(liveShapeData.MaterialId) as Material;
            currentObject.Transform.Position = savedPosition;
        }
    }
}

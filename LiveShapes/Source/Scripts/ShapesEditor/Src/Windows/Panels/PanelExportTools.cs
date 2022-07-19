using LiveShapes.Source.Scripts.ShapesEditor.Images;
using LiveShapes.Source.Scripts.ShapesEditor.Images.ExportPanel;
using LiveShapes.Source.Scripts.ShapesEditor.Images.MainPanel;
using LiveShapes.Source.Scripts.ShapesEditor.Src.ExportTools;
using LiveShapes.Source.Scripts.ShapesEditor.Src.MainComponents;
using UnityEditor;
using UnityEngine;

namespace LiveShapes.Source.Scripts.ShapesEditor.Src.Windows.Panels
{
    public class PanelExportTools : IPanel
    {
        public FloatingPanel FloatingPanel { get; }
        private CurveObject currentObject;
        private readonly GUIStyle textStyle;
        private readonly GUIStyle textStyleLabel;
        private readonly GUIStyle textAdditionalStyle;
        
        private ExportToFileBuilder exportToFileBuilder;
        
        private readonly Vector2 windowSize = new Vector2(170, 196);

        public PanelExportTools(FloatingPanel floatingPanel)
        {
            FloatingPanel = floatingPanel;
            floatingPanel.SetupWindowSize(windowSize);
            floatingPanel.SetupTitle("Export", SkinExportPanel.Instance.icon);
            floatingPanel.SetupPositionFunc(
                pos => LsStatics.FloatPanelPosition = pos,
                        () => LsStatics.FloatPanelPosition);
            floatingPanel.SetupClosePanelAction(() => { LsStatics.ShowPanelExport = false; });
            textStyle = Customization.TextStyle;
            textStyleLabel = Customization.TextLabelPanelStyle;
            textAdditionalStyle = Customization.TextPanelAdditionalStyle;
        }

        public void AddExportToFileBuilder(ExportToFileBuilder builder) => exportToFileBuilder = builder;

        public void UpdatePanel()
        {
            if (!LsStatics.ShowPanelExport)
                return;
            
            FloatingPanel.Update();
        }

        public void Update(CurveObject curveObject)
        {
            if (!LsStatics.ShowPanelExport)
                return;
            
            currentObject = curveObject;
            var position = 28f;
            
            DrawTitle("File Path", 7, position);
            position -= 4;
            DrawButtonGetPath(position);
            position += 23f;
            DrawPathView(position);
            position += 31f;
            DrawPathDescription(position);
            position += 18f;
            DrawHorizontalLine(position);
            position += 7f;
            DrawTitle("File Name", 7, position);
            position += 16f;
            DrawFileNameInput(position);
            position += 25f;
            DrawHorizontalLine(position);
            position += 9f;
            DrawTitle("Export", 7, position);
            position -= 3f;
            DrawChooseExportFormat(position);
            position += 21f;
            DrawAdditionalExportSettings(position);
            DrawButtonExport(23);
        }
        
        private void DrawTitle(string text, float positionX, float positionY)
        {
            var position = FloatingPanel.WindowPosition +
                           new Vector2(positionX, positionY);

            var rect = new Rect(position, new Vector2(50, 8));
            GUI.Box(rect, text, textStyleLabel);
        }
        
        private void DrawPathView(float positionY)
        {
            var position = FloatingPanel.WindowPosition + new Vector2(7, positionY);
            var rect = new Rect(position, new Vector2(FloatingPanel.WindowSize.x - 14, 25f));
            var color = Customization.PanelWhiteBorderColor;
            GUI.DrawTexture(rect, Texture2D.whiteTexture, ScaleMode.ScaleAndCrop, false, 1, color, 0f, 5f);
            rect.position += new Vector2(5, 0);
            rect.size -= new Vector2(10, 0);
            var style = new GUIStyle(textStyle) { clipping = TextClipping.Clip };
            GUI.TextField(rect, exportToFileBuilder.ExportAssetPath, style);
        }
        
        private void DrawButtonGetPath(float position)
        {
            var rectPosition = new Vector2(FloatingPanel.WindowSize.x / 2, position);
            rectPosition += FloatingPanel.WindowPosition;
            var rectSize = new Vector2(FloatingPanel.WindowSize.x / 2 - 7f, 15);
            var rect = new Rect(rectPosition, rectSize);
            
            if (GUI.Button(rect, "Get Path", Customization.ButtonStyle))
                exportToFileBuilder.TryUpdatePath();
        }
        
        private void DrawPathDescription(float positionY)
        {
            var position = FloatingPanel.WindowPosition + new Vector2(7, positionY);
            var rect = new Rect(position, new Vector2(FloatingPanel.WindowSize.x - 14, 20f));
            var style = new GUIStyle(textStyle)
            {
                wordWrap = true, 
                normal = {textColor = Customization.PanelTextAdditionalColor},
                alignment = TextAnchor.UpperLeft
            };
            GUI.Label(rect, $"Select folder in \"Project\".", style);
        }
        
        private void DrawFileNameInput(float positionY)
        {
            var position = FloatingPanel.WindowPosition + new Vector2(7, positionY);
            var rect = new Rect(position, new Vector2(FloatingPanel.WindowSize.x - 14, 18f));
            var style = new GUIStyle(GUI.skin.textField)
            {
                fontSize = 11, 
                alignment = TextAnchor.MiddleLeft,
                normal = {textColor = Customization.PanelTextInputFieldColor}
            };
            exportToFileBuilder.FileName = EditorGUI.TextField(rect, exportToFileBuilder.FileName, style);
        }
        
        private void DrawHorizontalLine(float positionY)
        {
            var rect = new Rect(FloatingPanel.WindowPosition + 
                                new Vector2(0, positionY), 
                new Vector2(FloatingPanel.WindowSize.x, 0.5f));
            
            GUI.DrawTexture(rect, Texture2D.whiteTexture, ScaleMode.ScaleAndCrop, false, 1, 
                Customization.PanelBordersColor, 0f, 0f);
        }
        
        private void DrawChooseExportFormat(float position)
        {
            var rectSize = new Vector2(110, 15);
            var rectPosition = new Vector2(FloatingPanel.WindowSize.x - rectSize.x - 7, position);
            rectPosition += FloatingPanel.WindowPosition;
            var rect = new Rect(rectPosition, rectSize);

            var savedFormat = exportToFileBuilder.ConvertFormatId;
            exportToFileBuilder.ConvertFormatId = EditorGUI.Popup(rect, exportToFileBuilder.ConvertFormatId,
                exportToFileBuilder.FormatNames);

            CorrectWindowSize();
            if (savedFormat != exportToFileBuilder.ConvertFormatId)
                FloatingPanel.ReplaceWindowByDelta(Vector2.zero);
        }

        private void DrawAdditionalExportSettings(float position)
        {
            var rectPosition = new Vector2(7, position);
            rectPosition += FloatingPanel.WindowPosition;
            var rectSize = new Vector2(FloatingPanel.WindowSize.x - 14, 15);
            var rect = new Rect(rectPosition, rectSize);

            if (exportToFileBuilder.ConvertFormatId == 1)
            {
                DrawExportToPngOutline(rect);
                rect.position += new Vector2(0, 20);
                DrawExportToPngOutlineColor(rect);
                rect.position += new Vector2(0, 20);
                DrawExportToPngSize(rect);
            }
            
            if (exportToFileBuilder.ConvertFormatId == 2)
            {
                DrawExportToPngOutline(rect);
                rect.position += new Vector2(0, 20);
                DrawExportToPngOutlineColor(rect);
                if (!CheckNeedHelper())
                    return;
                rect.position += new Vector2(0, 24);
                DrawHasWarning(rect);
                rect.position += new Vector2(23, 0);
                rect.size -= new Vector2(23, 0);
                DrawExportHelper(rect, "Supported painting: Color, Linear / Radial gradients.");
            }
        }

        private void DrawExportToPngSize(Rect rect)
        {
            GUI.Box(rect, "Texture size", textAdditionalStyle);
            
            rect.size = new Vector2(77, rect.size.y);
            rect.position = new Vector2(FloatingPanel.WindowSize.x - rect.size.x - 7, rect.position.y);
            rect.position += new Vector2(FloatingPanel.WindowPosition.x, 0);
            exportToFileBuilder.SpriteSize = EditorGUI.Popup(rect, exportToFileBuilder.SpriteSize,
                exportToFileBuilder.SpriteSizeNames);
        }
        
        private void DrawExportToPngOutline(Rect rect)
        {
            GUI.Box(rect, "Stroke size", textAdditionalStyle);
            
            rect.size = new Vector2(75, rect.size.y);
            rect.position = new Vector2(FloatingPanel.WindowSize.x - rect.size.x - 8, rect.position.y);
            rect.position += new Vector2(FloatingPanel.WindowPosition.x, 0);
            exportToFileBuilder.StrokeWidth = EditorGUI.FloatField(rect, exportToFileBuilder.StrokeWidth);
        }
        
        private void DrawExportToPngOutlineColor(Rect rect)
        {
            GUI.Box(rect, "Stroke color", textAdditionalStyle);
            
            rect.size = new Vector2(75, rect.size.y);
            rect.position = new Vector2(FloatingPanel.WindowSize.x - rect.size.x - 8, rect.position.y);
            rect.position += new Vector2(FloatingPanel.WindowPosition.x, 0);
            exportToFileBuilder.StrokeColor = EditorGUI.ColorField(rect, exportToFileBuilder.StrokeColor);
            exportToFileBuilder.StrokeColor.a = 255;
        }
        
        private void DrawExportHelper(Rect rect, string info)
        {
            var style = new GUIStyle(textStyle)
            {
                wordWrap = true, 
                normal = {textColor = Customization.PanelTextAdditionalColor},
                alignment = TextAnchor.MiddleLeft
            };
            
            GUI.Box(rect, info, style);
        }
        
        private void CorrectWindowSize()
        {
            if (exportToFileBuilder == null)
                return;

            if (exportToFileBuilder.ConvertFormatId == 1)
            {
                FloatingPanel.SetupWindowSize(windowSize + new Vector2(0, 60));
                return;
            }
            
            if (exportToFileBuilder.ConvertFormatId == 2)
            {
                var helperHeight = CheckNeedHelper() ? 28f : 0f;
                FloatingPanel.SetupWindowSize(windowSize + new Vector2(0, 40 + helperHeight));
                return;
            }

            FloatingPanel.SetupWindowSize(windowSize);
        }
        
        private void DrawButtonExport(float positionFromBottom)
        {
            var rectPosition = new Vector2(7f, FloatingPanel.WindowSize.y - positionFromBottom);
            rectPosition += FloatingPanel.WindowPosition;
            var rectSize = new Vector2(FloatingPanel.WindowSize.x - 14f, 15);
            var rect = new Rect(rectPosition, rectSize);

            if (GUI.Button(rect, "Export", Customization.ButtonStyle))
            {
                exportToFileBuilder.StartExport();
            }
        }

        private bool CheckNeedHelper()
        {
            return !(exportToFileBuilder.MaterialBuilder.SetupPaintingId == 0 ||
                   (exportToFileBuilder.MaterialBuilder.SetupPaintingId == 3 &&
                    (exportToFileBuilder.MaterialBuilder.GradientType == 0 ||
                     exportToFileBuilder.MaterialBuilder.GradientType == 1)));
        }
        
        private void DrawHasWarning(Rect rect)
        {
            rect.size = new Vector2(16,16);
            rect.position += new Vector2(1, 0);
            GUI.Box(rect, SkinMainPanel.Instance.warning, GUIStyle.none);
        }
    }
}

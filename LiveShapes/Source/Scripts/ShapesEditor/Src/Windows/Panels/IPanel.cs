using LiveShapes.Source.Scripts.ShapesEditor.Src.MainComponents;

namespace LiveShapes.Source.Scripts.ShapesEditor.Src.Windows.Panels
{
    public interface IPanel
    {
        FloatingPanel FloatingPanel { get; }
        void UpdatePanel();
        void Update(CurveObject curveObject);
    }
}

using UnityEngine;

namespace LiveShapes.Source.Scripts.ShapesEditor.Src.MeshTools.UvSetup
{
    public class EditSession
    {
        private readonly ContactType type;
        private readonly UvData uvData;
        
        public EditSession(UvData uvData, ContactType type)
        {
            this.type = type;
            this.uvData = uvData;
        }

        public void Update(Vector2 deltaMove)
        {
            if (type == ContactType.Position)
            {
                uvData.Position -= deltaMove;
                uvData.Anchor -= deltaMove;
            }
            
            if (type == ContactType.Anchor)
                uvData.Anchor -= deltaMove;

            if (type == ContactType.Left)
            {
                var middle = (uvData.Anchor - uvData.Position) / 2;
                var v = middle + new Vector2(-middle.y, middle.x);
                v -= deltaMove;
                uvData.Anchor = uvData.Position + v + new Vector2(v.y, -v.x);
            }
            
            if (type == ContactType.Right)
            {
                var middle = (uvData.Anchor - uvData.Position) / 2;
                var v = middle + new Vector2(middle.y, -middle.x);
                v -= deltaMove;
                uvData.Anchor = uvData.Position + v + new Vector2(-v.y, v.x);
            }
        }
        
        
        public enum ContactType
        {
            None,
            Position,
            Anchor,
            Left,
            Right
        }
    }
}

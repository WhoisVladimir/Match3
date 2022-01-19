using UnityEngine;

namespace Gameplay
{
    public class CellContentObject : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;

        public CellContent Content { get; private set; }
        public GameFieldGridCell LocationCell { get; private set; }

        /// <summary>
        /// Присваивает содержимое объекту
        /// </summary>
        /// <param name="externalContent">Содержимое</param>
        public void SetObjectContent(CellContent externalContent)
        {
            Content = externalContent;
            spriteRenderer.sprite = Content.Sprite;
        }

        public void SetObjectLocationCell(GameFieldGridCell locationCell)
        {
            if (LocationCell == null) LocationCell = locationCell;
        }

        public void DetachObjectLocationCell()
        {
            if (LocationCell != null) LocationCell = null;
        }
    }
}

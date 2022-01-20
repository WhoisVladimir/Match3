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
            Debug.Log($"Назначен контент {externalContent.ContentType}");
        }

        public void SetObjectLocationCell(GameFieldGridCell locationCell)
        {
            if (LocationCell == null) 
            {
                LocationCell = locationCell;
                Debug.Log($"Объект {this.GetHashCode()} {Content.ContentType} привязан к [{locationCell.RowNumber}, {locationCell.Index}");
            }
        }

        public void DetachObjectLocationCell()
        {
            if (LocationCell != null) 
            {
                Debug.Log($"Объект {this.GetHashCode()} {Content.ContentType} отвязан от [{LocationCell.RowNumber}, {LocationCell.Index}]");

                LocationCell = null;
            } 
        }
    }
}

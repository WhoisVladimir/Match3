using System.Collections;
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

        public void ChangePosition(GameFieldGridCell cell)
        {
            transform.position = cell.transform.position;
            LocationCell = cell;
        }

        public void DetachObjectLocationCell()
        {
            if (LocationCell != null) LocationCell = null;
        }
    }
}

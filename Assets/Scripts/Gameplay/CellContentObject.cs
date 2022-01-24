using UnityEngine;

namespace Gameplay
{
    public class CellContentObject : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private GameObject specialContentObject;

        public CellContent Content { get; private set; }
        public GameFieldGridCell LocationCell { get; private set; }
        public bool IsSpecial { get; private set; }

        /// <summary>
        /// Присваивает содержимое объекту
        /// </summary>
        /// <param name="externalContent">Содержимое</param>
        public void SetObjectContent(CellContent externalContent)
        {
            Content = externalContent;
            spriteRenderer.sprite = Content.Sprite;
        }

        /// <summary>
        /// Присваивает новую позицию и соответствующую ячейку.
        /// </summary>
        /// <param name="cell">Заполняемая ячейка</param>
        public void ChangePosition(GameFieldGridCell cell)
        {
            transform.position = cell.transform.position;
            LocationCell = cell;
        }

        /// <summary>
        /// Выполняет действия при опустошении ячейки.
        /// </summary>
        public void DetachObjectLocationCell()
        {
            if (LocationCell != null) LocationCell = null;
            if (IsSpecial) 
            {
                IsSpecial = false;
                specialContentObject.SetActive(false);
            } 
        }

        /// <summary>
        /// Назначает объекту особый статус.
        /// </summary>
        public void SetSpecial()
        {
            IsSpecial = true;
            specialContentObject.SetActive(true);
        }
    }
}

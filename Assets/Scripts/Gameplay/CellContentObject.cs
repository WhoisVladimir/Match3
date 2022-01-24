using System.Collections;
using UnityEngine;

namespace Gameplay
{
    public class CellContentObject : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private GameObject specialContentObject;

        public CellContent Content { get; private set; }
        public GameFieldGridCell LocationCell { get; private set; }
        public bool IsSpecial;

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
            if (IsSpecial) 
            {
                IsSpecial = false;
                specialContentObject.SetActive(false);
            } 
            
        }

        public void SetSpecial()
        {
            IsSpecial = true;
            specialContentObject.SetActive(true);
        }
    }
}

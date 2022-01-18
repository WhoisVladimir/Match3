using UnityEngine;

namespace Gameplay
{
    public class CellContentObject : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;

        public CellContent Content { get; private set; }

        /// <summary>
        /// Присваивает содержимое объекту
        /// </summary>
        /// <param name="externalContent">Содержимое</param>
        public void SetObjectContent(CellContent externalContent)
        {
            Content = externalContent;
            spriteRenderer.sprite = Content.Sprite;
        }
    }
}

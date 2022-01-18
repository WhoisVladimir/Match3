using UnityEngine;

namespace Gameplay
{
    public class CellContentObject : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;

        public CellContent Content { get; private set; }

        /// <summary>
        /// ����������� ���������� �������
        /// </summary>
        /// <param name="externalContent">����������</param>
        public void SetObjectContent(CellContent externalContent)
        {
            Content = externalContent;
            spriteRenderer.sprite = Content.Sprite;
        }
    }
}

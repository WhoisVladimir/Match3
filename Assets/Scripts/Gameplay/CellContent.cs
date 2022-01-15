using UnityEngine;

namespace Gameplay
{
    [CreateAssetMenu(fileName = "Cell Content", menuName = "Scriptable Objects/Cell Content")]
    public class CellContent : ScriptableObject
    {
        [SerializeField] private Sprite sprite;
        [SerializeField] private CellContentType contentType;

        public Sprite Sprite => sprite;
        public CellContentType ContentType => contentType;
    }
}

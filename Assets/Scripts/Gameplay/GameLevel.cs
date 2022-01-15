using System.Collections.Generic;
using UnityEngine;

namespace Gameplay
{
    [CreateAssetMenu(fileName = "Game Level", menuName = "Scriptable Objects/Game Level")]
    public class GameLevel : ScriptableObject
    {
        [SerializeField] private int lvlIndexNumber;
        [SerializeField] private int goalPoints;
        [SerializeField] private List<CellContent> contentItems;

        public int LvlIndexNumber => lvlIndexNumber;
        public int GoalPoints => goalPoints;
        public List<CellContent> ContentItems => contentItems;
    }
}

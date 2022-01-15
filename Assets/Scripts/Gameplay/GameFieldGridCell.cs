using System.Collections.Generic;
using UnityEngine;

namespace Gameplay
{
    public class GameFieldGridCell : MonoBehaviour
    {
        public CellContentObject ContentObject { get; private set; }
        public List<GameFieldGridCell> HorizontalAdjacentCells { get; } = new List<GameFieldGridCell>();
        public List<GameFieldGridCell> VerticalAdjacentCells { get; } = new List<GameFieldGridCell>();
        public bool IsEmpty { get; private set; } = true;

        public void SetCellContent(CellContentObject contentObj)
        {
            ContentObject = contentObj;
            IsEmpty = false;
        }
    }
}

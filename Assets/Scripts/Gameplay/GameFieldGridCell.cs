using System.Collections.Generic;
using UnityEngine;

namespace Gameplay
{
    public class GameFieldGridCell : MonoBehaviour
    {
        [SerializeField] private GameObject contentGObj;

        public CellContentObject ContentObject { get; private set; }
        //public List<GameFieldGridCell> HorizontalAdjacentCells { get; } = new List<GameFieldGridCell>();
        //public List<GameFieldGridCell> VerticalAdjacentCells { get; } = new List<GameFieldGridCell>();
        public bool IsEmpty { get; private set; } = true;

        public GameFieldGridCell LeftCell { get; private set; }
        public GameFieldGridCell RightCell { get; private set; }
        public GameFieldGridCell TopCell { get; private set; }
        public GameFieldGridCell DownCell { get; private set; }

        public void SetLeftCell(GameFieldGridCell cell)
        {
            LeftCell = cell;
        }

        public void SetRightCell(GameFieldGridCell cell)
        {
            RightCell = cell;
        }

        public void SetTopCell(GameFieldGridCell cell)
        {
            TopCell = cell;
        }

        public void SetDownCell(GameFieldGridCell cell)
        {
            DownCell = cell;
        }

        /// <summary>
        /// Ћогика заполнени€ €чейки
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="contentList">—писок доступного неповтор€ющегос€ контента</param>
        public void FillCell(List<CellContent> contentList)
        {
            if (IsEmpty)
            {
                contentGObj = Instantiate(contentGObj, transform.position, transform.rotation, GameFieldGrid.Instance.transform);
                IsEmpty = false;
            }

            ContentObject = contentGObj.GetComponent<CellContentObject>();
            var contentItemIndex = Random.Range(0, contentList.Count);
            ContentObject.SetObjectContent(contentList[contentItemIndex]);
        }
    }
}

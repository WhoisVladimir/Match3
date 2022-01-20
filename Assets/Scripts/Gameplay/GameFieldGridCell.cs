using System.Collections.Generic;
using UnityEngine;

namespace Gameplay
{
    public delegate void CellAction(GameFieldGridCell actingCell, CellContentObject contentObject);

    public class GameFieldGridCell : MonoBehaviour
    {
        public static event CellAction CellEmptying;

        [SerializeField] private GameObject contentGObj;

        public CellContentObject ContentObject { get; private set; }
        public bool IsEmpty { get; private set; } = true;
        public Dictionary<DirectionType, GameFieldGridCell> AdjacentCells { get; private set; }
        public int RowNumber { get; private set; }
        public int Index { get; private set; }

        private bool isReady;

        private void Awake()
        {
            AdjacentCells = new Dictionary<DirectionType, GameFieldGridCell>();
        }

        /// <summary>
        /// Назначение соседней ячейки
        /// </summary>
        /// <param name="direction">Сторона ячейки</param>
        /// <param name="cell">Соседняя ячейка</param>
        public void SetAdjacentCell(DirectionType direction, GameFieldGridCell cell)
        {
            AdjacentCells.Add(direction, cell);
        }

        /// <summary>
        /// Заполнение ячейки рандомным контентом
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="contentList">Список доступного неповторяющегося контента</param>
        public void FillCell(List<CellContent> contentList)
        {
            if (!isReady)
            {
                contentGObj = Instantiate(contentGObj, transform.position, transform.rotation, GameFieldGrid.Instance.transform);
                isReady = true;
            }

            ContentObject = contentGObj.GetComponent<CellContentObject>();
            var contentItemIndex = Random.Range(0, contentList.Count);
            ContentObject.SetObjectContent(contentList[contentItemIndex]);
            ContentObject.SetObjectLocationCell(this);
            IsEmpty = false;
        }

        /// <summary>
        /// Заполнение ячейки конкретном контентом
        /// </summary>
        /// <param name="contentObject">Объект содержимого ячейки</param>
        public void FillCell(CellContentObject contentObject)
        {
            ContentObject = contentObject;
            ContentObject.transform.position = transform.position;
            ContentObject.DetachObjectLocationCell();
            ContentObject.SetObjectLocationCell(this);
            IsEmpty = false;
        }

        public void FillCell(CellContentObject contentObject, List<CellContent> contentList)
        {
            FillCell(contentObject);

            var contentItemIndex = Random.Range(0, contentList.Count);
            ContentObject.SetObjectContent(contentList[contentItemIndex]);
            ContentObject.gameObject.SetActive(true);
        }

        public void EmptyCell(bool isMatch = false)
        {
            Debug.Log($"Опустошение ячейки [{RowNumber}, {Index}] ({ContentObject.Content.ContentType})");
            if (isMatch) 
            {
                ContentObject.gameObject.SetActive(false);
                Debug.Log("Объект невидим");
            } 

            ContentObject.DetachObjectLocationCell();
            var tempObject = ContentObject;
            ContentObject = null;
            IsEmpty = true;

            Debug.Log("Ячейка свободна");
            //CellEmptying?.Invoke(this, tempObject);
            GameFieldGrid.Instance.OnCellEmptying(this, tempObject);
        }

        public void SetIndex(int index, int rowNumber)
        {
            RowNumber = rowNumber;
            Index = index;
        }
    }
}

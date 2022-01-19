using System.Collections.Generic;
using UnityEngine;

namespace Gameplay
{
    public delegate void CellAction(GameFieldGridCell actingCell);

    public class GameFieldGridCell : MonoBehaviour
    {
        public static event CellAction CellEmptying;
        public static event CellAction ContentEnded;

        [SerializeField] private GameObject contentGObj;

        public CellContentObject ContentObject { get; private set; }
        public bool IsEmpty { get; private set; } = true;
        public Dictionary<DirectionType, GameFieldGridCell> AdjacentCells { get; private set; }
        public int RowNumber { get; private set; }
        public int Index { get; private set; }

        private void Awake()
        {
            AdjacentCells = new Dictionary<DirectionType, GameFieldGridCell>();
        }

        private void OnEnable()
        {
            CellEmptying += OnCellEmptying;
        }

        private void OnDisable()
        {
            CellEmptying -= OnCellEmptying;
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
            if (IsEmpty)
            {
                contentGObj = Instantiate(contentGObj, transform.position, transform.rotation, GameFieldGrid.Instance.transform);
                IsEmpty = false;
            }

            ContentObject = contentGObj.GetComponent<CellContentObject>();
            var contentItemIndex = Random.Range(0, contentList.Count);
            ContentObject.SetObjectContent(contentList[contentItemIndex]);
            ContentObject.SetObjectLocationCell(this);
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
        }

        public void EmptyCell(bool isMatch = false)
        {
            if(isMatch) ContentObject.gameObject.SetActive(false);

            ContentObject.DetachObjectLocationCell();
            ContentObject = null;
            IsEmpty = true;

            CellEmptying?.Invoke(this);
        }

        private void OnCellEmptying(GameFieldGridCell actingCell)
        {
            var gameplay = GameplayController.Instance;
            DirectionType spawnerDirection;

            if (gameplay.SpawnDirection == DirectionType.DOWN) spawnerDirection = DirectionType.TOP;
            else spawnerDirection = DirectionType.DOWN;

            if(AdjacentCells.TryGetValue(gameplay.SpawnDirection, out var item))
            {
                if (actingCell == item)
                {
                    if (ContentObject == null)
                    {
                        while (AdjacentCells.TryGetValue(spawnerDirection, out var previousCell))
                        {

                        }
                    }
                    gameplay.MoveCellContent(this, gameplay.SpawnDirection, false);
                    EmptyCell();
                }

            }
            else
            {
                ContentEnded?.Invoke(this);
            }
        }

        public void SetIndex(int index, int rowNumber)
        {
            RowNumber = rowNumber;
            Index = index;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gameplay
{
    public class GameFieldGridCell : MonoBehaviour
    {
        [SerializeField] private GameObject contentGObj;

        public CellContentObject ContentObject { get; private set; }
        public bool IsEmpty { get; private set; } = true;
        public Dictionary<DirectionType, GameFieldGridCell> AdjacentCells { get; private set; }

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
        /// Логика заполнения ячейки
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
        }

        /// <summary>
        /// Логика заполнения ячейки
        /// </summary>
        /// <param name="contentObject">Объект содержимого ячейки</param>
        public void FillCell(CellContentObject contentObject)
        {
            ContentObject = contentObject;
            ContentObject.transform.position = transform.position;
        }

        /// <summary>
        /// Проверка соседних ячеек на совпадение содержимого.
        /// </summary>
        public void CheckAdjacentCells()
        {
            Debug.Log($"Проверяем {ContentObject.Content.ContentType} {GetHashCode()}");
            var checkedCells = new Stack<GameFieldGridCell>();
            var verticalMatches = new List<CellContentObject>();
            var horizontalMatches = new List<CellContentObject>();

            verticalMatches.Add(ContentObject);
            horizontalMatches.Add(ContentObject);

            Debug.Log($"Добавлен объект-источник. vertical: {verticalMatches.Count} horizontal: {horizontalMatches.Count}");

            foreach (var item in AdjacentCells)
            {
                checkedCells.Push(item.Value);
                Debug.Log($"Заполнение очереди: {item.Key} {item.Value.ContentObject.Content.ContentType}");
            }

            var sourceCell = this;
            Debug.Log($"Хэш источника: {sourceCell.GetHashCode()}");
            while (checkedCells.Count > 0)
            {
                var targetCell = checkedCells.Pop();
                Debug.Log($"Проверка в цикле {targetCell.ContentObject.Content.ContentType}");
                Debug.Log($"Изменение очереди: {checkedCells.Count}");

                var currentDirectionType = sourceCell.AdjacentCells
                    .Where(pair => pair.Value.Equals(targetCell))
                    .Select(pair => pair.Key)
                    .FirstOrDefault();
                Debug.Log($"Тип значится как: {currentDirectionType}");
                if (sourceCell.ContentObject.Content.ContentType == targetCell.ContentObject.Content.ContentType)
                {
                    Debug.Log("Контент совпал");
                    if (currentDirectionType == DirectionType.DOWN || currentDirectionType == DirectionType.TOP)
                    {
                        verticalMatches.Add(targetCell.ContentObject);
                        Debug.Log($"Добавление в vertical. {verticalMatches.Count}");
                    }
                    else
                    {
                        horizontalMatches.Add(targetCell.ContentObject);
                        Debug.Log($"Добавление в horizontal. {horizontalMatches.Count}");
                    }

                    if (targetCell.AdjacentCells.TryGetValue(currentDirectionType, out var adjacentCell))
                    {
                        Debug.Log($"Проверка на продолжение типа ячейки: {currentDirectionType} ? {adjacentCell}");
                        checkedCells.Push(adjacentCell);
                        Debug.Log($"Изменение очереди: {checkedCells.Count}");
                        sourceCell = targetCell;
                    }
                    else sourceCell = this;

                }
                else sourceCell = this;

                
                Debug.Log($"Конец цикла. Обновление источника-ячейки {sourceCell.ContentObject.Content.ContentType} {sourceCell.GetHashCode()}");
            }

            if (verticalMatches.Count >= 3) Debug.Log($"{verticalMatches.Count} {ContentObject.Content.ContentType} by vertical!");
            else Debug.Log("No equals by vertical!");
            if (horizontalMatches.Count >= 3) Debug.Log($"{horizontalMatches.Count} {ContentObject.Content.ContentType} by horizontal!");
            else Debug.Log("No equals by horizontal!");
        }
    }
}

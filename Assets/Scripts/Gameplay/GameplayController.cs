using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gameplay
{
    public class GameplayController : Singleton<GameplayController>
    {
        [SerializeField] private List<GameLevel> gameLevels;

        private GameLevel currentLevel;
        private int boundIndex = 100;
        private List<CellContent> lvlContentItems;
        
        public DirectionType SpawnDirection = DirectionType.DOWN;

        protected override void Awake()
        {
            base.Awake();
            currentLevel = SetLevel(1);
        }

        private void Start()
        {
            lvlContentItems = currentLevel.ContentItems;
            GameFieldGrid.Instance.FillGrid(lvlContentItems);
        }

        private void OnEnable()
        {
            GameFieldGridCell.ContentEnded += OnContentEnded;
        }

        private void OnDisable()
        {
            GameFieldGridCell.ContentEnded -= OnContentEnded;
        }

        /// <summary>
        /// Назначает уровень.
        /// </summary>
        /// <param name="index">Порядковый номер уровня.</param>
        /// <returns></returns>
        private GameLevel SetLevel(int index)
        {
            //реализовать логику завершающую игру
            if (index == boundIndex) return null;

            var level = gameLevels.Find(lvl => lvl.LvlIndexNumber == index);

            if (level is null)
            {
                Debug.Log($"Level {index} not found");
                SetLevel(++index);
            }
            return level;
        }

        /// <summary>
        /// Перемещение объекта содержимого в ячейку
        /// </summary>
        /// <param name="cell">Ячейка из которой перемещается объект</param>
        /// <param name="direction">Направление в котором перемещается объект</param>
        /// <param name="isIntentialAction">Является ли перемещение следствием ввода игрока</param>
        public void MoveCellContent(GameFieldGridCell cell, DirectionType direction, bool isIntentialAction)
        {
            var contentObj = cell.ContentObject;
            if (!cell.AdjacentCells.TryGetValue(direction, out var targetCell)) Debug.Log("Bound cell");
            if (targetCell.IsEmpty) targetCell.FillCell(contentObj);
            else
            {
                Debug.Log($"Движение: {direction} {cell.ContentObject.Content.ContentType}");
                if (isIntentialAction) SwitchCellContent(cell, targetCell);
            }
        }

        /// <summary>
        /// Замена объекта содержимого.
        /// </summary>
        /// <param name="cell">Ячейка из котрой производится замена</param>
        /// <param name="targetCell">Ячейка в которую производится замена</param>
        public void SwitchCellContent(GameFieldGridCell cell, GameFieldGridCell targetCell)
        {
            var tempContentObj = targetCell.ContentObject;
            Debug.Log($"Произошла замена на {tempContentObj.Content.ContentType}");
            targetCell.FillCell(cell.ContentObject);
            cell.FillCell(tempContentObj);
            Debug.Log($"Целевая ячейка: {targetCell.ContentObject.Content.ContentType}");
            Debug.Log($"Ячейка источник: {cell.ContentObject.Content.ContentType}");

            var targetCellMatches = CheckAdjacentCells(targetCell);
            var cellMatches = CheckAdjacentCells(cell);
            if (targetCellMatches.Count > 1 || cellMatches.Count > 1)
            {
                MatchesHandling(targetCellMatches);
                MatchesHandling(cellMatches);
            }
            else
            {
                Debug.Log($"Произошла замена на {tempContentObj.Content.ContentType}");
                cell.FillCell(targetCell.ContentObject);
                targetCell.FillCell(tempContentObj);
            }
        }

        /// <summary>
        /// Проверка соседних ячеек на совпадение содержимого.
        /// </summary>
        public List<CellContentObject> CheckAdjacentCells(GameFieldGridCell sourceCell)
        {
            var contentObj = sourceCell.ContentObject;
            Debug.Log($"Проверяем {contentObj.Content.ContentType} {GetHashCode()}");
            var checkedCells = new Stack<GameFieldGridCell>();
            var verticalMatches = new List<CellContentObject>();
            var horizontalMatches = new List<CellContentObject>();

            //verticalMatches.Add(contentObj);
            //horizontalMatches.Add(contentObj);

            Debug.Log($"Добавлен объект-источник. vertical: {verticalMatches.Count} horizontal: {horizontalMatches.Count}");

            foreach (var item in sourceCell.AdjacentCells)
            {
                checkedCells.Push(item.Value);
                Debug.Log($"Заполнение очереди: {item.Key} {item.Value.ContentObject.Content.ContentType}");
            }

            var currentCell = sourceCell;
            Debug.Log($"Хэш источника: {currentCell.GetHashCode()}");
            while (checkedCells.Count > 0)
            {
                var targetCell = checkedCells.Pop();
                Debug.Log($"Проверка в цикле {targetCell.ContentObject.Content.ContentType}");
                Debug.Log($"Изменение очереди: {checkedCells.Count}");

                var currentDirectionType = currentCell.AdjacentCells
                    .Where(pair => pair.Value.Equals(targetCell))
                    .Select(pair => pair.Key)
                    .FirstOrDefault();
                Debug.Log($"Тип значится как: {currentDirectionType}");
                if (currentCell.ContentObject.Content.ContentType == targetCell.ContentObject.Content.ContentType)
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
                        currentCell = targetCell;
                    }
                    else currentCell = sourceCell;
                }
                else currentCell = sourceCell;

                Debug.Log($"Конец цикла. Обновление источника-ячейки {currentCell.ContentObject.Content.ContentType} {currentCell.GetHashCode()}");
            }

            var matches = new List<CellContentObject>();
            if (verticalMatches.Count >= 2)
            {
                Debug.Log($"{verticalMatches.Count} {contentObj.Content.ContentType} by vertical!");
                matches.AddRange(verticalMatches);
            }
            else Debug.Log("No equals by vertical!");

            if (horizontalMatches.Count >= 2)
            {
                Debug.Log($"{horizontalMatches.Count} {contentObj.Content.ContentType} by horizontal!");
                matches.AddRange(horizontalMatches);
            }
            else Debug.Log("No equals by horizontal!");

            matches.Add(contentObj);

            return matches;
        }

        public void MatchesHandling(List<CellContentObject> contentObjects)
        {
            if (contentObjects.Count < 3) return;
            Debug.Log("Обработка совпадения");
            foreach (var item in contentObjects)
            {
                item.LocationCell.EmptyCell(true);
            }
        }

        public void OnCellEmptying(GameFieldGridCell emptyCell)
        {
            DirectionType direction;
            if (SpawnDirection == DirectionType.DOWN) direction = DirectionType.TOP;
            else direction = DirectionType.DOWN;

            var adjacentCells = GameFieldGrid.Instance.GetAdjacentCells(emptyCell, direction);

            while (adjacentCells.Count > 0)
            {
                var currentCell = adjacentCells.Dequeue();
                if(currentCell.AdjacentCells.TryGetValue(SpawnDirection, out var adjacentCell))
                {
                    while (adjacentCell.IsEmpty)
                    {
                        MoveCellContent(currentCell, SpawnDirection, false);
                    }
                }
            }
        }

        private void OnContentEnded(GameFieldGridCell actingCell)
        {
            actingCell.FillCell(lvlContentItems);
        }
    }
}

using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Gameplay
{
    public class GameFieldGrid : Singleton<GameFieldGrid>
    {
        public Dictionary<int, LinkedList<CellContentObject>> LinkedContentGrid { get; private set; }

        private const int columnsCount = 5;
        private const int linesCount = 6;

        private GameFieldGridCell[,] grid;
        private List<CellContent> lvlContent;

        [SerializeField] private GameFieldGridCell cell;

        protected override void Awake()
        {
            base.Awake();
            LinkedContentGrid = new Dictionary<int, LinkedList<CellContentObject>>();
            CreateGrid();
        }

        private void OnEnable()
        {
            GameFieldGridCell.CellEmptying += OnCellEmptying;
        }

        private void OnDisable()
        {
            GameFieldGridCell.CellEmptying -= OnCellEmptying;
        }

        public void OnCellEmptying(GameFieldGridCell actingCell, CellContentObject contentObject)
        {
            Debug.Log($"Сеть отреагировала на событие в [{actingCell.RowNumber}, {actingCell.Index}]");
            var rowNumber = actingCell.RowNumber;
            LinkedContentGrid.TryGetValue(rowNumber, out var row);

            var ar = row.ToArray();
            Debug.Log($"Предыдущий список ряда ({row.Count}):");
            foreach (var item in ar)
            {
                Debug.Log(item.Content.ContentType);
            }

            var node = row.Find(contentObject);

            for (int i = actingCell.Index; i < linesCount - 1; i++)
            {
                grid[rowNumber, i].FillCell(node.Next.Value);
            }
            row.Remove(node);

            grid[rowNumber, linesCount - 1].FillCell(contentObject, lvlContent);
            row.AddLast(contentObject);

            Debug.Log($"Текущий список ряда ({row.Count}):");
            ar = row.ToArray();
            foreach (var item in ar)
            {
                Debug.Log(item.Content.ContentType);
            }


        }

        /// <summary>
        /// Создаёт сетку с пустыми ячейками, добавляет соседние ячейки.
        /// </summary>
        public void CreateGrid()
        {
            grid = new GameFieldGridCell[columnsCount, linesCount];

            for (int i = 0; i < columnsCount; i++)
            {
                for (int j = 0; j < linesCount; j++)
                {
                    var cellPosition = new Vector2(transform.position.x + i, transform.position.y + j);
                    grid[i, j] = Instantiate(cell, cellPosition, transform.rotation, transform);


                    if (i - 1 >= 0)
                    {
                        grid[i, j].SetAdjacentCell(DirectionType.LEFT, grid[i - 1, j]);
                        grid[i - 1, j].SetAdjacentCell(DirectionType.RIGHT, grid[i, j]);
                    }
                    if (j - 1 >= 0)
                    {
                        grid[i, j].SetAdjacentCell(DirectionType.DOWN, grid[i, j - 1]);
                        grid[i, j - 1].SetAdjacentCell(DirectionType.TOP, grid[i, j]);
                    }
                }
            }
        }

        /// <summary>
        /// Логика заполнения сетки. Производится проверка предыдущих ячеек на совпадение содержимого.
        /// </summary>
        /// <param name="content">Список возможного содержимого (перенести в более адекватное место)</param>
        public void FillGrid(List<CellContent> content)
        {
            lvlContent = content;
            for (int i = 0; i < columnsCount; i++)
            {
                var linkedContentRow = new LinkedList<CellContentObject>();
                LinkedContentGrid.Add(i, linkedContentRow);

                for (int j = 0; j < linesCount; j++)
                {
                    var abbreviatedList = new List<CellContent>();

                    if (j - 2 >= 0)
                    {
                        var previousContent = grid[i, j - 2].ContentObject.Content;
                        if (previousContent.ContentType.Equals(grid[i, j - 1].ContentObject.Content.ContentType))
                        {
                            abbreviatedList.AddRange(content);
                            abbreviatedList.Remove(previousContent);

                            grid[i, j].FillCell(abbreviatedList);
                        }
                        else grid[i, j].FillCell(content);
                    }

                    if (i - 2 >= 0)
                    {
                        var previousContent = grid[i - 2, j].ContentObject.Content;
                        if (previousContent.ContentType.Equals(grid[i - 1, j].ContentObject.Content.ContentType))
                        {
                            if (abbreviatedList.Count == 0) abbreviatedList.AddRange(content);

                            abbreviatedList.Remove(previousContent);
                            grid[i, j].FillCell(abbreviatedList);
                        }
                        else if (grid[i, j].IsEmpty) grid[i, j].FillCell(content);
                    }

                    if (j - 2 < 0 && i - 2 < 0) grid[i, j].FillCell(content);

                    var contentObject = grid[i, j].ContentObject;
                    if (j == 0)
                    {
                        linkedContentRow.AddFirst(contentObject);
                    }
                    else linkedContentRow.AddAfter(linkedContentRow.Last, contentObject);

                    grid[i, j].SetIndex(j, i);
                }
            }
        }

        public Queue<GameFieldGridCell> GetAdjacentCells(GameFieldGridCell cell, DirectionType direction)
        {
            var adjacentCells = new Queue<GameFieldGridCell>();

            var currentCell = cell;
            while (currentCell.AdjacentCells.TryGetValue(direction, out var adjacentCell))
            {
                adjacentCells.Enqueue(adjacentCell);
                currentCell = adjacentCell;
            }

            return adjacentCells;
        }
    }

}

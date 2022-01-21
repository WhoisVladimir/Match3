using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Gameplay
{
    public class GameFieldGrid : Singleton<GameFieldGrid>
    {
        public Dictionary<int, LinkedList<CellContentObject>> VerticalLinkedContentGrid { get; private set; }
        public Dictionary<int, LinkedList<CellContentObject>> HorizontalLinkedContentGrid { get; private set; }

        private GameplayController gameplay;
        private const int columnsCount = 5;
        private const int linesCount = 6;

        private GameFieldGridCell[,] grid;
        private List<CellContent> lvlContent;

        [SerializeField] private GameFieldGridCell cell;

        protected override void Awake()
        {
            base.Awake();
            VerticalLinkedContentGrid = new Dictionary<int, LinkedList<CellContentObject>>();
            HorizontalLinkedContentGrid = new Dictionary<int, LinkedList<CellContentObject>>();
            CreateGrid();
        }

        private void Start()
        {
            gameplay = GameplayController.Instance;
        }

        //private void OnEnable()
        //{
        //    GameFieldGridCell.CellEmptying += FindPlaceholder;
        //}

        //private void OnDisable()
        //{
        //    GameFieldGridCell.CellEmptying -= FindPlaceholder;
        //}

        //public void OnCellEmptying(GameFieldGridCell actingCell, CellContentObject contentObject)
        //{
        //    Debug.Log($"Сеть отреагировала на событие в [{actingCell.RowNumber}, {actingCell.LineNumber}]");
        //    var rowNumber = actingCell.RowNumber;
        //    VerticalLinkedContentGrid.TryGetValue(rowNumber, out var row);

        //    var ar = row.ToArray();
        //    Debug.Log($"Предыдущий список ряда ({row.Count}):");
        //    foreach (var item in ar)
        //    {
        //        Debug.Log(item.Content.ContentType);
        //    }

        //    var node = row.Find(contentObject);

        //    for (int i = actingCell.LineNumber; i < linesCount - 1; i++)
        //    {
        //        grid[rowNumber, i].FillCell(node.Next.Value);
        //    }
        //    row.Remove(node);

        //    grid[rowNumber, linesCount - 1].FillCell(contentObject, lvlContent);
        //    row.AddLast(contentObject);

        //    Debug.Log($"Текущий список ряда ({row.Count}):");
        //    ar = row.ToArray();
        //    foreach (var item in ar)
        //    {
        //        Debug.Log(item.Content.ContentType);
        //    }
        //}

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
                    grid[i, j].SetIndex(j, i);

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
                for (int j = 0; j < linesCount; j++)
                {
                    var abbreviatedList = new List<CellContent>();
                    CellContentObject contentObj = null;
                    if (j - 2 < 0 && i - 2 < 0) contentObj = SpawnManager.Instance.GetContentObject(content, contentObj);

                    if (j - 2 >= 0)
                    {
                        var previousContent = grid[i, j - 1].ContentObject.Content;
                        if (previousContent.ContentType == grid[i, j - 2].ContentObject.Content.ContentType)
                        {
                            abbreviatedList.AddRange(content);
                            abbreviatedList.Remove(previousContent);

                            contentObj = SpawnManager.Instance.GetContentObject(abbreviatedList, contentObj);
                        }
                        else contentObj = SpawnManager.Instance.GetContentObject(content, contentObj);
                    }

                    if (i - 2 >= 0)
                    {
                        var previousContent = grid[i - 2, j].ContentObject.Content;
                        if (previousContent.ContentType == (grid[i - 1, j].ContentObject.Content.ContentType))
                        {
                            if (abbreviatedList.Count == 0) abbreviatedList.AddRange(content);

                            abbreviatedList.Remove(previousContent);
                            contentObj = SpawnManager.Instance.GetContentObject(abbreviatedList, contentObj);
                        }
                    }

                    if(contentObj == null) contentObj = SpawnManager.Instance.GetContentObject(content, contentObj);

                    grid[i, j].FillCell(contentObj);
                }
            }
        }

        public void SwitchCellContent(GameFieldGridCell sourceCell, GameFieldGridCell targetCell)
        {
            if(targetCell.ContentObject == null)
            {
                var obj = sourceCell.ContentObject;
                grid[sourceCell.RowNumber, sourceCell.LineNumber].EmptyCell();
                grid[targetCell.RowNumber, targetCell.LineNumber].FillCell(obj);

            }
            else
            {
                var tempContentObj = targetCell.ContentObject;
                grid[targetCell.RowNumber, targetCell.LineNumber].FillCell(sourceCell.ContentObject);
                grid[sourceCell.RowNumber, sourceCell.LineNumber].FillCell(tempContentObj);
            }
        }

        public GameFieldGridCell FindTargetCell(DirectionType direction, GameFieldGridCell sourceCell)
        {
            GameFieldGridCell targetCell = null;
            switch (direction)
            {
                case DirectionType.LEFT:
                    int offset = sourceCell.RowNumber - 1;
                    targetCell = grid[offset, sourceCell.LineNumber];
                    break;
                case DirectionType.RIGHT:
                    offset = sourceCell.RowNumber + 1;
                    targetCell = grid[offset, sourceCell.LineNumber];
                    break;
                case DirectionType.TOP:
                    offset = sourceCell.LineNumber + 1;
                    targetCell = grid[sourceCell.RowNumber, offset];
                    break;
                case DirectionType.DOWN:
                    offset = sourceCell.LineNumber - 1;
                    targetCell = grid[sourceCell.RowNumber, offset];
                    break;
            }
            return targetCell;
        }

        public List<GameFieldGridCell> CheckMatch(GameFieldGridCell cell)
        {
            var verticalMatches = new List<GameFieldGridCell>();
            var horizontalMatches = new List<GameFieldGridCell>();

            for (int i = cell.LineNumber + 1; i < linesCount; i++)
            {
                if (grid[cell.RowNumber, i].ContentObject.Content.ContentType != cell.ContentObject.Content.ContentType) break;
                else verticalMatches.Add(grid[cell.RowNumber, i]);
            }

            for (int i = cell.LineNumber - 1; i >= 0; i--)
            {
                if (grid[cell.RowNumber, i].ContentObject.Content.ContentType != cell.ContentObject.Content.ContentType) break;
                else verticalMatches.Add(grid[cell.RowNumber, i]);
            }

            for (int i = cell.RowNumber - 1 ; i >= 0 ; i--)
            {
                if (grid[i, cell.LineNumber].ContentObject.Content.ContentType != cell.ContentObject.Content.ContentType) break;
                else horizontalMatches.Add(grid[i, cell.LineNumber]);
            }

            for (int i = cell.RowNumber + 1; i < columnsCount; i++)
            {
                if (grid[i, cell.LineNumber].ContentObject.Content.ContentType != cell.ContentObject.Content.ContentType) break;
                else horizontalMatches.Add(grid[i, cell.LineNumber]);
            }

            var allMatches = new List<GameFieldGridCell>();

            if (verticalMatches.Count >= 2) allMatches.AddRange(verticalMatches);
            if (horizontalMatches.Count >= 2) allMatches.AddRange(horizontalMatches);

            allMatches.Add(grid[cell.RowNumber, cell.LineNumber]);
            return allMatches;
        }

        public void FindPlaceholder(GameFieldGridCell cell)
        {
            GameFieldGridCell filledCell = null;
            for (int i = cell.LineNumber + 1; i < linesCount; i++)
            {
                var currentGrid = grid[cell.RowNumber, i];
                if (!currentGrid.IsEmpty)
                {
                    filledCell = currentGrid;
                    break;
                }
            }

            //if (filledCell == null) 
            //{
            //    CellContentObject obj = null;
            //    obj = SpawnManager.Instance.GetContentObject(lvlContent, obj);
            //    filledCell = grid[columnsCount - 1, linesCount - 1];
            //    filledCell.FillCell(obj);
            //}  

            if(filledCell != null)gameplay.MoveCellContent(filledCell, gameplay.SpawnDirection, false);
        }
    }
}

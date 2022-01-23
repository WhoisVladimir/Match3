using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System.Linq;
using System;

namespace Gameplay
{
    public class GameFieldGrid : Singleton<GameFieldGrid>
    {
        private GameplayController gameplay;
        private const int columnsCount = 5;
        private const int linesCount = 6;

        private GameFieldGridCell[,] grid;
        private List<CellContent> lvlContent;

        [SerializeField] private GameFieldGridCell cell;

        protected override void Awake()
        {
            base.Awake();

            CreateGrid();
        }

        private void Start()
        {
            gameplay = GameplayController.Instance;
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
                    grid[i, j].SetIndex(j, i);
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
                    if (j - 2 < 0 && i - 2 < 0) contentObj = SpawnManager.Instance.GetContentObject(content);

                    if (j - 2 >= 0)
                    {
                        var previousContent = grid[i, j - 1].ContentObject.Content;
                        if (previousContent.ContentType == grid[i, j - 2].ContentObject.Content.ContentType)
                        {
                            abbreviatedList.AddRange(content);
                            abbreviatedList.Remove(previousContent);

                            contentObj = SpawnManager.Instance.GetContentObject(abbreviatedList);
                        }
                        else contentObj = SpawnManager.Instance.GetContentObject(content);
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

                    if(contentObj == null) contentObj = SpawnManager.Instance.GetContentObject(content);

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
                    if(offset >= 0) targetCell = grid[offset, sourceCell.LineNumber];
                    break;
                case DirectionType.RIGHT:
                    offset = sourceCell.RowNumber + 1;
                    if(offset < columnsCount) targetCell = grid[offset, sourceCell.LineNumber];
                    break;
                case DirectionType.TOP:
                    offset = sourceCell.LineNumber + 1;
                    if(offset < linesCount) targetCell = grid[sourceCell.RowNumber, offset];
                    break;
                case DirectionType.DOWN:
                    offset = sourceCell.LineNumber - 1;
                    if(offset >= 0) targetCell = grid[sourceCell.RowNumber, offset];
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
                if (grid[cell.RowNumber, i].ContentObject == null) break;
                else if (grid[cell.RowNumber, i].ContentObject.Content.ContentType != cell.ContentObject.Content.ContentType) break;
                else verticalMatches.Add(grid[cell.RowNumber, i]);
            }

            for (int i = cell.LineNumber - 1; i >= 0; i--)
            {
                if (grid[cell.RowNumber, i].ContentObject == null) break;
                else if (grid[cell.RowNumber, i].ContentObject.Content.ContentType != cell.ContentObject.Content.ContentType) break;
                else verticalMatches.Add(grid[cell.RowNumber, i]);
            }

            for (int i = cell.RowNumber - 1 ; i >= 0 ; i--)
            {
                if (grid[i, cell.LineNumber].ContentObject == null) break;
                else if (grid[i, cell.LineNumber].ContentObject.Content.ContentType != cell.ContentObject.Content.ContentType) break;
                else horizontalMatches.Add(grid[i, cell.LineNumber]);
            }

            for (int i = cell.RowNumber + 1; i < columnsCount; i++)
            {
                if (grid[i, cell.LineNumber].ContentObject == null) break;
                else if (grid[i, cell.LineNumber].ContentObject.Content.ContentType != cell.ContentObject.Content.ContentType) break;
                else horizontalMatches.Add(grid[i, cell.LineNumber]);
            }

            var allMatches = new List<GameFieldGridCell>();

            if (verticalMatches.Count >= 2) allMatches.AddRange(verticalMatches);
            if (horizontalMatches.Count >= 2) allMatches.AddRange(horizontalMatches);

            allMatches.Add(grid[cell.RowNumber, cell.LineNumber]);
            return allMatches;
        }

        public void OnCellEmptying(GameFieldGridCell actingCell)
        {
            StartCoroutine(FindPlaceholder(actingCell));
        }

        public IEnumerator FindPlaceholder(GameFieldGridCell cell)
        {
            
            GameFieldGridCell filledCell = null;
            for (int i = cell.LineNumber + 1; i < linesCount; i++)
            {
                var currentGrid = grid[cell.RowNumber, i];
                if (currentGrid.IsEmpty == false)
                {
                    filledCell = currentGrid;
                    break;
                }
            }

            if (filledCell == null)
            {
                yield return new WaitForEndOfFrame();
                CellContentObject obj = null;
                obj = SpawnManager.Instance.GetContentObject(lvlContent);
                filledCell = grid[cell.RowNumber, linesCount - 1];
                if(grid[cell.RowNumber, linesCount -1].IsEmpty) filledCell.FillCell(obj);
            }

            gameplay.MoveCellContent(filledCell, gameplay.SpawnDirection, false);

            yield return null;
        }

        public IEnumerator HandleAdjacentCells(GameFieldGridCell sourceCell)
        {
            var adjacentCells = new List<GameFieldGridCell>();
            foreach (var item in Enum.GetValues(typeof(DirectionType)))
            {
                var direction = (DirectionType)item;
                var adjCell = FindTargetCell(direction, sourceCell);
                if (adjCell != null) adjacentCells.Add(adjCell);
            }
            foreach (var item in adjacentCells)
            {
                if (item.IsEmpty)
                {
                    yield return new WaitWhile(() => item.IsEmpty == true);
                }
            }
            if (!sourceCell.IsEmpty)
            {
                var matchCells = CheckMatch(sourceCell);
                gameplay.MatchesHandling(matchCells);
            }
        }
    }
}

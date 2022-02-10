using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System;
using Utils;

namespace Gameplay
{
    public class GameFieldGrid : Singleton<GameFieldGrid>
    {
        public int EmptyCellsCount { get; private set; } = 0;

        [SerializeField] private GameFieldGridCell cell;

        private GameplayController gameplay;
        private SpawnManager spawner;
       
        private const int columnsCount = 5;
        private const int linesCount = 6;
        private GameFieldGridCell[,] grid;

        private List<CellContent> lvlContent;

        protected override void Awake()
        {
            base.Awake();

            CreateGrid();
        }

        private void Start()
        {
            gameplay = GameplayController.Instance;
            spawner = SpawnManager.Instance;
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
                    grid[i, j].SetIndex(i, j);
                }
            }
        }

        /// <summary>
        /// Логика заполнения сетки контентом. Производится проверка предыдущих ячеек на совпадение содержимого.
        /// </summary>
        /// <param name="content">Список возможного содержимого.</param>
        public void FillGrid(List<CellContent> content)
        {
            lvlContent = content;
            for (int i = 0; i < columnsCount; i++)
            {
                for (int j = 0; j < linesCount; j++)
                {
                    var abbreviatedList = new List<CellContent>();
                    CellContentObject contentObj = null;
                    if (j - 2 < 0 && i - 2 < 0) contentObj = spawner.GetContentObject(content);

                    if (j - 2 >= 0)
                    {
                        var previousContent = grid[i, j - 1].ContentObject.Content;
                        if (previousContent.ContentType == grid[i, j - 2].ContentObject.Content.ContentType)
                        {
                            abbreviatedList.AddRange(content);
                            abbreviatedList.Remove(previousContent);

                            contentObj = spawner.GetContentObject(abbreviatedList);
                        }
                        else contentObj = spawner.GetContentObject(content);
                    }

                    if (i - 2 >= 0)
                    {
                        var previousContent = grid[i - 2, j].ContentObject.Content;
                        if (previousContent.ContentType == (grid[i - 1, j].ContentObject.Content.ContentType))
                        {
                            if (abbreviatedList.Count == 0) abbreviatedList.AddRange(content);

                            abbreviatedList.Remove(previousContent);
                            contentObj = spawner.GetContentObject(abbreviatedList, contentObj);
                        }
                    }

                    if(contentObj == null) contentObj = spawner.GetContentObject(content);

                    var startContentCommand = new SetStartContentCommand(grid[i, j], contentObj);
                    if (i == columnsCount - 1 && j == linesCount - 1) Invoker.AddEndedCommand(startContentCommand);
                    else Invoker.AddCommand(startContentCommand);
                }
            }
        }

        /// <summary>
        /// Перемещает объект из заполненной ячейки в пустую, а также меняет объекты местами.
        /// </summary>
        /// <param name="sourceCell">Ячейка-источник движения</param>
        /// <param name="targetCell">Ячейка по направлению движения.</param>
        public void SwitchCellContent(GameFieldGridCell sourceCell, GameFieldGridCell targetCell)
        {
            if(targetCell.ContentObject == null)
            {
                var obj = sourceCell.ContentObject;
                //grid[sourceCell.RowNumber, Math.Abs(sourceCell.LineNumber)].EmptyCell();
                var fillCell = new FillCellCommand(targetCell, sourceCell.ContentObject);
                Invoker.AddCommand(fillCell);
            }
            else
            {
                Debug.Log("Cмена ячеек");
                var tempContentObj = targetCell.ContentObject;
                var fillCell = new FillCellCommand(grid[targetCell.RowNumber, Math.Abs(targetCell.LineNumber)], sourceCell.ContentObject);
                var secondFillCell = new FillCellCommand(grid[sourceCell.RowNumber, Math.Abs(sourceCell.LineNumber)], tempContentObj);

                Invoker.AddCommand(fillCell);
                Invoker.AddCommand(secondFillCell);
            }
        }

        /// <summary>
        /// Ищет ячейку в заданном направлении.
        /// </summary>
        /// <param name="direction">Направление движения.</param>
        /// <param name="sourceCell">Ячейка-источник движения.</param>
        /// <returns></returns>
        public GameFieldGridCell FindTargetCell(DirectionType direction, GameFieldGridCell sourceCell)
        {
            GameFieldGridCell targetCell = null;
            switch (direction)
            {
                case DirectionType.LEFT:
                    int offset = sourceCell.RowNumber - 1;
                    if(offset >= 0) targetCell = grid[offset, Math.Abs(sourceCell.LineNumber)];
                    break;
                case DirectionType.RIGHT:
                    offset = sourceCell.RowNumber + 1;
                    if(offset < columnsCount) targetCell = grid[offset, Math.Abs(sourceCell.LineNumber)];
                    break;
                case DirectionType.TOP:
                    offset = Math.Abs(sourceCell.LineNumber) + 1;
                    if(offset < linesCount) targetCell = grid[sourceCell.RowNumber, offset];
                    break;
                case DirectionType.DOWN:
                    offset = Math.Abs(sourceCell.LineNumber) - 1;
                    if(offset >= 0) targetCell = grid[sourceCell.RowNumber, offset];
                    break;
            }
            return targetCell;
        }

        /// <summary>
        /// Производит проверку контента соседних ячеек.
        /// </summary>
        /// <param name="cell">Ячейка-источник.</param>
        /// <returns></returns>
        public List<GameFieldGridCell> CheckMatch(GameFieldGridCell cell)
        {
            var verticalMatches = new List<GameFieldGridCell>();
            var horizontalMatches = new List<GameFieldGridCell>();

            for (int i = Math.Abs(cell.LineNumber) + 1; i < linesCount; i++)
            {
                if (grid[cell.RowNumber, i].ContentObject == null) break;
                else if (grid[cell.RowNumber, i].ContentObject.Content.ContentType != cell.ContentObject.Content.ContentType) break;
                else verticalMatches.Add(grid[cell.RowNumber, i]);
            }

            for (int i = Math.Abs(cell.LineNumber) - 1; i >= 0 ; i--)
            {
                if (grid[cell.RowNumber, i].ContentObject == null) break;
                else if (grid[cell.RowNumber, i].ContentObject.Content.ContentType != cell.ContentObject.Content.ContentType) break;
                else verticalMatches.Add(grid[cell.RowNumber, i]);
            }

            for (int i = cell.RowNumber - 1 ; i >= 0 ; i--)
            {
                var line = Math.Abs(cell.LineNumber);
                if (grid[i, line].ContentObject == null) break;
                else if (grid[i, line].ContentObject.Content.ContentType != cell.ContentObject.Content.ContentType) break;
                else horizontalMatches.Add(grid[i, line]);
            }

            for (int i = cell.RowNumber + 1; i < columnsCount; i++)
            {
                var line = Math.Abs(cell.LineNumber);
                if (grid[i, line].ContentObject == null) break;
                else if (grid[i, line].ContentObject.Content.ContentType != cell.ContentObject.Content.ContentType) break;
                else horizontalMatches.Add(grid[i, line]);
            }

            var allMatches = new List<GameFieldGridCell>();

            if (verticalMatches.Count >= 2) allMatches.AddRange(verticalMatches);
            if (horizontalMatches.Count >= 2) allMatches.AddRange(horizontalMatches);

            allMatches.Add(grid[cell.RowNumber, Math.Abs(cell.LineNumber)]);
            return allMatches;
        }


        /// <summary>
        /// Сопрограмма ожидания заполнения соседних ячеек для последующей проверки на совпадение контента.
        /// </summary>
        /// <param name="sourceCell">Ячейка-источник.</param>
        /// <returns></returns>
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
            //if (!sourceCell.IsEmpty)
            //{
            //    var matchCells = CheckMatch(sourceCell);
            //    gameplay.MatchesHandling(matchCells);
            //}
        }

        public void MoveCells(GameFieldGridCell initCell, int stepsCount)
        {
            int bound = GetGridBound();

            var filledCells = new LinkedList<GameFieldGridCell>();
            for (int i = initCell.LineNumber + 1; i < bound; i++)
            {
                var currentGrid = grid[initCell.RowNumber, Math.Abs(i)];

                if (!currentGrid.IsEmpty)
                {
                    filledCells.AddLast(currentGrid);
                }
            }

            var respawnerCell = grid[initCell.RowNumber, bound - 1];

            if (filledCells.Count == 0)
            {
                Respawn(respawnerCell);
                filledCells.AddLast(respawnerCell);
                stepsCount--;
                if (initCell == respawnerCell) return;
            }

            var refillCommand = new FillRowCommand(filledCells, respawnerCell, initCell, stepsCount);
            Invoker.AddCommand(refillCommand);

            //for (int i = 0; i < stepsCount; i++)
            //{
            //    var refillCommand = new FillRowCommand(filledCells, respawnerCell);
            //    Invoker.AddCommand(refillCommand);
            //}
        }

        public void Respawn(GameFieldGridCell respawnerCell)
        {
            var contentObj = spawner.GetContentObject(lvlContent);
            var respawnCommand = new SetStartContentCommand(respawnerCell, contentObj);
            Invoker.AddCommand(respawnCommand);
            EmptyCellsCount--;
            Debug.Log($"{EmptyCellsCount} пустых ячеек.");
            if (EmptyCellsCount == 0) Debug.Log("Сетка заполнена!");
        }

        public GameFieldGridCell GetNextCell(GameFieldGridCell currentCell)
        {
            int bound = GetGridBound();
            if (currentCell.LineNumber == bound - linesCount ) return null;

            var nextCell = grid[currentCell.RowNumber, currentCell.LineNumber - 1];
            return nextCell;
        }
        public GameFieldGridCell GetPreviousCell(GameFieldGridCell currentCell)
        {
            int bound = GetGridBound();
            if (currentCell.LineNumber == bound) return null;

            var prevCell = grid[currentCell.RowNumber, currentCell.LineNumber + 1];
            return prevCell;
        }

        private int GetGridBound()
        {
            int bound = 0;
            switch (gameplay.SpawnDirection)
            {
                case DirectionType.TOP:
                    bound = 1;
                    break;
                case DirectionType.DOWN:
                    bound = linesCount;
                    break;
            }
            return bound;
        }

        public void AddEmptyCells(int count)
        {
            EmptyCellsCount += count;
        }
    }
}

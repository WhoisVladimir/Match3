using System.Collections.Generic;
using UnityEngine;

namespace Gameplay
{
    public class GameFieldGrid : Singleton<GameFieldGrid>
    {
        private const int columnsCount = 5;
        private const int linesCount = 6;

        private GameFieldGridCell[,] grid;

        [SerializeField] private GameFieldGridCell cell;

        protected override void Awake()
        {
            base.Awake();
            CreateGrid();
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
            for (int i = 0; i < columnsCount; i++)
            {
                for (int j = 0; j < linesCount; j++)
                {
                    List<CellContent> abbreviatedList = new List<CellContent>();

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
                }
            }
        }
    }

}

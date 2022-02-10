using System.Collections.Generic;
using Utils;
using System.Linq;
using UnityEngine;

namespace Gameplay
{
    public class EmptyCellsCommand : ICommand
    {
        private GameFieldGrid grid;
        private List<GameFieldGridCell> cells;
        public EmptyCellsCommand(List<GameFieldGridCell> cells)
        {
            grid = GameFieldGrid.Instance;
            this.cells = cells;
        }

        public void Execute()
        {
            grid.AddEmptyCells(cells.Count);

            var notifiers = new List<GameFieldGridCell>();

            var sortedCells = cells.OrderBy(x => x.LineNumber);
            foreach (var cell in sortedCells)
            {
                Debug.Log($"{cell.RowNumber}, {cell.LineNumber}");
                cell.EmptyCell();
                if (!notifiers.Exists(c => c.RowNumber == cell.RowNumber)) notifiers.Add(cell);
            }

            foreach (var cell in notifiers)
            {
                Debug.Log($"{cell.RowNumber} {cell.LineNumber}");

                var rowLength = cells.Where(x => x.RowNumber == cell.RowNumber).Count();
                grid.MoveCells(cell, rowLength);
            }
        }
    }
}

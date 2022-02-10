using Utils;
using System.Collections.Generic;

namespace Gameplay
{
    public class CheckMatchCommand : ICommand
    {
        private GameFieldGrid grid;
        private GameFieldGridCell cell;
        public CheckMatchCommand(GameFieldGridCell checkableCell)
        {
            grid = GameFieldGrid.Instance;
            cell = checkableCell;
        }

        public void Execute()
        {
            var matches = grid.CheckMatch(cell);
        }
    }
}

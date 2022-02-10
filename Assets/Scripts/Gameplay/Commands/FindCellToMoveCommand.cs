using Utils;

namespace Gameplay
{
    public class FindCellToMoveCommand : ICommand
    {
        private GameFieldGrid grid;
        private DirectionType direction;
        private GameFieldGridCell sourceCell;

        public FindCellToMoveCommand(DirectionType direction, GameFieldGridCell sourceCell)
        {
            grid = GameFieldGrid.Instance;
            this.direction = direction;
            this.sourceCell = sourceCell;
        }

        public void Execute()
        {
            var targetCell = grid.FindTargetCell(direction, sourceCell);
            
            var switchCommand = new SwitchContentCommand(sourceCell, targetCell);
            Invoker.AddCommand(switchCommand);
        }
    }
}

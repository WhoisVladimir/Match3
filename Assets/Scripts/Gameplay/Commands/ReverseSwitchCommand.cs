using Utils;

namespace Gameplay
{
    public class ReverseSwitchCommand : ICommand
    {
        private GameFieldGrid grid;
        private GameFieldGridCell sourceCell;
        private GameFieldGridCell targetCell;
        public ReverseSwitchCommand(GameFieldGridCell sourceCell, GameFieldGridCell targetCell)
        {
            grid = GameFieldGrid.Instance;
            this.sourceCell = sourceCell;
            this.targetCell = targetCell;
        }

        public void Execute()
        {
            grid.SwitchCellContent(targetCell, sourceCell);
        }
    }

}

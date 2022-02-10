using Utils;

namespace Gameplay
{
    public class MoveItemCommand : ICommand
    {
        private GameplayController gameplay;
        private GameFieldGridCell cell;
        private DirectionType direction;
        private bool isIntentialAction;

        public MoveItemCommand(GameFieldGridCell cell, DirectionType direction, bool isIntentialAction)
        {
            gameplay = GameplayController.Instance;
            this.cell = cell;
            this.direction = direction;
            this.isIntentialAction = isIntentialAction;
        }

        public void Execute()
        {
            gameplay.MoveCellContent(cell, direction, isIntentialAction);
        }
    }
}

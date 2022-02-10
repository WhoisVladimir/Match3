using Utils;

namespace Gameplay
{
    public class SpecializeItemCommand : ICommand
    {
        private GameFieldGridCell cell;
        public SpecializeItemCommand(GameFieldGridCell cell)
        {
            this.cell = cell;
        }
        public void Execute()
        {
            cell.ContentObject.SetSpecial();
        }

    }

}

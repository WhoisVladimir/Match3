using Utils;

namespace Gameplay
{
    public class ChangePositionCommand : ICommand
    {
        private CellContentObject reciever;
        private GameFieldGridCell cell;

        public ChangePositionCommand(CellContentObject reciever, GameFieldGridCell cell)
        {
            this.reciever = reciever;
            this.cell = cell;
        }

        public void Execute()
        {
            reciever.ChangePosition(cell);
        }
    }
}

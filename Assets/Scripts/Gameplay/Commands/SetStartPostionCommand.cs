using Utils;

namespace Gameplay
{
    public class SetStartPostionCommand : ICommand
    {
        private CellContentObject receiver;
        private GameFieldGridCell cell;

        public SetStartPostionCommand(CellContentObject receiver, GameFieldGridCell cell)
        {
            this.receiver = receiver;
            this.cell = cell;
        }
        public void Execute()
        {
            receiver.SetStartPosition(cell);
        }
    }
}

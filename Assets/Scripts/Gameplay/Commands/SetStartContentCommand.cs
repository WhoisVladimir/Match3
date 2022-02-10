using Utils;

namespace Gameplay
{
    public class SetStartContentCommand : ICommand
    {
        private GameFieldGridCell receiver;
        private CellContentObject contentObject;
        public SetStartContentCommand(GameFieldGridCell receiver, CellContentObject contentObject)
        {
            this.receiver = receiver;
            this.contentObject = contentObject;
        }
        public void Execute()
        {
            receiver.SetStartContent(contentObject);
        }
    }

}

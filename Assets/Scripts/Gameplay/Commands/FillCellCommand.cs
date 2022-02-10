using Utils;

namespace Gameplay
{
    public class FillCellCommand : ICommand
    {
        GameFieldGridCell cell;
        CellContentObject contentObject;

        public FillCellCommand(GameFieldGridCell cell, CellContentObject contentObject)
        {
            this.cell = cell;
            this.contentObject = contentObject;
        }

        public void Execute()
        {
            cell.FillCell(contentObject);
        }
    }

}

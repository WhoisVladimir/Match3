using System.Collections.Generic;
using Utils;

namespace Gameplay
{
    public class FillGridCommand : ICommand
    {
        GameFieldGrid grid;
        List<CellContent> content;
        public FillGridCommand(List<CellContent> contentItems)
        {
            grid = GameFieldGrid.Instance;
            content = contentItems;
        }
        public void Execute()
        {
            grid.FillGrid(content);
        }
    }
}
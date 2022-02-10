using UnityEngine;
using Utils;

namespace Gameplay
{
    public class SwitchContentCommand : ICallbackableCommand
    {
        private GameFieldGrid grid;
        private GameplayController gameplay;
        private GameFieldGridCell sourceCell;
        private GameFieldGridCell targetCell;
        private int endedOperations = 0;

        public SwitchContentCommand(GameFieldGridCell sourceCell, GameFieldGridCell targetCell)
        {
            grid = GameFieldGrid.Instance;
            gameplay = GameplayController.Instance;
            this.sourceCell = sourceCell;
            this.targetCell = targetCell;
        }

        public void Execute()
        {

            sourceCell.ContentObject.Callback += OnCallbackAction;
            targetCell.ContentObject.Callback += OnCallbackAction;

            grid.SwitchCellContent(sourceCell, targetCell);
        }

        public void OnCallbackAction(ICallbacker callbacker)
        {
            endedOperations++;
            if(endedOperations == 2)
            {

                sourceCell.ContentObject.Callback -= OnCallbackAction;
                targetCell.ContentObject.Callback -= OnCallbackAction;

                gameplay.HandleSwitch(sourceCell, targetCell);
            }
        }
    }

}

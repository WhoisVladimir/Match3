using System.Collections.Generic;
using UnityEngine;
using Utils; 

namespace Gameplay
{
    public class FillRowCommand : ICallbackableCommand
    {
        private GameFieldGrid grid;
        private LinkedList<GameFieldGridCell> row;
        private GameFieldGridCell respawnerCell;
        GameFieldGridCell initCell;
        private int stepsCount;

        public FillRowCommand(LinkedList<GameFieldGridCell> row, GameFieldGridCell respawnerCell, GameFieldGridCell initCell, int stepsCount)
        {
            grid = GameFieldGrid.Instance;
            this.row = row;
            this.respawnerCell = respawnerCell;
            this.initCell = initCell;
            this.stepsCount = stepsCount;

            row.Last.Value.ContentObject.Callback += OnCallbackAction;
        }
        public void Execute()
        {
            if (stepsCount == 0) return;
            LinkedListNode<GameFieldGridCell> currentNode = row.First;
            for (int i = 0; i < row.Count; i++)
            {
                //if( currentNode == null) currentNode = row.First;
                //else if (i > 0) currentNode = currentNode.Next;


                //try
                //{
                //    if (currentNode.Value != initCell)
                //    {
                //        var fillableCell = grid.GetNextCell(currentNode.Value);

                //        var filledCommand = new FillCellCommand(fillableCell, currentNode.Value.ContentObject);
                //        Invoker.AddCommand(filledCommand);

                //        currentNode.Value = fillableCell;
                //    }

                //}
                //catch (System.Exception e)
                //{

                //    throw;
                //}
                if( currentNode.Value == initCell)
                {
                    var previousCell = grid.GetPreviousCell(currentNode.Value);
                    if (previousCell != null)
                    {
                        initCell = previousCell;

                    }
                    var previousNode = currentNode;
                    currentNode = currentNode.Next;
                    row.Remove(previousNode);

                }
                 var fillableCell = grid.GetNextCell(currentNode.Value);

                var filledCommand = new FillCellCommand(fillableCell, currentNode.Value.ContentObject);
                Invoker.AddCommand(filledCommand);

                currentNode.Value = fillableCell;
                if(row.Count > 1) currentNode = currentNode.Next;


                //if (currentNode.Value != initCell)
                //{
                //}
            }
        }

        public void OnCallbackAction(ICallbacker callbacker)
        {
            if (stepsCount > 0)
            {
                Debug.Log(stepsCount);
                grid.Respawn(respawnerCell);

                row.Last.Value.ContentObject.Callback -= OnCallbackAction;
                row.AddLast(respawnerCell);
                row.Last.Value.ContentObject.Callback += OnCallbackAction;

                stepsCount--;
                Execute();
            }
            else row.Last.Value.ContentObject.Callback -= OnCallbackAction;

        }
    }
}

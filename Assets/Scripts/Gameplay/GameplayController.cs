using System.Collections.Generic;
using UnityEngine;
using Utils;

namespace Gameplay
{
    public class GameplayController : Singleton<GameplayController>
    {
        [SerializeField] private List<GameLevel> gameLevels;

        private GameLevel currentLevel;
        private int boundIndex = 100;
        private List<CellContent> lvlContentItems;
        private GameFieldGrid grid;
        
        public DirectionType SpawnDirection = DirectionType.DOWN;

        protected override void Awake()
        {
            base.Awake();
            currentLevel = SetLevel(1);
        }

        private void Start()
        {
            lvlContentItems = currentLevel.ContentItems;

            grid = GameFieldGrid.Instance;
            var fillCommand = new FillGridCommand(lvlContentItems);
            Invoker.StartCommand(fillCommand);
        }

        //private void OnEnable()
        //{
        //    GameFieldGridCell.SwitchSpawnDirection += OnSwitchNotifiсation;
        //}
        //private void OnDisable()
        //{
        //    GameFieldGridCell.SwitchSpawnDirection -= OnSwitchNotifiсation;
        //}

        /// <summary>
        /// Назначает уровень.
        /// </summary>
        /// <param name="index">Порядковый номер уровня.</param>
        /// <returns></returns>
        private GameLevel SetLevel(int index)
        {
            if (index == boundIndex) return null;

            var level = gameLevels.Find(lvl => lvl.LvlIndexNumber == index);

            if (level is null)
            {
                Debug.Log($"Level {index} not found");
                SetLevel(++index);
            }
            return level;
        }

        /// <summary>
        /// Перемещение объекта содержимого в ячейку
        /// </summary>
        /// <param name="cell">Ячейка из которой перемещается объект</param>
        /// <param name="direction">Направление в котором перемещается объект</param>
        /// <param name="isIntentialAction">Является ли перемещение следствием ввода игрока</param>
        public void MoveCellContent(GameFieldGridCell cell, DirectionType direction, bool isIntentialAction)
        {

            if (isIntentialAction) 
            {
                var findSwitchCommand = new FindCellToMoveCommand(direction, cell);
                Invoker.AddCommand(findSwitchCommand);
            }
            else
            {
                var targetCell = grid.FindTargetCell(direction, cell);

                while (targetCell != null && targetCell.IsEmpty == true)
                {
                    if (cell.IsEmpty) return;
                    grid.SwitchCellContent(cell, targetCell);
                    cell = targetCell;
                    targetCell = grid.FindTargetCell(direction, targetCell);
                }

                StartCoroutine(grid.HandleAdjacentCells(cell));
            }
        }

        public void HandleSwitch(GameFieldGridCell sourceCell, GameFieldGridCell targetCell)
        {
            var targetMatches = grid.CheckMatch(targetCell);
            var sourceMatches = grid.CheckMatch(sourceCell);

            var resultMatches = new List<GameFieldGridCell>();

            if (targetMatches.Count < 3 && sourceMatches.Count < 3)
            {
                var switchCommand = new ReverseSwitchCommand(sourceCell, targetCell);
                Invoker.AddEndedCommand(switchCommand);
            }
            else
            {
                resultMatches.AddRange(MatchesHandling(targetMatches, targetCell));
                resultMatches.AddRange(MatchesHandling(sourceMatches, sourceCell));

                var emptyingCommand = new EmptyCellsCommand(resultMatches);
                Invoker.AddCommand(emptyingCommand);
            }

        }

        /// <summary>
        /// Обрабатывает найденные совпадения.
        /// </summary>
        /// <param name="matches">Список совпадений.</param>
        public List<GameFieldGridCell> MatchesHandling(List<GameFieldGridCell> matches, GameFieldGridCell initialCell)
        {
            if (matches.Count < 3) matches.Clear();
            if (matches.Count > 3) 
            {
                var index = matches.IndexOf(initialCell);
                var unspecialCell = matches[index];

                var specializeCommand = new SpecializeItemCommand(unspecialCell);
                Invoker.AddCommand(specializeCommand);
                matches.Remove(unspecialCell);
            }
            return matches;
        }

        /// <summary>
        /// Меняет направление спавна.
        /// </summary>
        private void OnSwitchNotifiсation()
        {
            switch (SpawnDirection)
            {
                case DirectionType.TOP:
                    SpawnDirection = DirectionType.DOWN;
                    break;
                case DirectionType.DOWN:
                    SpawnDirection = DirectionType.TOP;
                    break;
            }
        }
    }
}

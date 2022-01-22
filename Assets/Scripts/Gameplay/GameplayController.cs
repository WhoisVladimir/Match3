using System.Collections.Generic;
using System.Collections;
using UnityEngine;

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
            grid.FillGrid(lvlContentItems);
        }

        /// <summary>
        /// Назначает уровень.
        /// </summary>
        /// <param name="index">Порядковый номер уровня.</param>
        /// <returns></returns>
        private GameLevel SetLevel(int index)
        {
            //реализовать логику завершающую игру
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
            Debug.Log("Обработка движения");

            if (isIntentialAction) 
            {
                var targetCell = grid.FindTargetCell(direction, cell);
                grid.SwitchCellContent(cell, targetCell);
                var targetMatches = grid.CheckMatch(targetCell);
                var sourceMatches = grid.CheckMatch(cell);

                if (targetMatches.Count < 3 && sourceMatches.Count < 3) grid.SwitchCellContent(targetCell, cell);
                else
                {
                    MatchesHandling(targetMatches);
                    MatchesHandling(sourceMatches);
                }
            }
            else
            {
                var targetCell = grid.FindTargetCell(direction, cell);

                while (targetCell.IsEmpty)
                {
                    grid.SwitchCellContent(cell, targetCell);
                    cell = targetCell;
                    targetCell = grid.FindTargetCell(direction, targetCell);
                }
                //var targetMatches = grid.CheckMatch(cell);
                //MatchesHandling(targetMatches);
            }
        }

        public void MatchesHandling(List<GameFieldGridCell> matches)
        {

            if (matches.Count < 3) return;

            foreach (var item in matches)
            {
                item.EmptyCell();
            }
        }
    }
}

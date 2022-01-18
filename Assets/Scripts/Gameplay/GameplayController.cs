using System.Collections.Generic;
using UnityEngine;

namespace Gameplay
{
    public class GameplayController : Singleton<GameplayController>
    {
        [SerializeField] private List<GameLevel> gameLevels;

        private GameLevel currentLevel;
        private int boundIndex = 100;
        private List<CellContent> lvlContentItems;

        protected override void Awake()
        {
            base.Awake();
            currentLevel = SetLevel(1);
        }

        private void Start()
        {
            lvlContentItems = currentLevel.ContentItems;
            GameFieldGrid.Instance.FillGrid(lvlContentItems);
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
            var contentObj = cell.ContentObject;
            if (!cell.AdjacentCells.TryGetValue(direction, out var targetCell)) Debug.Log("Bound cell");
            if (targetCell.IsEmpty) targetCell.FillCell(contentObj);
            else
            {
                Debug.Log($"Движение: {direction} {cell.ContentObject.Content.ContentType}");
                if (isIntentialAction) SwitchCellContent(cell, targetCell);
            }
        }

        /// <summary>
        /// Замена объекта содержимого.
        /// </summary>
        /// <param name="cell">Ячейка из котрой производится замена</param>
        /// <param name="targetCell">Ячейка в которую производится замена</param>
        public void SwitchCellContent(GameFieldGridCell cell, GameFieldGridCell targetCell)
        {
            var tempContentObj = targetCell.ContentObject;
            Debug.Log($"Произошла замена на {tempContentObj.Content.ContentType}");
            targetCell.FillCell(cell.ContentObject);
            cell.FillCell(tempContentObj);
            Debug.Log($"Целевая ячейка: {targetCell.ContentObject.Content.ContentType}");
            Debug.Log($"Ячейка источник: {cell.ContentObject.Content.ContentType}");

            targetCell.CheckAdjacentCells();
            cell.CheckAdjacentCells();
        }
    }
}

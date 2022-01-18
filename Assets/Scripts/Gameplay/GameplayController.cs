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
        /// ��������� �������.
        /// </summary>
        /// <param name="index">���������� ����� ������.</param>
        /// <returns></returns>
        private GameLevel SetLevel(int index)
        {
            //����������� ������ ����������� ����
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
        /// ����������� ������� ����������� � ������
        /// </summary>
        /// <param name="cell">������ �� ������� ������������ ������</param>
        /// <param name="direction">����������� � ������� ������������ ������</param>
        /// <param name="isIntentialAction">�������� �� ����������� ���������� ����� ������</param>
        public void MoveCellContent(GameFieldGridCell cell, DirectionType direction, bool isIntentialAction)
        {
            var contentObj = cell.ContentObject;
            if (!cell.AdjacentCells.TryGetValue(direction, out var targetCell)) Debug.Log("Bound cell");
            if (targetCell.IsEmpty) targetCell.FillCell(contentObj);
            else
            {
                Debug.Log($"��������: {direction} {cell.ContentObject.Content.ContentType}");
                if (isIntentialAction) SwitchCellContent(cell, targetCell);
            }
        }

        /// <summary>
        /// ������ ������� �����������.
        /// </summary>
        /// <param name="cell">������ �� ������ ������������ ������</param>
        /// <param name="targetCell">������ � ������� ������������ ������</param>
        public void SwitchCellContent(GameFieldGridCell cell, GameFieldGridCell targetCell)
        {
            var tempContentObj = targetCell.ContentObject;
            Debug.Log($"��������� ������ �� {tempContentObj.Content.ContentType}");
            targetCell.FillCell(cell.ContentObject);
            cell.FillCell(tempContentObj);
            Debug.Log($"������� ������: {targetCell.ContentObject.Content.ContentType}");
            Debug.Log($"������ ��������: {cell.ContentObject.Content.ContentType}");

            targetCell.CheckAdjacentCells();
            cell.CheckAdjacentCells();
        }
    }
}

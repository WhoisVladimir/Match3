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

    }

}

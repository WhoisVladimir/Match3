using System.Collections.Generic;
using UnityEngine;

namespace Gameplay
{
    public class GameplayController : Singleton<GameplayController>
    {
        [SerializeField] private List<GameLevel> gameLevels;
        [SerializeField] private GameObject cellContentObject;

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
        /// Логика заполнения 
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="contentList">Список доступного неповторяющегося контента</param>
        public void FillCell(GameFieldGridCell cell, List<CellContent> contentList = null)
        {
            if (contentList == null) contentList = lvlContentItems;

            var cellTf = cell.transform;
            CellContentObject contentObject;

            if (cell.IsEmpty)
            {
                contentObject =
                    Instantiate(cellContentObject, cellTf.position, cellTf.rotation, GameFieldGrid.Instance.transform)
                    .GetComponent<CellContentObject>();
            }

            else contentObject = cell.ContentObject;

            var contentItemIndex = Random.Range(0, contentList.Count);
            contentObject.SetObjectContent(contentList[contentItemIndex]);

            cell.SetCellContent(contentObject);
        }

    }

}

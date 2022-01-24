using System.Collections;
using UnityEngine;

namespace Gameplay
{
    public delegate void VoidAction();

    public class GameFieldGridCell : MonoBehaviour
    {
        public static VoidAction SwitchSpawnDirection;


        [SerializeField] private GameObject contentGObj;

        public bool IsEmpty { get; private set; } = true;

        public CellContentObject ContentObject { get; private set; }

        public int RowNumber { get; private set; }
        public int LineNumber { get; private set; }

        private bool isReady;

        /// <summary>
        /// Заполнение ячейки конкретном контентом.
        /// </summary>
        /// <param name="contentObject">Объект содержимого ячейки.</param>
        public void FillCell(CellContentObject contentObject)
        {
            ContentObject = contentObject;
            ContentObject.ChangePosition(this);
            IsEmpty = false;
            if(isReady == false)
            {
                isReady = true;
            }

            ContentObject.gameObject.SetActive(true);
        }

        /// <summary>
        /// Опустошает ячейку.
        /// </summary>
        public void EmptyCell()
        {
            if (IsEmpty) return;
          
            if (ContentObject.IsSpecial) SwitchSpawnDirection?.Invoke();

            ContentObject.gameObject.SetActive(false);

            ContentObject.DetachObjectLocationCell();
            ContentObject = null;
            IsEmpty = true;

            StartCoroutine(EmptyingPause());
        }

        /// <summary>
        /// Назначает внутренний номер столбца и строки.
        /// </summary>
        /// <param name="row">Столбец.</param>
        /// <param name="line">Линия.</param>
        public void SetIndex(int row, int line)
        {
            RowNumber = row;
            LineNumber = line;
        }

        /// <summary>
        /// Пауза.
        /// </summary>
        /// <returns></returns>
        private IEnumerator EmptyingPause()
        {
            yield return new WaitForSeconds(0.3f);
            GameFieldGrid.Instance.OnCellEmptying(this);
        }
    }
}

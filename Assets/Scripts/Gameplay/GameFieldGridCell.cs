using System.Collections;
using UnityEngine;
using Utils;

namespace Gameplay
{

    public class GameFieldGridCell : MonoBehaviour
    {
        [SerializeField] private GameObject contentGObj;


        public bool IsEmpty { get; private set; } = true;

        public CellContentObject ContentObject { get; private set; }

        public int RowNumber { get; private set; }
        public int LineNumber { get; private set; }

        public bool IsReady { get; private set; }

        public void SetStartContent(CellContentObject contentObject)
        {
            ContentObject = contentObject;
            var positionCommand = new SetStartPostionCommand(contentObject, this);
            Invoker.AddCommand(positionCommand);
            IsEmpty = false;
        }

        /// <summary>
        /// Заполнение ячейки конкретном контентом.
        /// </summary>
        /// <param name="contentObject">Объект содержимого ячейки.</param>
        public void FillCell(CellContentObject contentObject)
        {
            contentObject.ChangePosition(this);
            ContentObject = contentObject;
            IsEmpty = false;
        }

        /// <summary>
        /// Опустошает ячейку.
        /// </summary>
        public void EmptyCell()
        {
            if (IsEmpty) return;
          
            //if (ContentObject.IsSpecial) SwitchSpawnDirection?.Invoke();

            ContentObject.gameObject.SetActive(false);

            ContentObject.DetachObjectLocationCell();
            ContentObject = null;
            IsEmpty = true;

            //StartCoroutine(EmptyingPause());
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
            //if (IsEmpty) GameFieldGrid.Instance.OnCellEmptying(this);
        }

        public void NotificateAboutFilling()
        {
            IsEmpty = false;
            Debug.Log("Notification about filling;");
        }

    }
}

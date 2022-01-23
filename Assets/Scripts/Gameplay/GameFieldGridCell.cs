using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace Gameplay
{
    public delegate void CellAction(GameFieldGridCell actingCell);

    public class GameFieldGridCell : MonoBehaviour
    {
        public static event CellAction CellEmptying;

        [SerializeField] private GameObject contentGObj;

        public bool IsEmpty { get; private set; } = true;

        public CellContentObject ContentObject { get; private set; }

        public int RowNumber { get; private set; }
        public int LineNumber { get; private set; }

        private bool isReady;

        /// <summary>
        /// Заполнение ячейки конкретном контентом
        /// </summary>
        /// <param name="contentObject">Объект содержимого ячейки</param>
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
            Debug.Log($"Заполнение ячейки [{RowNumber}, {LineNumber}] ({ContentObject.Content.ContentType} {ContentObject.gameObject.GetHashCode()})");

        }

        public void EmptyCell()
        {
            if (IsEmpty) return;

            Debug.Log($"Опустошение ячейки [{RowNumber}, {LineNumber}] ({ContentObject.Content.ContentType} {ContentObject.gameObject.GetHashCode()})");
            ContentObject.gameObject.SetActive(false);

            ContentObject.DetachObjectLocationCell();
            ContentObject = null;
            IsEmpty = true;

            StartCoroutine(EmptyingPause());
        }

        public void SetIndex(int index, int rowNumber)
        {
            RowNumber = rowNumber;
            LineNumber = index;
        }

        private IEnumerator EmptyingPause()
        {
            yield return new WaitForSeconds(0.3f);
            GameFieldGrid.Instance.OnCellEmptying(this);
        }
    }
}

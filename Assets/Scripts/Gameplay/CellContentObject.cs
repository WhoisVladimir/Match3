using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

namespace Gameplay
{
    public class CellContentObject : MonoBehaviour, ICallbacker
    {
        public event CallbackAction Callback;

        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private GameObject specialContentObject;

        public CellContent Content { get; private set; }
        public GameFieldGridCell LocationCell { get; private set; }
        public bool IsSpecial { get; private set; }

        /// <summary>
        /// Присваивает содержимое объекту
        /// </summary>
        /// <param name="externalContent">Содержимое</param>
        public void SetObjectContent(CellContent externalContent)
        {
            Content = externalContent;
            spriteRenderer.sprite = Content.Sprite;
        }

        public void SetStartPosition(GameFieldGridCell cell)
        {
            transform.position = cell.transform.position;
            LocationCell = cell;
            spriteRenderer.sortingOrder = cell.LineNumber * -1;
            gameObject.SetActive(true);
        }

        /// <summary>
        /// Присваивает новую позицию и соответствующую ячейку.
        /// </summary>
        /// <param name="cell">Заполняемая ячейка</param>
        public void ChangePosition(GameFieldGridCell cell)
        {
            Debug.Log($"Целевая ячейка [{cell.RowNumber}, {cell.LineNumber}]");
            Debug.Log($"Object: {transform.position.ToString()} => cell:  {cell.transform.position.ToString()} ");

            spriteRenderer.sortingOrder = cell.LineNumber * -1;

            StartCoroutine(MoveToCell(cell));
            LocationCell = cell;
        }

        /// <summary>
        /// Выполняет действия при опустошении ячейки.
        /// </summary>
        public void DetachObjectLocationCell()
        {
            if (LocationCell != null) LocationCell = null;
            if (IsSpecial) 
            {
                IsSpecial = false;
                specialContentObject.SetActive(false);
            } 
        }

        /// <summary>
        /// Назначает объекту особый статус.
        /// </summary>
        public void SetSpecial()
        {
            IsSpecial = true;
            specialContentObject.SetActive(true);
        }

        private IEnumerator MoveToCell(GameFieldGridCell targetCell)
        {
            var targetPosition = targetCell.transform.position;
            var localX = transform.position.x;
            var localY = transform.position.y;

            var direction = (targetPosition - transform.position).normalized;

            //while (localX != targetPosition.x || localY != targetPosition.y)
            while(!Mathf.Approximately(localX, targetPosition.x) || !Mathf.Approximately(localY, targetPosition.y))
            {
                localX = (float)Math.Round(transform.position.x, 2);
                localY = (float)Math.Round(transform.position.y, 2);

                transform.Translate(direction * Time.deltaTime);
                //transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 10);
                yield return new WaitForSeconds(0.01f);
            }
            Debug.Log("Выход из цикла");

            transform.position = targetPosition;

            Callback?.Invoke(this);
        }
    }
}

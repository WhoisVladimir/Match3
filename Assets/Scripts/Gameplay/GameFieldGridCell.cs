using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gameplay
{
    public class GameFieldGridCell : MonoBehaviour
    {
        [SerializeField] private GameObject contentGObj;

        public CellContentObject ContentObject { get; private set; }
        public bool IsEmpty { get; private set; } = true;
        public Dictionary<DirectionType, GameFieldGridCell> AdjacentCells { get; private set; }

        private void Awake()
        {
            AdjacentCells = new Dictionary<DirectionType, GameFieldGridCell>();
        }

        /// <summary>
        /// ���������� �������� ������
        /// </summary>
        /// <param name="direction">������� ������</param>
        /// <param name="cell">�������� ������</param>
        public void SetAdjacentCell(DirectionType direction, GameFieldGridCell cell)
        {
            AdjacentCells.Add(direction, cell);
        }

        /// <summary>
        /// ������ ���������� ������
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="contentList">������ ���������� ���������������� ��������</param>
        public void FillCell(List<CellContent> contentList)
        {
            if (IsEmpty)
            {
                contentGObj = Instantiate(contentGObj, transform.position, transform.rotation, GameFieldGrid.Instance.transform);
                IsEmpty = false;
            }

            ContentObject = contentGObj.GetComponent<CellContentObject>();
            var contentItemIndex = Random.Range(0, contentList.Count);
            ContentObject.SetObjectContent(contentList[contentItemIndex]);
        }

        /// <summary>
        /// ������ ���������� ������
        /// </summary>
        /// <param name="contentObject">������ ����������� ������</param>
        public void FillCell(CellContentObject contentObject)
        {
            ContentObject = contentObject;
            ContentObject.transform.position = transform.position;
        }

        /// <summary>
        /// �������� �������� ����� �� ���������� �����������.
        /// </summary>
        public void CheckAdjacentCells()
        {
            Debug.Log($"��������� {ContentObject.Content.ContentType} {GetHashCode()}");
            var checkedCells = new Stack<GameFieldGridCell>();
            var verticalMatches = new List<CellContentObject>();
            var horizontalMatches = new List<CellContentObject>();

            verticalMatches.Add(ContentObject);
            horizontalMatches.Add(ContentObject);

            Debug.Log($"�������� ������-��������. vertical: {verticalMatches.Count} horizontal: {horizontalMatches.Count}");

            foreach (var item in AdjacentCells)
            {
                checkedCells.Push(item.Value);
                Debug.Log($"���������� �������: {item.Key} {item.Value.ContentObject.Content.ContentType}");
            }

            var sourceCell = this;
            Debug.Log($"��� ���������: {sourceCell.GetHashCode()}");
            while (checkedCells.Count > 0)
            {
                var targetCell = checkedCells.Pop();
                Debug.Log($"�������� � ����� {targetCell.ContentObject.Content.ContentType}");
                Debug.Log($"��������� �������: {checkedCells.Count}");

                var currentDirectionType = sourceCell.AdjacentCells
                    .Where(pair => pair.Value.Equals(targetCell))
                    .Select(pair => pair.Key)
                    .FirstOrDefault();
                Debug.Log($"��� �������� ���: {currentDirectionType}");
                if (sourceCell.ContentObject.Content.ContentType == targetCell.ContentObject.Content.ContentType)
                {
                    Debug.Log("������� ������");
                    if (currentDirectionType == DirectionType.DOWN || currentDirectionType == DirectionType.TOP)
                    {
                        verticalMatches.Add(targetCell.ContentObject);
                        Debug.Log($"���������� � vertical. {verticalMatches.Count}");
                    }
                    else
                    {
                        horizontalMatches.Add(targetCell.ContentObject);
                        Debug.Log($"���������� � horizontal. {horizontalMatches.Count}");
                    }

                    if (targetCell.AdjacentCells.TryGetValue(currentDirectionType, out var adjacentCell))
                    {
                        Debug.Log($"�������� �� ����������� ���� ������: {currentDirectionType} ? {adjacentCell}");
                        checkedCells.Push(adjacentCell);
                        Debug.Log($"��������� �������: {checkedCells.Count}");
                        sourceCell = targetCell;
                    }
                    else sourceCell = this;

                }
                else sourceCell = this;

                
                Debug.Log($"����� �����. ���������� ���������-������ {sourceCell.ContentObject.Content.ContentType} {sourceCell.GetHashCode()}");
            }

            if (verticalMatches.Count >= 3) Debug.Log($"{verticalMatches.Count} {ContentObject.Content.ContentType} by vertical!");
            else Debug.Log("No equals by vertical!");
            if (horizontalMatches.Count >= 3) Debug.Log($"{horizontalMatches.Count} {ContentObject.Content.ContentType} by horizontal!");
            else Debug.Log("No equals by horizontal!");
        }
    }
}

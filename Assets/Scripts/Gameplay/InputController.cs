using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace Gameplay
{
    public class InputController : MonoBehaviour
    {
        private Transform targetObj;
        private Vector2 startPoint;

        private GameplayController gameplay;

        private void Start()
        {
            gameplay = GameplayController.Instance;
        }

        /// <summary>
        /// Определяет направление перемещения указателя после нажатия.
        /// </summary>
        /// <param name="context"></param>
        public void TapCellContent(InputAction.CallbackContext context)
        {
            var pointer = context.control.device as Pointer;

            var evSystem = EventSystem.current;
            if (evSystem != null && evSystem.IsPointerOverGameObject(pointer.deviceId)) return;

            if (context.phase == InputActionPhase.Started)
            {
                startPoint = pointer.position.ReadValue();

                var hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(startPoint), Vector2.zero);
                if (hit.collider == null) return;
                else
                {
                    Debug.Log(hit.collider.gameObject.name);
                    targetObj = hit.transform;
                }
            }
            if (context.phase == InputActionPhase.Canceled || context.phase == InputActionPhase.Performed)
            {
                Debug.Log(context.phase);
                if (targetObj == null) return;
                var cell = targetObj.gameObject.GetComponent<GameFieldGridCell>();

                var currentPoint = pointer.position.ReadValue();
                if (startPoint == currentPoint) return;

                Vector2 pointerPosition = Camera.main.ScreenToWorldPoint(currentPoint);
                Vector2 objPosition = targetObj.position;
                var result = pointerPosition - objPosition;
                var x = Mathf.Abs(result.x);
                var y = Mathf.Abs(result.y);
                if (x > y)
                {
                    if (result.x > 0) gameplay.MoveCellContent(cell, DirectionType.RIGHT, true);
                    else gameplay.MoveCellContent(cell, DirectionType.LEFT, true); ;
                }
                else if (y > x)
                {
                    if (result.y > 0) gameplay.MoveCellContent(cell, DirectionType.TOP, true); 
                    else gameplay.MoveCellContent(cell, DirectionType.DOWN, true);
                }

                targetObj = null;
            }
        }
    }
}

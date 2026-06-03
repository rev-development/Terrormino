using UnityEngine;
using UnityEngine.InputSystem;

namespace Helpers
{
    public class SimpleMovement2D : MonoBehaviour
    {
        private InputAction _moveAction;

        private void Start()
        {
            _moveAction = InputSystem.actions.FindAction("Move");
        }

        private void Update()
        {
            var moveValue = _moveAction.ReadValue<Vector2>().normalized;
            //UnityEngine.Debug.Log(moveValue);
            gameObject.transform.Translate(moveValue);
        }
    }
}
using Helpers.Attributes;
using Helpers.Ext;
using UnityEngine;
using UnityEngine.InputSystem;

namespace EC.Tetris
{
	[RequireComponent(typeof(EventBus))]
	[DisallowMultipleComponent]
	public class MKBControls : MonoBehaviour
	{
		[DisableInEditor] [SerializeField] private EventBus _eventBus;

		private TetrisControls _controls;

		private TetrisControls.TetrisLeftActions _leftActions;

		private TetrisControls.TetrisRightActions _rightActions;

		public void Awake()
		{
			_eventBus = gameObject.TryFindComponent<EventBus>();

			_controls = new TetrisControls();
			_leftActions = _controls.TetrisLeft;
			_rightActions = _controls.TetrisRight;
		}

		private void OnEnable() => _controls.Enable();

		// _leftActions.Move.performed += OnMoveInput;
		// _leftActions.Move.canceled += OnMoveCanceled;
		// _rightActions.Rotate.performed += OnRotateInput;
		private void OnHardDropInput(InputAction.CallbackContext context) => _eventBus.HardDropInput.Invoke();

		public void OnMoveCanceled(InputAction.CallbackContext _)
		{
			_eventBus.HorizontalMoveInputCancel.Invoke();
			_eventBus.DownMoveInputCancel.Invoke();
		}

		public void OnMoveInput(InputAction inputAction)
		{
			var inputValue = inputAction.ReadValue<Vector2>();

			var processedInputValue = new Vector2Int();

			switch (inputValue.x)
			{
				case > 0:
					processedInputValue.x = 1;

					break;
				case < 0:
					processedInputValue.x = -1;

					break;
				default:
					_eventBus.HorizontalMoveInputCancel.Invoke();

					break;
			}

			_eventBus.HorizontalMoveInput.Invoke(processedInputValue);

			if (inputValue.y < 0)
				_eventBus.DownMoveInput.Invoke(new Vector2Int(0, -1));
			else
				_eventBus.DownMoveInputCancel.Invoke();
		}

		public void OnRotateInput(InputAction inputAction) =>
			_eventBus.RotateInput.Invoke((int)inputAction.ReadValue<float>());
	}
}
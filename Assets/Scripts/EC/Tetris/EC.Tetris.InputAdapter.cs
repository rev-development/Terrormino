using Helpers.Attributes;
using UnityEngine;
using UnityEngine.InputSystem;

namespace EC.Tetris
{
	/// <summary>
	///     Translates raw Input System callbacks into calls on Controller. Owns DAS
	///     (Delayed Auto Shift) timing and decides whether an input should be forwarded
	///     at all (e.g. gating hard drop behind Config.HardDropEnabled). Controller's
	///     job is only to execute whatever action reaches it, never to decide whether
	///     that action is currently allowed — that gating always belongs here.
	/// </summary>
	[AiGenerated("Claude", "claude-sonnet-5")]
	[RequireComponent(typeof(EventBus))]
	public class InputAdapter : MonoBehaviour
	{
		[SerializeField] private Controller _controller;

		[SerializeField] private EventBus _eventBus;

		private float _dasTimer;

		// DAS (Delayed Auto Shift) state: the direction currently held and how long
		// it's been held. Config.MoveDelay is reused as both the initial hold delay
		// and the repeat interval — a single-knob simplification, not a separate ARR.
		private Vector2Int _heldDirection;

		private void Update()
		{
			if (!_controller.IsRunning) return;

			if (_heldDirection == Vector2Int.zero) return;

			_dasTimer += Time.deltaTime;

			if (_dasTimer < _eventBus.Config.DASDelay) return;

			_dasTimer -= _eventBus.Config.DASDelay;

			_controller.Move(_heldDirection);
		}

		public void OnHardDrop(InputAction.CallbackContext ctx)
		{
			if (!ctx.performed) return;

			if (!_eventBus.Config.HardDropEnabled) return;

			_controller.HardDrop();
		}

		public void OnMove(InputAction.CallbackContext ctx)
		{
			// Gate at the recording point, not with a reset — while the game isn't
			// running, a held direction is never logged into _heldDirection at all,
			// so there's nothing for DAS to have "pre-charged" once it does start.
			if (!_controller.IsRunning) return;

			if (ctx.canceled)
			{
				_heldDirection = Vector2Int.zero;

				return;
			}

			if (!ctx.performed) return;

			var raw = ctx.ReadValue<Vector2>();
			var direction = new Vector2Int(Mathf.RoundToInt(raw.x), Mathf.RoundToInt(raw.y));

			// Clamp vertical to down-only — upward movement is not a valid Tetris input
			direction.y = Mathf.Clamp(direction.y, -1, 0);

			if (direction == Vector2Int.zero
				|| direction == _heldDirection)
				return;

			_heldDirection = direction;
			_dasTimer = 0f;

			_controller.Move(direction);
		}

		public void OnRotate(InputAction.CallbackContext ctx)
		{
			if (!ctx.performed) return;

			var direction = Mathf.RoundToInt(ctx.ReadValue<float>());

			if (direction == 0) return;

			_controller.Rotate(direction);
		}
	}
}
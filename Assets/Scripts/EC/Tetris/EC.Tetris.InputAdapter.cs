using Helpers.Attributes;
using Helpers.Ext;
using UnityEngine;

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
	[DisallowMultipleComponent]
	[RequireComponent(typeof(EventBus))]
	[RequireComponent(typeof(Controller))]
	[AddComponentMenu("EC.Tetris.InputAdapter")]
	public class InputAdapter : MonoBehaviour
	{
		[DisableInEditor] [SerializeField] private Controller _controller;

		[DisableInEditor] [SerializeField] private EventBus _eventBus;

		[SerializeField] private float _arrTimer = 0f;

		[SerializeField] private Vector2Int _dasInput = new();

		[SerializeField] private float _dasTimer = 0f;

		[SerializeField] private Vector2Int _softDropInput = new();

		[SerializeField] private float _softDropTimer = 0f;

		public ConfigSO Config => _eventBus.Config;

		public void Awake()
		{
			_controller = gameObject.TryFindComponent<Controller>();
			_eventBus = gameObject.TryFindComponent<EventBus>();
		}

		private void OnEnable()
		{
			_eventBus.HorizontalMoveInput.AddListener(OnHorizontalMoveInput);
			_eventBus.HorizontalMoveInputCancel.AddListener(OnHorizontalMoveInputCancel);
			_eventBus.DownMoveInput.AddListener(OnDownMoveInput);
			_eventBus.DownMoveInputCancel.AddListener(OnDownMoveInputCancel);
			_eventBus.RotateInput.AddListener(OnRotateInput);
			_eventBus.HardDropInput.AddListener(OnHardDropInput);
		}

		private void Update()
		{
			if (_controller.IsRunning)
			{
				HandleDAS();
				HandleSoftDrop();
			}
		}

		private void OnDisable()
		{
			_eventBus.HorizontalMoveInput.RemoveListener(OnHorizontalMoveInput);
			_eventBus.HorizontalMoveInputCancel.RemoveListener(OnHorizontalMoveInputCancel);
			_eventBus.DownMoveInput.RemoveListener(OnDownMoveInput);
			_eventBus.DownMoveInputCancel.RemoveListener(OnDownMoveInputCancel);
			_eventBus.RotateInput.RemoveListener(OnRotateInput);
			_eventBus.HardDropInput.RemoveListener(OnHardDropInput);
		}

		private void HandleSoftDrop()
		{
			if (_softDropInput == default) return;

			_softDropTimer += Time.deltaTime;

			if (!(_softDropTimer >= Config.SoftDropRate)) return;

			_controller.Move(_softDropInput);
			_softDropTimer = 0;
		}

		public void HandleDAS()
		{
			if (_dasInput == default) return;

			_dasTimer += Time.deltaTime;
			_arrTimer += Time.deltaTime;

			if (!(_dasTimer >= Config.DASDelay)
					|| !(_arrTimer >= Config.AutoRepeatRate))
				return;

			_controller.Move(_dasInput);
			_arrTimer = 0;
		}

		public void OnDownMoveInput(Vector2Int input)
		{
			_controller.Move(input);
			_softDropInput = input;
		}

		private void OnDownMoveInputCancel() => _softDropInput = default;

		[FeatureNotImplemented] public void OnHardDropInput() => _controller.HardDrop();

		public void OnHorizontalMoveInput(Vector2Int input)
		{
			_controller.Move(input);

			if (input != _dasInput) OnHorizontalMoveInputCancel();

			_dasInput = input;
		}

		public void OnHorizontalMoveInputCancel()
		{
			_dasInput = default;
			_dasTimer = 0;
			_arrTimer = 0;
		}

		public void OnRotateInput(int input) => _controller.Rotate(input);
	}
}
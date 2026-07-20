using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Helpers
{
	[Serializable]
	[Helpers.Attributes.AiGeneratedAttribute("Claude", "Sonnet 4.6")]
	public class Timer
	{
		public bool Dirty = false;

		public float BaseAlarmTime = 1f;

		[field: SerializeField] public Vector2 AlarmVarianceRange = new(0f, 0f);

		[SerializeField] private float _elapsedTime;

		[SerializeField] private float _alarmTime;

		[SerializeField] private bool _initialized;

		[SerializeField] private bool _running = false;

		[SerializeField] private bool _ringing = false;

		public Timer(float baseAlarmTime = 1f, Vector2 alarmVarianceRange = default) =>
			Init(baseAlarmTime, alarmVarianceRange);

		public bool Initialized { get => _initialized; private set => _initialized = value; }

		public bool Running { get => _running; private set => _running = value; }

		public bool Ringing { get => _ringing; private set => _ringing = value; }

		/// <summary>
		///     This is when the Timer will ring
		///     BaseAlarmTime + Random with AlarmVarianceRange
		/// </summary>
		public float AlarmTime { get => _alarmTime; private set => _alarmTime = value; }

		public float ElapsedTime { get => _elapsedTime; private set => _elapsedTime = value; }

		public bool Active => Running || Ringing;

		public void Init(float baseAlarmTime = 1f, Vector2 alarmVarianceRange = default)
		{
			BaseAlarmTime = baseAlarmTime;
			AlarmVarianceRange = alarmVarianceRange;
			Initialized = true;
		}

		/// <summary>
		///     Advances the timer. Call this once per frame (e.g. from MonoBehaviour.Update)
		///     with Time.deltaTime, passing the relevant delta for your use case
		///     (Time.deltaTime, Time.unscaledDeltaTime, etc).
		/// </summary>
		public void Tick(float deltaTime)
		{
			if (!Running || Ringing) return;

			ElapsedTime += deltaTime;

			if (!(ElapsedTime >= AlarmTime)) return;

			ElapsedTime = AlarmTime;
			Ringing = true;
			Running = false;
		}

		public void StartNewTimer()
		{
			if (!Initialized) Debug.LogWarning("Timer.StartNewTimer() called before Init().");
			AlarmTime = BaseAlarmTime + Random.Range(AlarmVarianceRange.x, AlarmVarianceRange.y);
			Dirty = true;
			ElapsedTime = 0f;
			Ringing = false;
			Running = true;
		}

		public void ResumeTimer()
		{
			if (Dirty) Running = true;
		}

		public void StopTimer() => Running = false;

		public void StopRinging()
		{
			Running = false;
			Ringing = false;
			ElapsedTime = 0f;
		}
	}
}
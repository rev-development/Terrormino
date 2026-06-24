using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Helpers
{
	[Serializable]
	[AiGenerated("Claude", "Sonnet 4.6")]
	public class Timer
	{
		public float BaseAlarmTime = 1f;

		public Vector2 AlarmVarianceRange = new(0f, 0f);

		public bool Dirty;

		public Timer(float baseAlarmTime = 1f, Vector2 alarmVarianceRange = default) =>
			Init(baseAlarmTime, alarmVarianceRange);

		public bool Initialized { get; private set; }

		/// <summary>
		///     This is when the Timer will ring
		///     BaseAlarmTime + Random with AlarmVarianceRange
		/// </summary>
		public float CurrentAlarmTime { get; private set; }

		public float ElapsedTime { get; private set; }

		public bool Running { get; private set; } = false;

		public bool Ringing { get; private set; } = false;

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

			if (!(ElapsedTime >= CurrentAlarmTime)) return;

			ElapsedTime = CurrentAlarmTime;
			Ringing = true;
			Running = false;
		}

		public void StartNewTimer()
		{
			if (!Initialized) UnityEngine.Debug.LogWarning("Timer.StartNewTimer() called before Init().");
			CurrentAlarmTime = BaseAlarmTime + Random.Range(AlarmVarianceRange.x, AlarmVarianceRange.y);
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
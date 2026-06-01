using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Helpers
{
    [Serializable]
    public class Timer
    {
        private float _currentAlarmTime;
        public float CurrentAlarmTime => _currentAlarmTime;

        private float _startTime = -1f;
        private bool _ringing = false;
        public bool Ringing
        {
            get
            {
                // Evaluate lazily each time it's checked so no Update() is needed
                if (!_ringing && _startTime >= 0f && Time.time >= _startTime + _currentAlarmTime)
                {
                    _ringing = true;
                }
                return _ringing;
            }
        }

        public float BaseAlarmTime = 1f;
        public float AlarmVarianceLowerBound = 0f;
        public float AlarmVarianceUpperBound = 0f;

        public void GenerateAlarmTime()
        {
            _currentAlarmTime =
                BaseAlarmTime
                + UnityEngine.Random.Range(AlarmVarianceLowerBound, AlarmVarianceUpperBound);
        }

        public void StartTimer()
        {
            GenerateAlarmTime();
            _startTime = Time.time;
            _ringing = false;
        }

        public void RestartTimer()
        {
            StartTimer();
        }

        public void ResetTimer()
        {
            GenerateAlarmTime();
            _startTime = Time.time;
            _ringing = false;
        }

        public static List<Timer> FilterRinging(List<Timer> timers)
        {
            return timers.Where(timer => timer.Ringing).ToList();
        }

        public Timer(
            float baseAlarmTime = 1f,
            float alarmVarianceLowerBound = 0f,
            float alarmVarianceUpperBound = 0f
        )
        {
            BaseAlarmTime = baseAlarmTime;
            AlarmVarianceLowerBound = alarmVarianceLowerBound;
            AlarmVarianceUpperBound = alarmVarianceUpperBound;
        }
    }
}
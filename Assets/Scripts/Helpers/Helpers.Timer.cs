using System;
using System.Collections;
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

        private bool _ringing = false;
        public bool Ringing => _ringing;
        public float BaseAlarmTime = 1f;
        public float AlarmVarianceLowerBound = 0f;
        public float AlarmVarianceUpperBound = 0f;

        private IEnumerator Tick()
        {
            yield return new WaitForSeconds(CurrentAlarmTime);
            _ringing = true;
        }

        public void GenerateAlarmTime()
        {
            _currentAlarmTime =
                BaseAlarmTime
                + UnityEngine.Random.Range(AlarmVarianceLowerBound, AlarmVarianceUpperBound);
        }

        public void StartTimer()
        {
            GenerateAlarmTime();
            Tick();
        }

        public void RestartTimer()
        {
            StartTimer();
        }

        public void ResetTimer()
        {
            GenerateAlarmTime();
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

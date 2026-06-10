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

        public float BaseAlarmTime = 1f;

        public float AlarmVarianceLowerBound = 0f;

        public float AlarmVarianceUpperBound = 0f;

        public Timer(float baseAlarmTime = 1f, float alarmVarianceLowerBound = 0f, float alarmVarianceUpperBound = 0f)
        {
            BaseAlarmTime = baseAlarmTime;
            AlarmVarianceLowerBound = alarmVarianceLowerBound;
            AlarmVarianceUpperBound = alarmVarianceUpperBound;
        }

        public float CurrentAlarmTime { get; private set; }

        public bool Ringing { get; private set; } = false;

        private IEnumerator Tick()
        {
            yield return new WaitForSeconds(CurrentAlarmTime);

            Ringing = true;
        }

        public void GenerateAlarmTime()
        {
            CurrentAlarmTime
                = BaseAlarmTime + UnityEngine.Random.Range(AlarmVarianceLowerBound, AlarmVarianceUpperBound);
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

    }
}
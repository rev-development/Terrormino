using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Ambient
{
    public class SwapPosition : Effect
    {
        public float MinDelay = 5f;
        public float MaxDelay = 10f;

        public static void SwapPositions(GameObject a, GameObject b)
        {
            a.transform.GetPositionAndRotation(out var oldPositionA, out var oldRotationA);
            b.transform.GetPositionAndRotation(out var oldPositionB, out var oldRotationB);

            a.transform.SetPositionAndRotation(oldPositionB, oldRotationB);
            b.transform.SetPositionAndRotation(oldPositionA, oldRotationA);
        }

        public class TimerEntry
        {
            public GameObject GameObject;
            public float Timer;
            public float MinDelay;
            public float MaxDelay;

            public void Tick(float deltaTime)
            {
                Timer -= deltaTime;
            }

            public void ResetTimer()
            {
                Timer = Random.Range(MinDelay, MaxDelay);
            }

            public TimerEntry(GameObject gameObject, float minDelay, float maxDelay)
            {
                GameObject = gameObject;
                MinDelay = minDelay;
                MaxDelay = maxDelay;
                Timer = Random.Range(minDelay, maxDelay);
            }
        }

        private readonly Dictionary<int, TimerEntry> _timers = new();

        private void TimerTick(List<GameObject> unwatchedObjects, float deltaTime)
        {
            foreach (var unwatchedObject in unwatchedObjects)
            {
                // If there is a TimerEntry, decrement the timer
                // If there is not a TimerEntry, make one
                if (_timers.TryGetValue(unwatchedObject.GetInstanceID(), out TimerEntry timerEntry))
                {
                    timerEntry.Tick(deltaTime);
                }
                else
                {
                    _timers.Add(
                        unwatchedObject.GetInstanceID(),
                        new TimerEntry(unwatchedObject, MinDelay, MaxDelay)
                    );
                }
            }
        }

        public override void OnTriggerEffect(List<GameObject> unwatchedObjects)
        {
            TimerTick(unwatchedObjects, Time.deltaTime);

            // This is effectively getting the InstanceIDs of all unwatched objects whose timers have hit 0
            var readyTimerKeys = _timers
                .Where(timerEntry => timerEntry.Value.Timer <= 0)
                .Select(timerEntry => timerEntry.Key)
                .ToList();

            if (readyTimerKeys.Count >= 2)
            {
                var chosen = Helpers.Math.PickTwo<int>(readyTimerKeys);

                // Make sure chosen is not empty
                if (
                    _timers.TryGetValue(chosen[0], out TimerEntry choiceA)
                    && _timers.TryGetValue(chosen[1], out TimerEntry choiceB)
                )
                {
                    // Use the chosen keys to get the GameObjects, run a position swap on them
                    SwapPositions(choiceA.GameObject, choiceB.GameObject);

                    // Reset timers
                    choiceA.ResetTimer();
                    choiceB.ResetTimer();
                }
            }
        }
    }
}

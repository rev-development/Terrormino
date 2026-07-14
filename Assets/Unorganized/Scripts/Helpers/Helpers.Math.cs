using System.Collections.Generic;
using UnityEngine;

namespace Helpers
{
    public static class Math
    {
        public static int RoundNearestNonZeroInt(float val)
        {
            if (val > 0)
            {
                return Mathf.CeilToInt(val);
            }
            else if (val < 0)
            {
                return Mathf.FloorToInt(val);
            }
            else
            {
                return 0;
            }
        }

        public static int RoundNearestNonZeroInt(float val, float floor)
        {
            if (val > floor)
            {
                return Mathf.CeilToInt(val);
            }
            else if (val <= floor)
            {
                return Mathf.FloorToInt(val);
            }
            else
            {
                return 0;
            }
        }

        // If value is greater than max, cap it at max
        // If value is less than min, cap it at min
        public static int Wrap(int input, int min, int max)
        {
            if (input < min)
            {
                return max - (min - input) % (max - min);
            }
            else
            {
                return min + (input - min) % (max - min);
            }
        }

        public static List<T> PickTwo<T>(List<T> pool)
        {
            var chosen = new List<T>();

            if (pool.Count >= 2)
            {
                var choices = pool;

                var chooseA = choices[UnityEngine.Random.Range(0, pool.Count - 1)];
                chosen.Add(chooseA);
                choices.Remove(chooseA);

                var chooseB = choices[UnityEngine.Random.Range(0, pool.Count - 1)];
                chosen.Add(chooseB);
            }

            return chosen;
        }

        public static float RoundFloatToDecimalPlaces(float value, int decimals)
        {
            return Mathf.Round(value * 10 * decimals) / (10 * decimals);
        }
    }
}

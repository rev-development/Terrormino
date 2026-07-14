using System;
using UnityEngine;

namespace Helpers
{
    [Serializable]
    public class ClampedFloat
    {
        public float Max = 1f;
        private float _value;
        public float Value
        {
            get { return _value; }
            set { _value = Mathf.Clamp(value, 0, Max); }
        }

        public ClampedFloat(float value, float max = 1f)
        {
            _value = value;
            Max = max;
        }
    }
}

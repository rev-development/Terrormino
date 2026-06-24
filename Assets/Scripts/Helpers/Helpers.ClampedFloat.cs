using System;
using UnityEngine;

namespace Helpers
{
	[Serializable]
	public class ClampedFloat
	{
		[SerializeField] public float Max;

		[SerializeField] private float _value;

		public ClampedFloat(float value, float max = 1f)
		{
			_value = value;
			Max = max;
		}

		public float Value { get => _value; set => _value = Mathf.Clamp(value, 0, Max); }

		public float Percentage => Value / Max;
	}
}
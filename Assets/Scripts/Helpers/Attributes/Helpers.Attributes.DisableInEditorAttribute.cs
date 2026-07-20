using System;
using UnityEngine;

namespace Helpers
{
	[AttributeUsage(AttributeTargets.Field)]
	public class DisableInEditorAttribute : PropertyAttribute
	{
	}
}
using System;
using UnityEngine;

namespace Helpers.Attributes
{
	[AttributeUsage(AttributeTargets.Field)]
	public class DisableInEditorAttribute : PropertyAttribute
	{ }
}
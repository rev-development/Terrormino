using System;
using UnityEngine;

namespace Helpers.Attributes
{
	/// <summary>
	///     Apply to a serializable struct/class field to replace its child Vector2Int[]
	///     fields (that carry [CellGrid]) with a toggleable grid instead of the default
	///     array foldout.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field)]
	public class CellGridContainerAttribute : PropertyAttribute
	{ }
}
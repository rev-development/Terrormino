using System;
using UnityEngine;

namespace Helpers.Attributes
{
	[AttributeUsage(AttributeTargets.Field)]
	public class CellGridAttribute : PropertyAttribute
	{
		public readonly int Columns;
		public readonly int Rows;

		public CellGridAttribute(int columns, int rows)
		{
			Columns = columns;
			Rows = rows;
		}
	}
}

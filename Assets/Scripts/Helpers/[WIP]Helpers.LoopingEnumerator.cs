using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Helpers
{
	/// <summary>
	///     WIP
	/// </summary>
	/// <typeparam name="T"></typeparam>
	[PublicAPI]
	public abstract class LoopingEnumerator<T> : IEnumerator<T>
	{
		private bool _disposed;

		public T[] Items;

		protected LoopingEnumerator(params T[] items) => Items = items;

		public int Index { get; private set; } = -1;

		public bool MoveNext()
		{
			if (Index + 1 >= Items.Length)
				Index = 0;
			else
				Index++;

			return true;
		}

		public void Reset() => Index = 0;

		public T Current => Items[Index];

		object IEnumerator.Current => Current;

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected abstract void Cleanup();

		protected virtual void Dispose(bool disposing)
		{
			if (_disposed) return;

			if (disposing) Cleanup();

			_disposed = true;
		}
	}
}
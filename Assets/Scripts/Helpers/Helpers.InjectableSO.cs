using System.ComponentModel;
using Mapster;
using UnityEngine;

namespace Helpers
{
	/// <summary>
	///     This is a ScriptableObject that can either be created as an asset with data or be initialized as empty and then
	///     injected with a runtime struct.
	/// </summary>
	/// <typeparam name="TData">The data object implementing the same interface</typeparam>
	/// <typeparam name="TInterface">The interface implemented by the ScriptableObject and the struct</typeparam>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public abstract class InjectableSOBase<TData, TInterface> : ScriptableObject
		where TData : TInterface
	{
		public virtual void AssignData(TData dto) => dto.Adapt(this);
	}

	/// <summary>
	///     This is a wrapper for the InjectableSOBase that enforces that the ScriptableObject also implements the interface.
	/// </summary>
	/// <typeparam name="TSelf">This will be the name of the implementing class, RE: CRTP</typeparam>
	/// <typeparam name="TData">The data object implementing the same interface</typeparam>
	/// <typeparam name="TInterface">The interface implemented by the ScriptableObject and the data object</typeparam>
	public abstract class InjectableSO<TSelf, TData, TInterface> : InjectableSOBase<TData, TInterface>
		where TSelf : InjectableSO<TSelf, TData, TInterface>, TInterface
		where TData : TInterface
	{
	}
}
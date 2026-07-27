using System.ComponentModel;
using Mapster;
using UnityEngine;

namespace Helpers
{
	/// <summary>
	///     This is a ScriptableObject that can either be created as an asset with data or be initialized as empty and then
	///     injected with a runtime struct.
	/// </summary>
	/// <typeparam name="TStruct">A struct implementing TInterface</typeparam>
	/// <typeparam name="TInterface">The interface implemented by the ScriptableObject and the struct</typeparam>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public abstract class InjectableSOBase<TStruct, TInterface> : ScriptableObject
		where TStruct : struct, TInterface
	{
		public virtual void AssignData(TStruct dto) => dto.Adapt(this);
	}

	/// <summary>
	///     This is a wrapper for the InjectableSOBase that enforces that the ScriptableObject also implements the interface.
	/// </summary>
	/// <typeparam name="TSelf">This will be the name of the implementing class, RE: CRTP</typeparam>
	/// <typeparam name="TStruct">A struct implementing TInterface</typeparam>
	/// <typeparam name="TInterface">The interface implemented by the ScriptableObject and the struct</typeparam>
	public abstract class InjectableSO<TSelf, TStruct, TInterface> : InjectableSOBase<TStruct, TInterface>
		where TSelf : InjectableSO<TSelf, TStruct, TInterface>, TInterface
		where TStruct : struct, TInterface
	{
	}
}
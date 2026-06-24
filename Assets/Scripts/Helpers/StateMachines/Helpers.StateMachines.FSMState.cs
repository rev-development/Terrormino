using System;
using UnityEngine;

namespace Helpers.StateMachines
{
	[Serializable]
	public abstract class FSMState<TStateType, TStateConfig, TController> : IFSMState<TStateType, TController>
		where TStateType : Enum
		where TController : MonoBehaviour

	{
		public abstract TStateConfig Config { get; protected set; }

		public bool Initialized { get; set; } = false;

		public abstract TStateType StateType { get; }

		[field: SerializeField] public TController Controller { get; protected set; }

		public virtual void Start() => Debug.InitializedWarn(Initialized, GetType().Name);

		public virtual void Update() => Debug.InitializedWarn(Initialized, GetType().Name);

		public virtual void Exit() => Debug.InitializedWarn(Initialized, GetType().Name);

		public FSMState<TStateType, TStateConfig, TController> Init(TController controller, TStateConfig stateConfig)
		{
			Controller = controller;
			Config = stateConfig;
			Initialized = true;

			return this;
		}
	}
}
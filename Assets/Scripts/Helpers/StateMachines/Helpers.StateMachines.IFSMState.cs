using System;
using UnityEngine;

namespace Helpers.StateMachines
{
	public interface IFSMState<out TState, out TController>
		where TState : Enum
		where TController : MonoBehaviour
	{
		public TState StateType { get; }

		public TController Controller { get; }

		public void Start();
		public void Update();
		public void Exit();
	}
}
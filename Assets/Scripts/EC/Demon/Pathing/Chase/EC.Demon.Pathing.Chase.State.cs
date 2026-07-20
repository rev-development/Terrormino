using System;
using Helpers;
using Helpers.Ext;

namespace EC.Demon.Pathing.Chase
{
	[Serializable]
	public class State : Helpers.StateMachines.FSMState<StateType, Config, Controller>
	{
		public override Config Config { get; protected set; } = new();

		public override StateType StateType => StateType.Chase;

		public override void Start()
		{
			Controller.NavMeshAgent.ApplyAreaMask(Config.AreaMask);

			Controller.NavMeshAgent.GoTo(Controller.Player);
		}
	}
}
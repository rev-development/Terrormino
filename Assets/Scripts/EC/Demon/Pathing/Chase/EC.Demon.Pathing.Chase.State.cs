using System;
using Helpers;
using Helpers.Ext;

namespace EC.Demon.Pathing.Chase
{
	[Serializable]
	public class State : FSMState<StateType, IConfig, Controller>
	{
		public override IConfig Config { get; protected set; }

		public override StateType StateType => StateType.Chase;

		public override void Start()
		{
			Controller.NavMeshAgent.ApplyAreaMask(Config.AreaMask);

			Controller.NavMeshAgent.GoTo(Controller.Player);
		}
	}
}
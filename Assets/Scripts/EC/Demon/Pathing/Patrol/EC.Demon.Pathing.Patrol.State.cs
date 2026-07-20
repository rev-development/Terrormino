using System;
using Helpers;
using Helpers.Ext;
using UnityEngine;

namespace EC.Demon.Pathing.Patrol
{
	[Serializable]
	public class State : Helpers.StateMachines.FSMState<StateType, Config, Controller>
	{
		private Timer _idleTimer = new();

		private Timer _patrolTimer = new();

		public override StateType StateType => StateType.Patrol;

		public override Config Config { get; protected set; } = new();

		public override void Start()
		{
			base.Start();

			Controller.NavMeshAgent.ApplyAreaMask(Config.AreaMask);

			_patrolTimer.Init(Config.PatrolDuration);

			_idleTimer.Init(Config.IdleDuration);

			Controller.NavMeshAgent.GoTo(Controller.NavBeaconsBag.Next());

			_patrolTimer.StartNewTimer();
		}

		public override void Update()
		{
			base.Update();
			_patrolTimer.Tick(Time.deltaTime);

			if (_patrolTimer.Ringing) Controller.EnterState(StateType.Chase);

			_idleTimer.Tick(Time.deltaTime);

			if (Controller.NavMeshAgent.IsAtDestination()
				&& !_idleTimer.Active)
			{
				_idleTimer.StartNewTimer();
				Controller.NavMeshAgent.TogglePathing(false);
			}

			if (Controller.NavMeshAgent.IsAtDestination()
				&& _idleTimer.Running
				&& !_idleTimer.Ringing)
				LookAtPlayer();

			if (_idleTimer.Ringing)
			{
				_idleTimer.StopRinging();
				Controller.NavMeshAgent.TogglePathing(true);
				Controller.NavMeshAgent.GoTo(Controller.NavBeaconsBag.Next());
			}
		}

		public void LookAtPlayer()
		{
			if (!Controller.Player) return;

			var directionToPlayer = Controller.Player.transform.position - Controller.gameObject.transform.position;
			directionToPlayer.y = 0f;

			if (directionToPlayer.sqrMagnitude < 0.001f) return;

			Controller.gameObject.transform.rotation = Quaternion.RotateTowards(
				Controller.transform.rotation,
				Quaternion.LookRotation(directionToPlayer),
				Controller.Config.SteeringConfig.AngularSpeed * Time.deltaTime
			);
		}
	}
}
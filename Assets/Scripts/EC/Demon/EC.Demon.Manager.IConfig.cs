using Helpers.Ext;

namespace EC.Demon
{
	public interface IConfig
	{
		float MaxHealth { get; set; }

		int DemonMax { get; set; }

		float SpawnGracePeriod { get; set; }

		float SpawnInterval { get; set; }

		NavMeshAgentExt.SteeringConfig SteeringConfig { get; set; }

		EC.Demon.Pathing.Patrol.ConfigSO PatrolConfigSO { get; set; }

		EC.Demon.Pathing.Chase.ConfigSO ChaseConfigSO { get; set; }
	}
}
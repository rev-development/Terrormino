namespace EC.Demon
{
	public interface IConfigDto
	{
		float MaxHealth { get; set; }

		int DemonMax { get; set; }

		Helpers.Ext.NavMeshAgentExt.SteeringConfig SteeringConfig { get; set; }

		Pathing.Patrol.Config PatrolConfig { get; set; }

		float SpawnGracePeriod { get; set; }

		float SpawnInterval { get; set; }
	}
}
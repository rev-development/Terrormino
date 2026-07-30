using Helpers;
using Helpers.Ext;
using UnityEngine;

namespace EC.Demon
{
	[CreateAssetMenu(fileName = "DemonManagerConfig", menuName = "Terrormino/Demon/Config")]
	public class ConfigSO : InjectableSO<ConfigSO, ConfigData, IConfig>, IConfig
	{
		// public void Awake()
		// {
		// 	PatrolConfigSO ??= CreateInstance<EC.Demon.Pathing.Patrol.ConfigSO>();
		//
		// 	ChaseConfigSO ??= CreateInstance<EC.Demon.Pathing.Chase.ConfigSO>();
		// }

		[field: SerializeField] public EC.Demon.Pathing.Patrol.ConfigSO PatrolConfigSO { get; set; }

		[field: SerializeField] public EC.Demon.Pathing.Chase.ConfigSO ChaseConfigSO { get; set; }

		[field: SerializeField] public float MaxHealth { get; set; }

		[field: SerializeField] public int DemonMax { get; set; }

		[field: SerializeField] public float SpawnGracePeriod { get; set; }

		[field: SerializeField] public float SpawnInterval { get; set; }

		[field: SerializeField] public NavMeshAgentExt.SteeringConfig SteeringConfig { get; set; }
	}
}
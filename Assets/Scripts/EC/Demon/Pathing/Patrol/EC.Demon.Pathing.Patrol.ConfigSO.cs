using Helpers;
using Helpers.Attributes;
using UnityEngine;

namespace EC.Demon.Pathing.Patrol
{
	[CreateAssetMenu(fileName = "DemonPathingPatrolConfig", menuName = "Terrormino/Demon/Pathing/Patrol/Config")]
	public class ConfigSO : InjectableSO<ConfigSO, ConfigData, IConfig>, IConfig
	{
		[field: SerializeField] [field: NavMeshAreaMask] public int AreaMask { get; set; } = 1;

		[field: SerializeField] public float PatrolDuration { get; set; } = 40f;

		[field: SerializeField] public float IdleDuration { get; set; } = 3f;
	}
}
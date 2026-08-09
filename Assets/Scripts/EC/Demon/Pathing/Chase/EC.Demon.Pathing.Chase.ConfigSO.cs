using Helpers;
using Helpers.Attributes;
using UnityEngine;

namespace EC.Demon.Pathing.Chase
{
	[CreateAssetMenu(fileName = "DemonPathingChaseConfig", menuName = "Terrormino/Demon/Pathing/Chase/Config")]
	public class ConfigSO : InjectableSO<ConfigSO, ConfigData, IConfig>, IConfig
	{
		[field: SerializeField] [field: NavMeshAreaMask] public int AreaMask { get; set; } = 1;
	}
}
using UnityEngine;

namespace Demon.Manager
{
	public interface IConfigDto
	{
		float MaxHealth { get; set; }

		float MoveSpeed { get; set; }

		float PatrolDuration { get; set; }

		float FreezeDuration { get; set; }

		/// <summary>
		///     Seconds before the first demon can spawn.
		/// </summary>
		[Tooltip("Seconds before the first demon can spawn.")]
		float GracePeriod { get; set; }

		/// <summary>
		///     Seconds between spawn attempts after grace period ends.
		/// </summary>
		[Tooltip("Seconds between spawn attempts after grace period ends.")]
		float SpawnInterval { get; set; }
	}
}
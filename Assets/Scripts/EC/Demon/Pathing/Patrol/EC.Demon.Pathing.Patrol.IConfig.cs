namespace EC.Demon.Pathing.Patrol
{
	public interface IConfig
	{
		public int AreaMask { get; set; }

		public float PatrolDuration { get; set; }

		public float IdleDuration { get; set; }
	}
}
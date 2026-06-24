namespace EC.Demon.Pathing.Patrol
{
	public interface IConfig
	{
		public float PatrolDuration { get; set; }

		public float IdleDuration { get; set; }

		public int AreaMask { get; set; }
	}
}
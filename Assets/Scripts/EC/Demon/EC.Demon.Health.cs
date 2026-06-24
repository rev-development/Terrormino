using Flashlight;
using UnityEngine;

namespace EC.Demon
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(EventBus))]
	[AddComponentMenu("EC.Demon.Health")]
	public class Health : MonoBehaviour
	{
		[Helpers.DisableInEditorAttribute] [SerializeField] private EventBus _eventBus;

		[field: SerializeField] public Helpers.ClampedFloat HP = new(3f, 3f);

		public void Awake()
		{
			_eventBus = Helpers.Debug.TryFindComponent<EventBus>(gameObject);
		}

		private void OnTriggerExit(Collider other)
		{
			if (other.CompareTag("Flashlight")) _eventBus.Illuminated.Invoke(false);
		}

		private void OnTriggerStay(Collider other)
		{
			if (other.CompareTag("Flashlight"))
			{
				var shake = other.GetComponentInParent<Shake>();

				if (shake.IsActive)
				{
					_eventBus.Illuminated.Invoke(true);
					HP.Value -= Time.deltaTime;

					if (HP.Value <= 0) _eventBus.BanishTriggered.Invoke(gameObject);
				}
				else
				{
					_eventBus.Illuminated.Invoke(false);
				}
			}
		}
	}
}
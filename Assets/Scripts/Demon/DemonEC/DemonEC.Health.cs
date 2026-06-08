using UnityEngine;
using UnityEngine.Events;

namespace Demon.DemonEC
{
    public class Health : MonoBehaviour
    {

        public Helpers.Events.Channels.GameObjectEC Death;

        public UnityEvent<Helpers.ClampedFloat> OnHPChanged = new();

        private readonly Helpers.ClampedFloat _hp = new(3f, 3f);

        public float HP
        {
            get => _hp.Value;
            set
            {
                _hp.Value = value;
                OnHPChanged.Invoke(_hp);

                if (_hp.Value <= 0)
                {
                    Death.RaiseEvent(gameObject);
                }
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.CompareTag("Flashlight"))
            {
                HP -= Time.deltaTime;
            }
        }

    }
}
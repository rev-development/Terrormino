using Demon.Manager;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Demon
{
    [RequireComponent(typeof(AI))]
    [RequireComponent(typeof(AnimationController))]
    [RequireComponent(typeof(Jumpscare))]
    [RequireComponent(typeof(LightFear))]
    [RequireComponent(typeof(Pathing))]
    [AddComponentMenu("Demon.Controller")]
    public class Controller : MonoBehaviour
    {

        public AI AI;

        public AnimationController AnimationController;

        public LightFear LightFear;

        public UnityEvent<GameObject> GlobalBanish = new();

        public UnityEvent<bool> GlobalIlluminate = new();

        public Pathing Pathing;

        [FormerlySerializedAs("ConfigDtoDto")]
        [FormerlySerializedAs("ConfigDataData")]
        [FormerlySerializedAs("ConfigData")]
        public Config Config = new();

        public Demon.Manager.Manager Manager;

        public Jumpscare Jumpscare;

        public void Awake()
        {
            AI = GetComponent<AI>();
            AnimationController = GetComponent<AnimationController>();
            LightFear = GetComponent<LightFear>();
            Pathing = GetComponent<Pathing>();
            Jumpscare = GetComponent<Jumpscare>();
        }

        public void Start()
        {
            Manager ??= Demon.Manager.Manager.Instance;
        }

        public void Init(Config config)
        {
            Config = config;
        }

    }
}
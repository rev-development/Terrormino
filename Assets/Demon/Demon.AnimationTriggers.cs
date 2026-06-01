using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Demon
{
    public class AnimationController : MonoBehaviour
    {
        public GameObject Room;
        public Light Moonlight;
        public AudioSource Scream;

        private string _sceneName; // 1
        private Animator _animator; // 2
        private LightFear _lightFear; // 3
        private Demon.Manager _demonManager; // 4
        private Player.Manager _playerManager; // 5
        private SimpleDissolve _simpleDissolve = new(); // 6

        public void Awake()
        {
            // 1
            _sceneName = ScenePicker.TryGetScenePath(gameObject);

            // 2
            _animator = Helpers.Debug.TryFindComponent<Animator>(gameObject);

            // 3
            _lightFear = Helpers.Debug.TryFindComponent<LightFear>(gameObject);
            if (_lightFear != null)
            {
                _lightFear.Banish.AddListener(OnBanish);
                _lightFear.Illuminate.AddListener(OnIlluminate);
            }

            // 4
            _demonManager = Helpers
                .Debug.TryFindByTag("DemonManager")
                .GetComponent<Demon.Manager>();

            // 5
            _playerManager = Helpers.Debug.TryFindByTag("Player").GetComponent<Player.Manager>();
            if (_playerManager != null)
            {
                _playerManager.GameOver.AddListener(OnJumpScare);
            }

            // 6
            _simpleDissolve.Init(gameObject);
        }

        public void OnBanish(GameObject _)
        {
            _animator.SetTrigger("Banish");
        }

        public void OnIlluminate(bool value)
        {
            _animator.SetBool("IsIlluminated", value);

            if ((_lightFear.Health.Value / _lightFear.Health.Max) <= 1f / 8)
            {
                _simpleDissolve.Dissolve(Time.deltaTime);
            }
        }

        public void OnJumpScare()
        {
            Moonlight.enabled = false;
            _demonManager.Demons.ForEach(demon => demon.SetActive(false));
            Scream.Play();
            Room.SetActive(false);
            StartCoroutine(EndJumpscare());
        }

        IEnumerator EndJumpscare()
        {
            yield return new WaitForSeconds(Scream.clip.length);
            Scream.Stop();
            SceneManager.LoadScene(_sceneName);
        }
    }
}

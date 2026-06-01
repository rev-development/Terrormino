using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;

public class TitleScreen : MonoBehaviour
{
    private float _transitionTime = 10;
    private bool _beginTransition = false;

    public Light LightSource;
    public ParticleSystem Fog;

    public GameObject FogBlock; //Temp fix for fog staying

    private string _sceneName;

    //Shader stuff
    public List<Material> materials = new List<Material>();

    private bool _isDissolving = false;
    private float _dissolveValue = 0f;

    public GameObject GameConsole;

    // Start is called before the first frame update
    public void Start()
    {
        _skinnedMeshRenderers = Helpers.Debug.TryFindComponentsInChildren<SkinnedMeshRenderer>(
            gameObject
        );
        _meshRenderers = Helpers.Debug.TryFindComponentsInChildren<MeshRenderer>(gameObject);
        if (gameObject.TryGetComponent(out ScenePicker scenePicker))
        {
            _sceneName = scenePicker.ScenePath;
        }
        else
        {
            Debug.Log($"No ScenePicker component found on {gameObject.name}", gameObject);
        }
    }

    // Update is called once per frame
    public void Update()
    {
        if (_beginTransition)
        {
            if (_isDissolving)
            {
                DissolveConsole();
            }

            LightSource.intensity -= Time.deltaTime * 0.75f;

            var emission = Fog.emission;
            emission.rateOverTime = Mathf.Max(
                0,
                emission.rateOverTime.constant - Time.deltaTime * 15f
            );

            _transitionTime -= Time.deltaTime;

            if (_transitionTime <= 0)
            {
                BeginGame();
                _transitionTime = 5;

                foreach (var skinnedMeshRenderer in _skinnedMeshRenderers)
                {
                    skinnedMeshRenderer.material.SetFloat("_DissolveValue", _dissolveValue);
                }
            }
        }
    }

    public void BeginGame()
    {
        SceneManager.LoadScene(_sceneName);
    }

    public UnityEvent<InputAction> OnTitleTransitionGrab = new();

    public bool IsDirty = false;

    public void TitleToGameplayTransition(SelectEnterEventArgs context)
    {
        XRGrabInteractable grabInteractable = gameObject.GetComponent<XRGrabInteractable>();
        grabInteractable.interactionManager.SelectExit(
            grabInteractable.interactorsSelecting[0],
            grabInteractable
        );

        if (IsDirty)
        {
            _beginTransition = true;
            _isDissolving = true;
            FogBlock.SetActive(true);
        }
        else
        {
            IsDirty = true;
        }
    }

    private SkinnedMeshRenderer[] _skinnedMeshRenderers;
    private MeshRenderer[] _meshRenderers;

    public void DissolveConsole()
    {
        _dissolveValue += Time.deltaTime * 1f;
        _dissolveValue = Mathf.Clamp01(_dissolveValue); // Keep between 0 and 1

        foreach (var skinnedMeshRenderer in _skinnedMeshRenderers)
        {
            skinnedMeshRenderer.material.SetFloat("_DissolveValue", _dissolveValue);
        }

        foreach (var meshRenderer in _meshRenderers)
        {
            meshRenderer.material.SetFloat("_DissolveValue", _dissolveValue);
        }

        if (_dissolveValue >= 1)
        {
            _isDissolving = false;
        }
    }
}

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class LivingRoomTransition : MonoBehaviour
{
    private float _transitionTime = 10;
    private bool _beginTransition = false;

    public Light LightSource;

    private ScenePicker _scenePicker;

    //Shader stuff
    public List<Material> materials = new List<Material>();

    private bool _isDissolving = false;
    private float _dissolveValue = 0f;

    public GameObject GameConsole;
    public SceneTransitioner SceneTransitioner;

    // Start is called before the first frame update
    public void Start()
    {
        _skinnedMeshRenderers = Helpers.Debug.TryFindComponentsInChildren<SkinnedMeshRenderer>(
            gameObject
        );
        _scenePicker = Helpers.Debug.TryFindComponent<ScenePicker>(gameObject);
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
        SceneTransitioner.FadeAndLoad(_scenePicker.ScenePath);
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
        }
        else
        {
            IsDirty = true;
        }
    }

    private SkinnedMeshRenderer[] _skinnedMeshRenderers;

    public void DissolveConsole()
    {
        _dissolveValue += Time.deltaTime * 1f;
        _dissolveValue = Mathf.Clamp01(_dissolveValue); // Keep between 0 and 1

        foreach (var skinnedMeshRenderer in _skinnedMeshRenderers)
        {
            skinnedMeshRenderer.material.SetFloat("_DissolveValue", _dissolveValue);
        }

        if (_dissolveValue >= 1)
        {
            _isDissolving = false;
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class FlashlightShake : MonoBehaviour
{
    public Light LightSource; //flashlight

    public bool FlashlightActive = false; //flag for knowing if the flashlight is currently active

    private float _battery = 5f; //battery life

    private readonly float smoothingFactor = 0.2f; //helping with noise from the controllers

    private Vector3 _cachedVelocity;

    //Audio

    public AudioSource ShakingSound;
    public AudioSource BatteryOutSound;

    // Start is called before the first frame update
    void Start()
    {
        // Ensure the flashlight starts off
        LightSource.enabled = false;
    }

    void Update()
    {
        if (FlashlightActive == true) // Flashlight battery drains when held
        {
            _battery -= Time.deltaTime; //Battery continually loses charge
        }

        if (_battery <= 0) // Battery dies
        {
            LightSource.enabled = false;
            FlashlightActive = false;
            BatteryOutSound.Play();
        }
    }

    public UnityEvent<InputAction> TogglePower = new();

    public void OnTogglePower(InputAction inputAction) //By using the input router we can check what device was being used to press the button that triggers the unity event
    {
        FlashlightActive = !FlashlightActive;
        LightSource.enabled = FlashlightActive;
    }

    public void VelocityCheck(InputAction inputAction)
    {
        var position = inputAction.ReadValue<Vector3>();
        CalcVelocity(position);
    }

    public void VelocityCheck(Vector3 position)
    {
        CalcVelocity(position);
    }

    public void CalcVelocity(Vector3 position)
    {
        float cachedMagnitude = _cachedVelocity.magnitude;
        float currentMagnitude = position.magnitude;
        _cachedVelocity = Vector3.Lerp(_cachedVelocity, position, smoothingFactor);

        if (cachedMagnitude > 0.2f) //checking to make sure we arent dividing by an insignificant amount to filter out noise
        {
            float PercentageIncrease =
                Mathf.Abs((currentMagnitude - _cachedVelocity.magnitude) / cachedMagnitude) * 100f; //Math for checking the percentage increase between past magnitude and current magnitude

            if (PercentageIncrease >= 2.5f) //checking to see if the current magnitude increased by 2.5% (i.e. shaking)
            {
                _battery += Time.deltaTime * 6f;
                ShakingSound.Play();
            }
            else
            {
                ShakingSound.Stop();
            }
        }

        _cachedVelocity = position;
    }
}

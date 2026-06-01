using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class TutorialGuide : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI TutorialText;
    private float _textChangeTimer = 8;

    public UnityEvent<InputAction> ObjectGrabbed = new();

    private void Start() { }

    private void Update()
    {
        TextManager();
    }

    public void OnGrabbed(InputAction action) { }

    private void TextManager()
    {
        _textChangeTimer -= Time.deltaTime;

        if (_textChangeTimer <= 0)
        {
            TutorialText.text = "Try grabbing the game console to your right";
        }

        //TODO: When the game console is grabbed with BOTH hands have the text change. Eventually try adding text
        //      that makes the player try controlling the Tetris with only one hand and show flashlight
    }
}

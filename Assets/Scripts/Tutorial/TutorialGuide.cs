using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class TutorialGuide : MonoBehaviour
{
	[SerializeField] private TextMeshProUGUI TutorialText;

	public UnityEvent<InputAction> ObjectGrabbed = new();

	private float _textChangeTimer = 8;

	private void Start()
	{
	}

	private void Update() => TextManager();

	private void TextManager()
	{
		_textChangeTimer -= Time.deltaTime;

		if (_textChangeTimer <= 0) TutorialText.text = "Try grabbing the game console to your right";

		//TODO: When the game console is grabbed with BOTH hands have the text change. Eventually try adding text
		//      that makes the player try controlling the Tetris with only one hand and show flashlight
	}

	public void OnGrabbed(InputAction action)
	{
	}
}
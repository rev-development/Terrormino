using System.Collections.Generic;
using UnityEngine;

public class DemonAudio : MonoBehaviour
{
	public List<AudioSource> HeartBeats = new();

	private float _timer;

	// Start is called before the first frame update
	private void Start()
	{
		HeartBeatSFXConnect();
		_timer = Random.Range(5f, 6f);
	}

	// Update is called once per frame
	private void Update()
	{
		_timer = _timer - Time.deltaTime;

		if (_timer <= 0)
		{
			var sound = HeartBeats[Random.Range(0, HeartBeats.Count)];
			sound.Play();
			_timer = Random.Range(10f, 12f);
		}
	}

	private void HeartBeatSFXConnect()
	{
		var HeartBeatAudio = GameObject.Find("HeartBeatSFX");

		if (HeartBeatAudio != null)
		{
			HeartBeats.Clear();
			HeartBeats.AddRange(HeartBeatAudio.GetComponents<AudioSource>());
		}
	}
}
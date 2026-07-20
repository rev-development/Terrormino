using System.IO;
using UnityEngine;

namespace GameLoop
{
	public class SaveManager : MonoBehaviour
	{
		public string SavePath;

		public static SaveManager Instance { get; private set; }

		public void Awake()
		{
			if (Instance != null
				&& Instance != this)
			{
				Destroy(gameObject);

				return;
			}

			Instance = this;

			DontDestroyOnLoad(this);
		}

		private void Start() => SavePath = Path.Combine(Application.persistentDataPath, "savegame.json");

		public void SaveGame(SaveData data)
		{
			var json = JsonUtility.ToJson(data, true);
			File.WriteAllText(SavePath, json);
			Debug.Log($"Data saved to: {SavePath}");
		}

		public SaveData LoadGame()
		{
			if (!File.Exists(SavePath)) return null;

			var json = File.ReadAllText(SavePath);

			return JsonUtility.FromJson<SaveData>(json);
		}
	}
}
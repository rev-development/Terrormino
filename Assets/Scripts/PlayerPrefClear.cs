using UnityEngine;

public class PlayerPrefClear : MonoBehaviour
{

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        PlayerPrefs.DeleteAll();
    }

    // Start is called before the first frame update
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
    }

}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPrefClear : MonoBehaviour
{



    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        PlayerPrefs.DeleteAll();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

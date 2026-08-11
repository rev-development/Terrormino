using UnityEngine;
using UnityEngine.Events;

public class SFXTrigger : MonoBehaviour
{

    public UnityEvent hallwayTriggerEvent = new();



    void Start()
    {

    }


    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            hallwayTriggerEvent.Invoke();
        }
    }
}

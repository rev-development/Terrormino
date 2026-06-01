using UnityEngine;

public class Reachable : MonoBehaviour
{
    private Collider _playerReach;
    private Vector3 _startingPosition;
    private Quaternion _startingRotation;

    // Start is called before the first frame update
    void Start()
    {
        _playerReach = GameObject.Find("Player.ReachableArea").GetComponent<Collider>();
        _startingPosition = gameObject.transform.position;
        _startingRotation = gameObject.transform.rotation;
    }

    public void OnTriggerExit(Collider other)
    {
        if (other == _playerReach)
        {
            gameObject.transform.position = _startingPosition;
            gameObject.transform.rotation = _startingRotation;
        }
    }
}

using UnityEngine;

public class ScenePicker : MonoBehaviour
{
    [SerializeField]
    public string ScenePath;

    public static string TryGetScenePath(GameObject gameObject)
    {
        if (gameObject.TryGetComponent(out ScenePicker scenePicker))
        {
            return scenePicker.ScenePath;
        }
        else
        {
            Debug.Log($"No ScenePicker component found on {gameObject.name}", gameObject);
            return null;
        }
    }
}

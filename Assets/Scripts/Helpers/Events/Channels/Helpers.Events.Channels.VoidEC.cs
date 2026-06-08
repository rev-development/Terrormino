using UnityEngine;
using UnityEngine.Events;

namespace Helpers.Events.Channels
{
    [CreateAssetMenu(fileName = "VoidEC", menuName = "Helpers/Events/Channels/VoidEC")]
    public class VoidEC : ScriptableObject
    {

        [Tooltip("The action to perform; Listeners subscribe to this UnityAction")]
        public UnityAction OnEventRaised;

        public void RaiseEvent()
        {
            OnEventRaised?.Invoke();
        }

    }
}
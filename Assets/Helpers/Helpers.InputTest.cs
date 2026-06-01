using UnityEngine;
using UnityEngine.InputSystem;

namespace Helpers
{
    public class InputTest : MonoBehaviour
    {
        public static void Test(InputAction.CallbackContext ctx)
        {
            UnityEngine.Debug.Log(ctx);
        }
    }
}

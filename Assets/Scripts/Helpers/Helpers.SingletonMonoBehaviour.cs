using UnityEngine;

namespace Helpers
{
	public abstract class SingletonMonoBehaviour<TImplementingClassName> : MonoBehaviour
		where TImplementingClassName : MonoBehaviour
	{
		public static TImplementingClassName Instance { get; private set; }

		protected virtual void Awake()
		{
			if (Instance != null
				&& Instance != this)
			{
				Destroy(gameObject);

				return;
			}

			Instance = this as TImplementingClassName;
		}
	}
}
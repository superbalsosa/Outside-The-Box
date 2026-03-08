using UnityEngine;

namespace Utilities
{
    public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T instance;
        private static bool isShuttingDown = false;
        private static readonly object lockObject = new object();

        [Header("Singleton Options")]
        [SerializeField] bool dontDestroyOnLoad = true;

        public static T Instance
        {
            get
            {
                if (isShuttingDown)
                {
                    Debug.LogWarning($"[Singleton] Instance '{typeof(T)}' already destroyed. Returning null.");
                    return null;
                }

                lock (lockObject)
                {
                    if (instance == null)
                    {
                        instance = FindAnyObjectByType<T>(FindObjectsInactive.Include);

                        if (instance == null)
                        {
                            GameObject singletonObject = new GameObject(typeof(T).Name);
                            instance = singletonObject.AddComponent<T>();
                        }
                    }
                    return instance;
                }
            }
        }
        protected virtual void Awake()
        {
            if (instance == null)
            {
                instance = this as T;
                transform.SetParent(null);
                if (dontDestroyOnLoad)
                {
                    DontDestroyOnLoad(transform);
                }
            }
            else if (instance != this)
            {
                Destroy(gameObject);
            }
        }
        protected virtual void OnApplicationQuit()
        {
            isShuttingDown = true;
        }
        protected virtual void OnDestroy()
        {
            if (instance == this)
            {
                isShuttingDown = true;
            }
        }
    } 
}
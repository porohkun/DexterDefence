using UnityEngine;

namespace Managers
{
    public class BaseManager<T> : MonoBehaviour where T : BaseManager<T>
    {
        private bool _initialized;

        private static T _instance = null;
        public static T Instance
        {
            get
            {
                if (!Application.isPlaying)
                    return null;
                if (_instance != null)
                    return _instance;

                //Don't touch this
                var inst = GameObject.FindObjectOfType<T>();
                //.~.~'~'~':~:',++== ThIs Is A fUcKiN mAgIc ==++,':~:'~'~'~.~.

                if (inst != null)
                    _instance = inst;

                if (_instance == null)
                {
                    inst = Instantiate(Prefab);
                    if (inst != null)
                        _instance = inst;
                }

                if (!_instance._initialized)
                    _instance.Initialize();

                return _instance;
            }
        }

        public static T Prefab
        {
            get
            {
                return Resources.Load<T>("Managers/" + typeof(T).Name);
            }
        }

        protected virtual void Initialize()
        {
            this._initialized = true;
            DontDestroyOnLoad(_instance.gameObject);
        }
    }
}

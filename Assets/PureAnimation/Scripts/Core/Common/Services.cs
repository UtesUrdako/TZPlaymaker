using System;
using UnityEngine;

namespace Common
{
    public abstract class MonoBehaviourService<T> : MonoBehaviour where T : class
    {
        private void Awake()
        {
            if (Services<T>.IsRegistered) DestroyImmediate(this);
                
            Services<T>.Register(this as T);
            OnCreateService();
        }

        private void OnDestroy()
        {
            if (!Services<T>.IsRegistered) return;
            
            OnDestroyService();
            Services<T>.Unregister(this as T);
        }

        protected abstract void OnCreateService();
        protected abstract void OnDestroyService();
    }
    
    
    public static class Services<T> where T : class
    {
        private static T _instance;
        
        public static  bool IsRegistered => (object) _instance != null;

        public static T Get()
        {
            if (_instance != null) return _instance;

            var type = typeof(T);

            if (type.IsSubclassOf(typeof(ScriptableObject)))
            {
                var list = Resources.FindObjectsOfTypeAll(type);

                if (list == null || list.Length == 0)
                  throw new UnityException(("Service<{0}> can be used only with exist/ Loaded asset with this type", type.Name).ToString());

                _instance = list[0] as T;
                return _instance;
            }
            
#if UNITY_EDITOR
            if (type.IsSubclassOf(typeof(Component)) && !type.IsSubclassOf(typeof(MonoBehaviourService<T>)))
                throw new UnityException (("\"{0}\" - invalid type, should be inherited from MonoBehaviourService", type.Name).ToString());
#endif
            if (type.IsSubclassOf (typeof (MonoBehaviourService<T>))) 
            {
                
#if UNITY_EDITOR
                if (!Application.isPlaying) throw new UnityException (("Service<{0}>.Get() can be used only at PLAY mode", type.Name).ToString());
#endif
                new GameObject (
#if UNITY_EDITOR
                    "_SERVICE_" + type.Name
#endif
                ).AddComponent (type);
            } else 
            {
                Register (Activator.CreateInstance (type) as T);
            }
            return _instance;
        }

        /// <summary>
        /// Register instance as service.
        /// </summary>
        /// <param name="instance">Service instance.</param>
        /// 
        public static void Register (T instance) {
            
            if (IsRegistered) 
                throw new UnityException ( ("Cant register \"{0}\" as service - type already registered", typeof (T).Name).ToString());

            _instance = instance ?? throw new UnityException ("Cant register null instance as service");
        }
        
        public static void Unregister (T instance, bool force = false) {
            if (instance == _instance || force) {
                _instance = null;
            }
        }
    }

}

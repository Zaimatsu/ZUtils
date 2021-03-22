using UnityEngine;

namespace ZUtils.ObjectPool
{
    public class MonoObjectPool<T> : ObjectPool<T> where T : MonoBehaviour, IPooledObject
    {
        private class MonoPooledObjectFactory : IPooledObjectFactory<T>
        {
            private readonly T prefab;
            private readonly Transform parent;

            public MonoPooledObjectFactory(T prefab, Transform parent = null)
            {
                this.prefab = prefab;
                this.parent = parent;
            }

            public T Create()
            {
                return Object.Instantiate(prefab, parent);
            }
        }

        public MonoObjectPool(T prefab, Transform parent = null) : base(new MonoPooledObjectFactory(prefab, parent))
        {
        }
    }

    public abstract class MonoPooledObject : MonoBehaviour, IPooledObject
    {
        public event OnReturned Returned;

        public void Return() => Returned?.Invoke(this);
        public virtual void OnBeforeSpawn() => gameObject.SetActive(true);
        public virtual void OnBeforeDespawn() => gameObject.SetActive(false);
    }
}

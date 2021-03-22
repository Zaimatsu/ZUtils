using System.Collections.Generic;

namespace ZUtils.ObjectPool
{
    public delegate void OnReturned(IPooledObject caller);

    public class ObjectPool<T> where T : class, IPooledObject
    {
        public delegate void OnChanged(ObjectPool<T> caller);

        public event OnChanged Changed;
        public List<T> Spawned { get; }

        private readonly IPooledObjectFactory<T> objectFactory;
        private readonly Stack<T> free;

        public ObjectPool(IPooledObjectFactory<T> objectFactory)
        {
            this.objectFactory = objectFactory;
            free = new Stack<T>();
            Spawned = new List<T>();
        }

        public T Get()
        {
            T spawnedObject = free.Count == 0 ? objectFactory.Create() : free.Pop();
            spawnedObject.Returned += SpawnedObjectOnReturned;
            spawnedObject.OnBeforeSpawn();
            Spawned.Add(spawnedObject);
            Changed?.Invoke(this);
            return spawnedObject;
        }

        private void SpawnedObjectOnReturned(IPooledObject pooledObject)
        {
            var tObject = pooledObject as T;
            if (free.Contains(tObject))
                return;

            pooledObject.Returned -= SpawnedObjectOnReturned;
            pooledObject.OnBeforeDespawn();
            free.Push(tObject);
            Spawned.Remove(tObject);
            Changed?.Invoke(this);
        }
    }

    public interface IPooledObject
    {
        event OnReturned Returned;
        void Return();
        void OnBeforeSpawn();
        void OnBeforeDespawn();
    }

    public abstract class PooledObject : IPooledObject
    {
        public event OnReturned Returned;

        public void Return()
        {
            Returned?.Invoke(this);
        }

        public virtual void OnBeforeSpawn()
        {
        }

        public virtual void OnBeforeDespawn()
        {
        }
    }

    public interface IPooledObjectFactory<out T> where T : class, IPooledObject
    {
        T Create();
    }
}

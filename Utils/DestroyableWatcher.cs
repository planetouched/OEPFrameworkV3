using System.Collections.Generic;
using OEPFrameworkV3._Base;

namespace OEPFrameworkV3.Utils
{
    public class DestroyableWatcher
    {
        private readonly List<IDestroyableObject> _objects = new();

        public void Add(IDestroyableObject destroyable)
        {
            _objects.Add(destroyable);
            destroyable.Destroyed += OnDestroy;
        }

        private void OnDestroy(IDestroyableObject destroyableObject)
        {
            _objects.Remove(destroyableObject);
        }

        public void DestroyAll()
        {
            foreach (var droppableObject in _objects)
            {
                droppableObject.Destroyed -= OnDestroy;
                droppableObject.Destroy();
            }
            
            _objects.Clear();
        }
    }
}
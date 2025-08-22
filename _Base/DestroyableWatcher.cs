using System.Collections.Generic;

namespace OEPFrameworkV3._Base
{
    public class DestroyableWatcher
    {
        private readonly List<IDestroyableObject> _droppableObjects = new();

        public void Add(IDestroyableObject destroyable)
        {
            _droppableObjects.Add(destroyable);
            destroyable.Destroyed += OnDestroy;
        }

        private void OnDestroy(IDestroyableObject destroyableObject)
        {
            _droppableObjects.Remove(destroyableObject);
        }

        public void DestroyAll()
        {
            foreach (var droppableObject in _droppableObjects)
            {
                droppableObject.Destroyed -= OnDestroy;
                droppableObject.Destroy();
            }
            
            _droppableObjects.Clear();
        }
    }
}
using MonoGame.Extended.Collisions;
using System.Collections.Generic;
using System.Linq;

namespace Flooded_Soul.System.Collision
{
    class CollisionTracker
    {
        private HashSet<ICollisionActor> _currentlyColliding = new HashSet<ICollisionActor>();
        private HashSet<ICollisionActor> _collidingThisFrame = new HashSet<ICollisionActor>();

        public delegate void CollisionEvent(ICollisionActor other);
        public event CollisionEvent CollisionEnter;
        public event CollisionEvent CollisionStay;
        public event CollisionEvent CollisionExit;

        public void RegisterCollision(ICollisionActor other)
        {
            _collidingThisFrame.Add(other);

            if (!_currentlyColliding.Contains(other))
            {
                _currentlyColliding.Add(other);
                CollisionEnter?.Invoke(other);
            }
            else
            {
                CollisionStay?.Invoke(other);
            }
        }

        public void Update()
        {
            var exited = _currentlyColliding.Except(_collidingThisFrame).ToList();
            foreach (var actor in exited)
            {
                _currentlyColliding.Remove(actor);
                CollisionExit?.Invoke(actor);
            }

            _collidingThisFrame.Clear();
        }
    }
}

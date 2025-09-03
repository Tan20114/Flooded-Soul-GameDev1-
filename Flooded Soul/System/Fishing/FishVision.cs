using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RectangleF = MonoGame.Extended.RectangleF;
using SizeF = MonoGame.Extended.SizeF;

namespace Flooded_Soul.System.Fishing
{
    class FishVision : ICollisionActor
    {
        private Fish parent;
        private HashSet<ICollisionActor> _currentlyColliding = new HashSet<ICollisionActor>();
        private HashSet<ICollisionActor> _collidingThisFrame = new HashSet<ICollisionActor>();

        public RectangleF _bound;
        public IShapeF Bounds => _bound;

        public FishVision(Fish fish, RectangleF rect)
        {
            parent = fish;
            _bound = rect;
        }

        public void OnCollision(CollisionEventArgs collisionInfo)
        {
            var other = collisionInfo.Other;

            // Register collision this frame
            _collidingThisFrame.Add(other);

            // Trigger enter only once
            if (!_currentlyColliding.Contains(other))
            {
                _currentlyColliding.Add(other);
                OnCollisionEnter(other);
            }
        }

        public void Update()
        {
            // Find actors that stopped colliding
            var exited = _currentlyColliding.Except(_collidingThisFrame).ToList();
            foreach (var actor in exited)
            {
                _currentlyColliding.Remove(actor);
                OnCollisionExit(actor);
            }

            // Clear for next frame
            _collidingThisFrame.Clear();
        }

        private void OnCollisionEnter(ICollisionActor other)
        {
            if (other is Hook)
                parent.speed *= 2;
        }

        private void OnCollisionExit(ICollisionActor other)
        {
            if (other is Hook)
                parent.speed /= 2;
        }
    }

}

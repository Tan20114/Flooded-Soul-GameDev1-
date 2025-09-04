using Flooded_Soul.System.Collision;
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
        CollisionTracker tracker;

        public RectangleF _bound;
        public IShapeF Bounds => _bound;

        int speedMultiplier = 3;

        public FishVision(Fish fish, RectangleF rect)
        {
            parent = fish;
            _bound = rect;

            tracker = new CollisionTracker();

            tracker.CollisionEnter += OnCollisionEnter;
            tracker.CollisionExit += OnCollisionExit;
        }

        public void OnCollision(CollisionEventArgs collisionInfo)
        {
            tracker.RegisterCollision(collisionInfo.Other);
        }

        public void Update()
        {
            tracker.Update();
        }

        private void OnCollisionEnter(ICollisionActor other)
        {
            if (other is Hook)
                parent.speed *= speedMultiplier;
        }

        private void OnCollisionExit(ICollisionActor other)
        {
            if (other is Hook)
                parent.speed /= speedMultiplier;
        }
    }
}

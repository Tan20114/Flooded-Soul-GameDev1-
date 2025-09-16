using Flooded_Soul.System.Collision_System;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Color = Microsoft.Xna.Framework.Color;
using RectangleF = MonoGame.Extended.RectangleF;
using SizeF = MonoGame.Extended.SizeF;

namespace Flooded_Soul.System.Fishing
{
    public class FishVision : ICollisionActor
    {
        private Fish parent;
        public CollisionTracker Collider;

        public RectangleF _bound;
        public IShapeF Bounds => _bound;

        int speedMultiplier = 3;

        public FishVision(Fish fish, RectangleF rect)
        {
            parent = fish;
            _bound = rect;

            Collider = new CollisionTracker();

            Collider.CollisionEnter += OnCollisionEnter;
            Collider.CollisionExit += OnCollisionExit;
        }

        public void OnCollision(CollisionEventArgs collisionInfo) => Collider.RegisterCollision(collisionInfo.Other);

        public void Update() => Collider.Update();

        public void Draw()
        {
            if (Collider.Collideable)
                Game1.instance._spriteBatch.DrawRectangle(_bound, Color.Yellow, 2);
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

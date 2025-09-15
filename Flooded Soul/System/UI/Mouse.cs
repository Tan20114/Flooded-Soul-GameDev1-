using Flooded_Soul.System.Collision_System;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flooded_Soul.System.UI
{
    public class Mouse : ICollisionActor
    {
        CollisionTracker Collider = new CollisionTracker();

        CircleF mousePos;
        public IShapeF Bounds => mousePos;

        public Vector2 posOffset = new Vector2(0, 0);

        public Mouse()
        {
            mousePos = new CircleF(new Vector2(Game1.instance.mouseState.X, Game1.instance.mouseState.Y), 5f);

            Game1.instance.collisionComponent.Insert(this);
        }

        public void OnCollision(CollisionEventArgs collisionInfo) => Collider.RegisterCollision(collisionInfo.Other);
        public void Update()
        {
            Vector2 mousePos = new Vector2(Game1.instance.mouseState.X /*+ posOffset.X*/, Game1.instance.mouseState.Y /*+ posOffset.Y*/);
            this.mousePos.Position = mousePos;
        }
    }
}

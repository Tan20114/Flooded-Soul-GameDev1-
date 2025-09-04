using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Color = Microsoft.Xna.Framework.Color;

namespace Flooded_Soul.System.Fishing
{
    internal class FishPointer : ICollisionActor
    {
        CircleF mousePos;
        public IShapeF Bounds => mousePos;

        public FishPointer()
        {
            Vector2 mousePos = new Vector2(Game1.instance.mouseState.X, Game1.instance.mouseState.Y + Game1.instance.viewPortHeight);

            this.mousePos = new CircleF(mousePos, 1);
        }

        public void Update()
        {
            Vector2 mousePos = new Vector2(Game1.instance.mouseState.X, Game1.instance.mouseState.Y + Game1.instance.viewPortHeight);

            this.mousePos.Position = mousePos;
        }

        public void Draw()
        {
            Game1.instance._spriteBatch.DrawCircle(mousePos, 3, Color.Red, 3);
        }

        public void OnCollision(CollisionEventArgs collisionInfo)
        {
            Debug.WriteLine("Collide With" + collisionInfo.Other);
        }
    }
}

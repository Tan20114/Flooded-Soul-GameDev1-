using Flooded_Soul.System.BG;
using Flooded_Soul.System.Collision_System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Color = Microsoft.Xna.Framework.Color;
using Keys = Microsoft.Xna.Framework.Input.Keys;
using RectangleF = MonoGame.Extended.RectangleF;

namespace Flooded_Soul.System.Shop
{
    internal class ShopLayer : ParallaxLayer , ICollisionActor
    {
        CollisionTracker Collider;

        Vector2 pos;

        RectangleF bound;
        public IShapeF Bounds => bound;

        public ShopLayer(string path, Vector2 posOffset, int speed) : base(path, posOffset, speed, 1)
        {
            Collider = new CollisionTracker();

            Collider.CollisionEnter += OnCollisionEnter;

            texture = Game1.instance.Content.Load<Texture2D>(path);

            pos = new Vector2(posOffset.X * 2, posOffset.Y);

            bound = new RectangleF(.59f * texture.Width * scaleX + posOffset.X * 2, posOffset.Y, .27f * texture.Width * scaleX, texture.Height * scaleY);

            Game1.instance.collisionComponent.Insert(this);
        }

        public void Update(GameTime gt, int speed)
        {
            if (isStop) return;

            if (pos.X + texture.Width * scaleX < 0)
            {
                pos = new Vector2(posOffset.X * 2, posOffset.Y);
                isStop = true;
                bound.Position = new Vector2(.59f * texture.Width * scaleX + posOffset.X * 2, posOffset.Y);
            }

            pos -= new Vector2(speed * Game1.instance.deltaTime, 0);
            bound.Position -= new Vector2(speed * Game1.instance.deltaTime,0);

            Collider.Update();
        }

        public void Draw(int i)
        {
            Game1.instance._spriteBatch.Draw(
                    texture,
                    pos,
                    null,
                    Color.White,
                    0f,
                    Vector2.Zero,
                    new Vector2(scaleX, scaleY),
                    SpriteEffects.None,
                    0f
                );
            Game1.instance._spriteBatch.DrawRectangle(bound, Color.Red, 1);
        }

        public void OnCollision(CollisionEventArgs collisionInfo) => Collider.RegisterCollision(collisionInfo.Other);

        void OnCollisionEnter(ICollisionActor other)
        {
            if (other is Player && Game1.instance.autoStopAtShop)
            {
                TogglePause();
            }
        }
    }
}

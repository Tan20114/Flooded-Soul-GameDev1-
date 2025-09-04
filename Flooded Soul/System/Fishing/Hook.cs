using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.ViewportAdapters;
using MonoGame.Extended.Collisions;
using Rectangle = Microsoft.Xna.Framework.Rectangle;
using System.Diagnostics;
using Color = Microsoft.Xna.Framework.Color;
using RectangleF = MonoGame.Extended.RectangleF;
using SizeF = MonoGame.Extended.SizeF;

namespace Flooded_Soul.System.Fishing
{
    internal class Hook : ICollisionActor
    {
        Texture2D texture;
        Vector2 pos;
        float scale = 0.2f;
        int followSpeed = 100;

        public int hookUpSpeed = 100;

        RectangleF _bounds;
        public IShapeF Bounds => _bounds;

        public Hook()
        {
            scale *= Game1.instance.screenRatio;

            texture = Game1.instance.Content.Load<Texture2D>("hook_test");

            pos = new Vector2(Game1.instance.viewPortWidth / 2, Game1.instance.viewPortHeight);
            _bounds = new RectangleF(pos, new SizeF(texture.Width * scale, texture.Height * scale));
        }

        public void Update()
        {
            PositionRestrict();
            if (Game1.instance.sceneState != Scene.Fishing)
            {
                ResetPosition();
                return;
            }
            _bounds.Position = pos;

            Vector2 mousePos = new Vector2(Game1.instance.mouseState.X, Game1.instance.mouseState.Y + Game1.instance.viewPortHeight);

            Vector2 distance = mousePos - pos;

            Vector2 moveDir = Vector2.Normalize(distance);
            
            if (Game1.instance.sceneState == Scene.Fishing)
                if(distance.Length() > 10)
                    pos += moveDir * followSpeed * Game1.instance.deltaTime;
                else
                    pos = mousePos;
        }

        public void OnCollision(CollisionEventArgs collisionInfo)
        {
            //Debug.WriteLine($"Hook collided with: {collisionInfo.Other}");
        }

        public void Draw()
        {
            Game1.instance._spriteBatch.DrawRectangle(_bounds, Color.Red, 3);
            Game1.instance._spriteBatch.Draw(texture, pos, null, Color.White, 0f, Vector2.Zero, new Vector2(scale), SpriteEffects.None, 0f);
        }

        void ResetPosition() => pos = new Vector2(Game1.instance.viewPortWidth / 2, Game1.instance.viewPortHeight);

        void PositionRestrict()
        {
            if(pos.X < 0)
                pos.X = 0;
            else if (pos.X > Game1.instance.viewPortWidth - texture.Width * scale)
                pos.X = Game1.instance.viewPortWidth - texture.Width * scale;
            if (pos.Y < Game1.instance.viewPortHeight)
                pos.Y = Game1.instance.viewPortHeight;
            else if (pos.Y > 2 * Game1.instance.viewPortHeight - texture.Height * scale)
                pos.Y = 2 * Game1.instance.viewPortHeight - texture.Height * scale;
        }
    }
}

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
using Flooded_Soul.System.Collision_System;
using MonoGame.Extended.Graphics;

namespace Flooded_Soul.System.Fishing
{
    internal class Hook : ICollisionActor
    {
        public CollisionTracker Collider;

        FishingManager fishingManager;

        Texture2DAtlas atlas;
        Texture2DRegion frameToDraw;
        int frameColumn = 4;
        int frameRow = 1;

        Vector2 pos;
        float scale = 5f;
        int followSpeed = 100;

        public int hookUpSpeed = 100;

        RectangleF _bounds;
        public IShapeF Bounds => _bounds;

        public Hook(FishingManager f)
        {
            fishingManager = f;

            Texture2D tex = Game1.instance.Content.Load<Texture2D>("fishing_hook");
            atlas = Texture2DAtlas.Create("hook_atlas", tex, tex.Width/frameColumn, tex.Height/frameRow);

            frameToDraw = atlas.CreateSprite(0).TextureRegion;

            Collider = new CollisionTracker();

            scale *= Game1.instance.screenRatio;

            pos = new Vector2(Game1.instance.viewPortWidth / 2, Game1.instance.viewPortHeight);
            _bounds = new RectangleF(pos, new SizeF(frameToDraw.Width * scale, frameToDraw.Height * scale));
        }

        public void Update()
        {
            PositionRestrict();
            UpdateSpeed();
            ChangeSprite(Game1.instance.player.HookLevel);

            _bounds.Position = pos;

            Vector2 mousePos = new Vector2(Game1.instance.mouseState.X, Game1.instance.mouseState.Y + Game1.instance.viewPortHeight);

            Vector2 distance = mousePos - pos;

            Vector2 moveDir = Vector2.Normalize(distance);

            if (Game1.instance.sceneState == Scene.Fishing)
            {
                if (fishingManager.isMinigame && fishingManager.targetFish != null && fishingManager.targetFish.IsActive)
                    pos = fishingManager.targetFish.pos;
                else
                    if (distance.Length() > 10)
                    pos += moveDir * followSpeed * Game1.instance.deltaTime;
                else
                    pos = mousePos;
            }
        }

        public void OnCollision(CollisionEventArgs collisionInfo) => Collider.RegisterCollision(collisionInfo.Other);
        public void Draw()
        {
            //if(Collider.Collideable)
            //    Game1.instance._spriteBatch.DrawRectangle(_bounds, Color.Red, 3);

            float linePosX = pos.X + (frameToDraw.Width * scale * .8f);
            float startPointY = pos.Y + (frameToDraw.Height * scale * .15f);

            Game1.instance._spriteBatch.Draw(frameToDraw, pos, Color.White, 0, Vector2.Zero, new Vector2(scale), SpriteEffects.None, 0);
            Game1.instance._spriteBatch.DrawLine(linePosX,startPointY,linePosX, Game1.instance.viewPortHeight,Color.White);
        }

        public void ResetPosition() => pos = new Vector2(Game1.instance.viewPortWidth / 2, Game1.instance.viewPortHeight);

        void PositionRestrict()
        {
            if (pos.X < 0)
                pos.X = 0;
            else if (pos.X > Game1.instance.viewPortWidth - frameToDraw.Width * scale)
                pos.X = Game1.instance.viewPortWidth - frameToDraw.Width * scale;
            if (pos.Y < Game1.instance.viewPortHeight)
                pos.Y = Game1.instance.viewPortHeight;
            else if (pos.Y > 2 * Game1.instance.viewPortHeight - frameToDraw.Height * scale)
                pos.Y = 2 * Game1.instance.viewPortHeight - frameToDraw.Height * scale;
        }

        void UpdateSpeed() => followSpeed = 100 + (Game1.instance.player.HookLevel * 50);

        public void ChangeSprite(int index) => frameToDraw = atlas.CreateSprite(index).TextureRegion;
    }
}

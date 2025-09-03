using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;
using Color = Microsoft.Xna.Framework.Color;
using RectangleF = MonoGame.Extended.RectangleF;
using SizeF = MonoGame.Extended.SizeF;

namespace Flooded_Soul.System.Fishing
{
    internal class Fish : ICollisionActor
    {
        protected Random random = new Random();
        public FishVision vision;

        FishingManager fishingManager;

        protected Texture2D texture;
        public Vector2 pos;
        protected float scale = 0.05f;
        public int speed = 100;

        protected int minSpawnHeight;
        protected int maxSpawnHeight;

        protected float initialSpawnRatio = .2f;
        protected float normalMaxSpawnRatio = .5f;
        protected float rareMaxSpawnRatio = .7f;
        protected float legendMaxSpawnRatio = .9f;

        protected int point = 0;

        RectangleF _bounds;
        RectangleF _seeRange;
        public IShapeF Bounds => _bounds;
        public IShapeF seeRange => _seeRange;

        public Fish(string textureName, float scale,FishingManager manager)
        {

            texture = Game1.instance.Content.Load<Texture2D>(textureName);
            fishingManager = manager;

            this.scale = scale * Game1.instance.screenRatio;
            
            RandomDir();
            RandomPos();
            _bounds = new RectangleF(pos, new SizeF(texture.Width * this.scale, texture.Height * this.scale));
            _seeRange = new RectangleF(_bounds.Position.X,_bounds.Position.Y,_bounds.Width * 2,_bounds.Height);

            vision = new FishVision(this, _seeRange);

            Game1.instance.collisionComponent.Insert(this);
            Game1.instance.collisionComponent.Insert(vision);
        }

        public void Update()
        {
            _bounds.Position = pos;
            vision._bound.Position = _seeRange.Position;

            pos.X += speed * Game1.instance.deltaTime;

            if (pos.X > Game1.instance.viewPortWidth - texture.Width * scale)
                speed = -speed;
            else if (pos.X < 0)
                speed = -speed;

            if(pos.Y < Game1.instance.viewPortHeight)
            {
                RandomPos();
                RandomDir();
            }

            if (speed > 0)
                _seeRange.Position = new Vector2(_bounds.Right,_bounds.Y);
            else
                _seeRange.Position = new Vector2(_bounds.Left - _seeRange.Width,_bounds.Y);
        }

        public void Draw()
        {
            Game1.instance._spriteBatch.DrawRectangle(_bounds, Color.Green, 3);
            Game1.instance._spriteBatch.DrawRectangle(_seeRange, Color.Blue, 3);
            Game1.instance._spriteBatch.Draw(texture, pos, null, Color.White, 0f, Vector2.Zero, new Vector2(scale), SetFaceDir(), 0f);
        }

        SpriteEffects SetFaceDir() => speed > 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

        public void OnCollision(CollisionEventArgs collisionInfo)
        {
            if (collisionInfo.Other is Hook hook)
                pos.Y -= hook.hookUpSpeed * Game1.instance.deltaTime;
        }


        void RandomDir() => speed = random.Next(0, 2) == 0 ? speed : -speed;

        protected virtual void RandomPos()
        {
            float minSpawnRatio = initialSpawnRatio;

            minSpawnHeight = (int)(Game1.instance.viewPortHeight * minSpawnRatio);
            maxSpawnHeight = (int)(Game1.instance.viewPortHeight * legendMaxSpawnRatio);

            pos.X = random.Next(0, Game1.instance.viewPortWidth - (int)(texture.Width * scale));
            pos.Y = random.Next(minSpawnHeight,maxSpawnHeight) + Game1.instance.viewPortHeight;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Flooded_Soul.System.Collision_System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;
using Color = Microsoft.Xna.Framework.Color;
using RectangleF = MonoGame.Extended.RectangleF;
using SizeF = MonoGame.Extended.SizeF;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace Flooded_Soul.System.Fishing
{
    internal class Fish : ICollisionActor
    {
        protected Random random = new Random();
        public FishVision vision;
        CollisionTracker Collider;

        FishingManager fishingManager;
        Queue<Fish> fishToRemove = new Queue<Fish>();

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

        FishingGameArea minigame = null;
        bool isMinigame = false;
        public bool otherMinigame = false;

        public Fish(string textureName, float scale,FishingManager manager)
        {
            texture = Game1.instance.Content.Load<Texture2D>(textureName);
            fishingManager = manager;

            this.scale = scale * Game1.instance.screenRatio;
            
            RandomDir();
            RandomPos();
            _bounds = new RectangleF(pos, new SizeF(texture.Width * this.scale, texture.Height * this.scale));
            _seeRange = new RectangleF(_bounds.Position.X,_bounds.Position.Y,_bounds.Width * 3,_bounds.Height);

            vision = new FishVision(this, _seeRange);

            Collider = new CollisionTracker();

            Collider.CollisionEnter += OnCollisionEnter;
            Collider.CollisionExit += OnCollisionExit;

            Game1.instance.collisionComponent.Insert(this);
            Game1.instance.collisionComponent.Insert(vision);
        }

        public void Update()
        {
            Collider.Update();
            if(minigame != null)
                minigame.Update();

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
            Game1.instance._spriteBatch.Draw(texture, pos, null, Color.White, 0f, Vector2.Zero, new Vector2(scale), SetFaceDir(), 0f);

            if (Collider.Collideable)
                Game1.instance._spriteBatch.DrawRectangle(_bounds, Color.Green, 3);

            vision.Draw();

            if (isMinigame)
                minigame.Draw();
        }

        SpriteEffects SetFaceDir() => speed > 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

        #region Collision
        public void OnCollision(CollisionEventArgs collisionInfo) => Collider.RegisterCollision(collisionInfo.Other);

        void OnCollisionEnter(ICollisionActor other)
        {
            if (other is Hook)
            {
                StartMinigame();
            }
        }

        void OnCollisionExit(ICollisionActor other)
        {

        }
        #endregion

        #region Initialize
        void RandomDir() => speed = random.Next(0, 2) == 0 ? speed : -speed;

        protected virtual void RandomPos()
        {
            float minSpawnRatio = initialSpawnRatio;

            minSpawnHeight = (int)(Game1.instance.viewPortHeight * minSpawnRatio);
            maxSpawnHeight = (int)(Game1.instance.viewPortHeight * legendMaxSpawnRatio);

            pos.X = random.Next(0, Game1.instance.viewPortWidth - (int)(texture.Width * scale));
            pos.Y = random.Next(minSpawnHeight,maxSpawnHeight) + Game1.instance.viewPortHeight;
        }
        #endregion

        void StartMinigame()
        {
            if (isMinigame) return;

            fishToRemove = new Queue<Fish>(fishingManager.fishInScreen.Where(f => f != this));
            Collider.EnableCollision();
            vision.Collider.DisableCollision();

            foreach (Fish f in fishToRemove)
            {
                f.Collider.DisableCollision();
                f.vision.Collider.DisableCollision();
            }

            minigame = new FishingGameArea(pos);
            isMinigame = true;
        }

        public void EndMinigame()
        {
            foreach (Fish f in fishToRemove)
            {
                f.Collider.EnableCollision();
                f.vision.Collider.EnableCollision();
            }

            Collider.EnableCollision();
            vision.Collider.EnableCollision();

            minigame = null;
            isMinigame = false;
            fishToRemove.Clear();
        }
    }
}

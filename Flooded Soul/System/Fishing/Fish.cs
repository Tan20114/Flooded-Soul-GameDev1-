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
    public class Fish : ICollisionActor
    {
        protected Random random = new Random();
        Hook hook;
        public FishVision vision;
        public CollisionTracker Collider;

        protected FishingManager fishingManager;
        Queue<Fish> fishToRemove = new Queue<Fish>();

        protected Texture2D texture;
        public Vector2 pos;
        protected float scale = 0.05f;
        public float speed = 100;
        int goDownSpeed = 50;
        protected float strength = 1;
        public float Strength { get => strength; set => strength = value; }

        public bool isClicked = false;
        float resetClickTime = .5f;
        float elasped = 0;

        protected int minSpawnHeight;
        protected int maxSpawnHeight;

        protected float initialSpawnRatio = .2f;
        protected float normalMaxSpawnRatio = .5f;
        protected float rareMaxSpawnRatio = .7f;
        protected float legendMaxSpawnRatio = .9f;

        protected int point = 0;
        int visionRange = 5;

        public bool isPause = false;

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
            _seeRange = new RectangleF(_bounds.Position.X,_bounds.Position.Y,_bounds.Width * visionRange,_bounds.Height);

            vision = new FishVision(this, _seeRange);

            Collider = new CollisionTracker();

            Collider.CollisionEnter += OnCollisionEnter;
            Collider.CollisionExit += OnCollisionExit;

            Game1.instance.collisionComponent.Insert(this);
            Game1.instance.collisionComponent.Insert(vision);
        }

        public void Update()
        {
            if (isPause) return;

            Collider.Update();
            
            _bounds.Position = pos;
            vision._bound.Position = _seeRange.Position;

            pos.X += speed * Game1.instance.deltaTime;

            if (isClicked)
                elasped += Game1.instance.deltaTime;

            if(elasped >= resetClickTime)
            {
                isClicked = false;
                elasped = 0;
            }

            if (fishingManager.isMinigame)
            {
                if (fishingManager.targetFish == this)
                    if(!isClicked)
                        pos.Y += goDownSpeed * Game1.instance.deltaTime;
                    else
                        pos.Y = MathHelper.Lerp(pos.Y, targetY, 0.05f);
            }

            if (pos.X > Game1.instance.viewPortWidth - texture.Width * scale)
                speed = -speed;
            else if (pos.X < 0)
                speed = -speed;

            if(pos.Y < Game1.instance.viewPortHeight * .1f + Game1.instance.viewPortHeight)
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

            //if (Collider.Collideable)
            //    Game1.instance._spriteBatch.DrawRectangle(_bounds, Color.Green, 3);

            //vision.Draw();
        }

        SpriteEffects SetFaceDir() => speed > 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

        #region Collision
        public void OnCollision(CollisionEventArgs collisionInfo) => Collider.RegisterCollision(collisionInfo.Other);

        void OnCollisionEnter(ICollisionActor other)
        {
            if (other is Hook)
            {
                Debug.WriteLine("Hooked");
                hook = other as Hook;
                fishingManager.targetFish = this;
                fishingManager.otherFishes = fishingManager.fishInScreen.Where(f => f != this).ToList();

                Collider.EnableCollision();
                vision.Collider.DisableCollision();
                hook.Collider.DisableCollision();
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

        public void EndMinigame()
        {
            foreach (Fish f in fishToRemove)
            {
                f.Collider.EnableCollision();
                f.vision.Collider.EnableCollision();
            }

            Collider.EnableCollision();
            vision.Collider.EnableCollision();

            fishingManager.minigameArea = null;
            fishingManager.isMinigame = false;
            fishToRemove.Clear();
        }

        public void Reset()
        {
            RandomPos();
            RandomDir();
            _bounds.Position = pos;
            _seeRange.Position = new Vector2(_bounds.Right, _bounds.Y);
        }

        int targetY;

        public int MoveToY(int targetY) => this.targetY = targetY;

        public void OnAlert()
        {
            speed /= 1.05f;
            int ranVal = random.Next(1, 101);

            if (ranVal > 95)
                speed *= 1;
            else
                speed *= -1;
        }

        public virtual void Destroy(bool Success)
        {
            fishingManager.fishInScreen.Remove(this);
            Collider.DisableCollision();
            vision.Collider.DisableCollision();
            if (Success)
                Game1.instance.player.fishPoint += point + Game1.instance.fm.bonus;
        }
    }
}

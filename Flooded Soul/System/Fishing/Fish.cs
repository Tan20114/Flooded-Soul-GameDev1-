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
        string fish_Id;

        protected Random random = new Random();
        Hook hook;
        public FishVision vision;
        public CollisionTracker Collider;

        protected FishingManager fishingManager;
        Queue<Fish> fishToRemove = new Queue<Fish>();

        protected Texture2D texture;
        public Vector2 pos;
        protected float scale = 0.05f;
        protected float initialSpeed = 100;
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
        public bool IsActive { get; set; } = false;
        bool isHooked = false;

        public Fish(string textureName, float scale,FishingManager manager)
        {
            fish_Id = textureName;

            texture = Game1.instance.Content.Load<Texture2D>(textureName);
            fishingManager = manager;

            this.scale = scale * Game1.instance.screenRatio;

            speed = initialSpeed;

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

            _bounds.Position = pos;
            _seeRange.Position = speed > 0
                ? new Vector2(_bounds.Right, _bounds.Y)
                : new Vector2(_bounds.Left - _seeRange.Width, _bounds.Y);

            Collider.Update();
            vision._bound.Position = _seeRange.Position;
            vision.Collider.Update();

            pos.X += speed * Game1.instance.deltaTime;

            if (fishingManager.isMinigame && fishingManager.targetFish == this)
            {
                if (!isClicked)
                    pos.Y += goDownSpeed * Game1.instance.deltaTime;
                else
                    pos.Y = MathHelper.Lerp(pos.Y, targetY, 0.05f);
            }

            if (pos.X > Game1.instance.viewPortWidth - texture.Width * scale || pos.X < 0)
                speed = -speed;
        }

        public void Draw() => Game1.instance._spriteBatch.Draw(texture, pos, null, Color.White, 0f, Vector2.Zero, new Vector2(scale), SetFaceDir(), 0f);

        SpriteEffects SetFaceDir() => speed > 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

        #region Collision
        public void OnCollision(CollisionEventArgs collisionInfo) => Collider.RegisterCollision(collisionInfo.Other);


        void OnCollisionEnter(ICollisionActor other)
        {
            if (!IsActive || isHooked || fishingManager.targetFish != null)
                return;

            if (other is Hook hookOther)
            {
                isHooked = true;

                hook = hookOther;
                fishingManager.targetFish = this;

                fishingManager.otherFishes = fishingManager.fishInScreen.Where(f => f != this && f.IsActive).ToList();

                Collider.DisableCollision();

                if (vision?.Collider != null) vision.Collider.DisableCollision();
                if (hook?.Collider != null) hook.Collider.DisableCollision();

                Debug.WriteLine($"Hooked {fish_Id} at pos {pos} bounds {_bounds} (isActive={IsActive})");
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

        public void EndMinigame() => isHooked = false;

        public void Reset(string textureName, FishingManager manager)
        {
            fish_Id = textureName;
            texture = Game1.instance.Content.Load<Texture2D>(textureName);
            fishingManager = manager;

            Game1.instance.collisionComponent.Remove(this);
            Game1.instance.collisionComponent.Remove(vision);

            RandomDir();
            RandomPos();
            speed = initialSpeed;

            _bounds = new RectangleF(pos, new SizeF(texture.Width * this.scale, texture.Height * this.scale));
            _seeRange = new RectangleF(_bounds.Position.X, _bounds.Position.Y, _bounds.Width * visionRange, _bounds.Height);

            vision = new FishVision(this, _seeRange);

            Collider.EnableCollision();
            vision.Collider.EnableCollision();

            Game1.instance.collisionComponent.Insert(this);
            Game1.instance.collisionComponent.Insert(vision);

            IsActive = true;
            isPause = false;
            isClicked = false;
            isHooked = false;
            elasped = 0;
        }

        public virtual void Reset()
        {
            Game1.instance.collisionComponent.Remove(this);
            Game1.instance.collisionComponent.Remove(vision);

            RandomPos();
            RandomDir();

            _bounds = new RectangleF(pos, new SizeF(texture.Width * scale, texture.Height * scale));
            _seeRange = new RectangleF(_bounds.Right, _bounds.Y, _bounds.Width * visionRange, _bounds.Height);

            vision = new FishVision(this, _seeRange);

            speed = initialSpeed;

            Game1.instance.collisionComponent.Insert(this);
            Game1.instance.collisionComponent.Insert(vision);

            Collider.EnableCollision();
            vision.Collider.EnableCollision();

            IsActive = true;
            isPause = false;
            isClicked = false;
            isHooked = false;
            elasped = 0;
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
            Game1.instance.collisionComponent.Remove(this);
            vision.Collider.DisableCollision();
            Game1.instance.collisionComponent.Remove(vision);

            if (Success)
            {
                AudioManager.Instance.PlaySfx("point_up");
                Game1.instance.player.fishPoint += point;
                FishPoint.Spawn(point, new Vector2(pos.X + GetTexWidth,pos.Y));
                Game1.instance.collection.AddFish(fish_Id);
            }

            IsActive = false;
        }

        public float GetTexWidth => texture.Width * scale;
    }
}

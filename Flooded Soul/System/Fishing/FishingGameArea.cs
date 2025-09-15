using Flooded_Soul.System.Collision_System;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using RectangleF = MonoGame.Extended.RectangleF;
using Vector2 = Microsoft.Xna.Framework.Vector2;    

namespace Flooded_Soul.System.Fishing
{
    internal class FishingGameArea : ICollisionActor
    {
        FishingManager fishingManager;

        CollisionTracker Collider;

        RectangleF bound;
        public IShapeF Bounds => bound;

        FishingGameArea boundToCreate= null;

        public bool fishInArea = true;

        int areaWidth = 1500;
        int heightOffset = 400;

        public FishingGameArea(Vector2 pos, FishingManager fishingManager)
        {
            this.fishingManager = fishingManager;

            Collider = new CollisionTracker();

            Collider.CollisionEnter += OnCollisionEnter;
            Collider.CollisionStay += OnCollisionStay;
            Collider.CollisionExit += OnCollisionExit;

            float spawnWidth = areaWidth * Game1.instance.screenRatio;
            float spawnHeight = Game1.instance.viewPortHeight + heightOffset;

            Vector2 spawnPos = new Vector2(pos.X - (spawnWidth / 2), Game1.instance.viewPortHeight - heightOffset);

            bound = new RectangleF(spawnPos.X, spawnPos.Y, spawnWidth, spawnHeight);

            boundToCreate = this;
        }

        public void Update()
        {
            if(boundToCreate != null)
            {
                Game1.instance.collisionComponent.Insert(boundToCreate);
                boundToCreate = null;
            }
            Collider.Update();
        }

        public void Draw() => Game1.instance._spriteBatch.DrawRectangle(bound, Microsoft.Xna.Framework.Color.Red, 3);

        public void OnCollision(CollisionEventArgs collisionInfo) => Collider.RegisterCollision(collisionInfo.Other);
        private void OnCollisionEnter(ICollisionActor other)
        {
            if (other == fishingManager.targetFish)
                fishInArea = true;
        }
        private void OnCollisionStay(ICollisionActor other)
        {
            if(other == fishingManager.targetFish)
                fishInArea = true;
        }
        private void OnCollisionExit(ICollisionActor other)
        {
            if (other == fishingManager.targetFish)
                fishInArea = false;
        }
    }
}

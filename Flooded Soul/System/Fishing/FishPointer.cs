using Flooded_Soul.System.Collision_System;
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
    public class FishPointer : ICollisionActor
    {
        FishingManager fishingManager;

        CircleF mousePos;
        public IShapeF Bounds => mousePos;

        public bool canClick = false;

        CollisionTracker Collider;

        public FishPointer(FishingManager fishingManager)
        {
            this.fishingManager = fishingManager;
            Vector2 mousePos = new Vector2(Game1.instance.mouseState.X, Game1.instance.mouseState.Y + Game1.instance.viewPortHeight);

            this.mousePos = new CircleF(mousePos, 3);
            Collider = new CollisionTracker();

            Collider.CollisionEnter += OnCollisionEnter;
            Collider.CollisionStay += OnCollisionStay;
            Collider.CollisionExit += OnCollisionExit;
        }

        public void Update()
        {
            Vector2 mousePos = new Vector2(Game1.instance.mouseState.X, Game1.instance.mouseState.Y + Game1.instance.viewPortHeight);

            this.mousePos.Position = mousePos;

            Collider.Update();
        }

        //public void Draw() => Game1.instance._spriteBatch.DrawCircle(mousePos, 3, Color.Red, 3);

        public void OnCollision(CollisionEventArgs collisionInfo) => Collider.RegisterCollision(collisionInfo.Other);

        void OnCollisionEnter(ICollisionActor other)
        {
            if (other == fishingManager.targetFish)
                canClick = true;
        }

        void OnCollisionStay(ICollisionActor other)
        {
            if(other == fishingManager.targetFish)
                canClick = true;
        }

        void OnCollisionExit(ICollisionActor other)
        {
            if(other == fishingManager.targetFish)
                canClick = false;
        }
    }
}

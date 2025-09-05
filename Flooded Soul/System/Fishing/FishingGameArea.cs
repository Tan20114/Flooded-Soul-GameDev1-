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
        CollisionTracker Collider;

        RectangleF bound;
        public IShapeF Bounds => bound;

        FishingGameArea boundToCreate= null;

        int areaWidth = 2000;
        int areaHeight = 1600;

        public FishingGameArea(Vector2 pos)
        {
            Collider = new CollisionTracker();

            Collider.CollisionEnter += OnCollisionEnter;
            Collider.CollisionStay += OnCollisionStay;
            Collider.CollisionExit += OnCollisionExit;

            float spawnWidth = areaWidth * Game1.instance.screenRatio;
            float spawnHeight = areaHeight * Game1.instance.screenRatio;

            Vector2 spawnPos = new Vector2(pos.X - (spawnWidth/2),pos.Y - (spawnHeight/2));

            bound = new RectangleF(spawnPos.X,spawnPos.Y, spawnWidth, spawnHeight);

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
            if(other is Fish)
                Debug.WriteLine("Fish entered area");
        }
        private void OnCollisionStay(ICollisionActor other)
        {
            if(other is Fish)
                Debug.WriteLine("Fish in area");
        }
        private void OnCollisionExit(ICollisionActor other)
        {
            if(other is Fish)
                Debug.WriteLine("Fish exited area");
        }
    }
}

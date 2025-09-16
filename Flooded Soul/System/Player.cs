using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Color = Microsoft.Xna.Framework.Color;
using Keys = Microsoft.Xna.Framework.Input.Keys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;
using Flooded_Soul.System.BG;
using System.Diagnostics;
using MonoGame.Extended.Collisions;
using MonoGame.Extended;
using RectangleF = MonoGame.Extended.RectangleF;
using Flooded_Soul.System.Collision_System;
using Flooded_Soul.System.Shop;

namespace Flooded_Soul.System
{
    public class Player : ICollisionActor
    {
        CollisionTracker Collider;

        int minSpeed = 0;
        public int maxSpeed = 100;

        public int speed = 100;
        public int Speed
        {
            get => speed;
            set => speed = MathHelper.Clamp(value, minSpeed, maxSpeed);
        }

        RectangleF bound;

        public IShapeF Bounds => bound;

        int distanceTraveled;
        public int fishPoint;

        bool isStop = false;

        public int hookLevel = 1;
        public int boatLevel = 1;

        int ySpeed = 5;
        float yTravelled = 0;

        List<ParallaxLayer> currentTex = new List<ParallaxLayer>();

        Vector2 pos;

        public Player()
        {
            Collider = new CollisionTracker();

            Collider.CollisionEnter += OnCollisionEnter;

            distanceTraveled = 0;
            fishPoint = 0;

            currentTex = LV1Draw();

            Texture2D tex = Game1.instance.Content.Load<Texture2D>("Boat/LV1/2_seperate_boat_LV1");
            float scaleX = Game1.instance.viewPortWidth / tex.Width;
            float scaleY = Game1.instance.viewPortHeight / tex.Height;
            bound = new RectangleF(.13f * tex.Width * scaleX,0,.05f * tex.Width,Game1.instance.viewPortHeight);

            pos = Vector2.Zero;

            Game1.instance.collisionComponent.Insert(this);
        }

        public void Update(KeyboardState ks, GameTime gameTime)
        {
            TestBoat();
            LevelVisualize();
            WaveMovement();
            Collider.Update();

            if (!isStop)
                distanceTraveled += (int)(Speed * (float)gameTime.ElapsedGameTime.TotalSeconds);
        }

        public void Draw(SpriteFont font)
        {
            for (int i = 0; i< currentTex.Count; i++)
            {
                currentTex[i].Draw(pos);
            }
            Game1.instance._spriteBatch.DrawRectangle(bound,Color.White);
        }

        public void Sail()
        {
            speed = maxSpeed;
            isStop = false;
        }

        public void Stop()
        {
            speed = minSpeed;
            isStop = true;
        }

        public int GetDistance() => distanceTraveled;

        void LevelVisualize()
        {
            switch(boatLevel)
            {
                case 1:
                    currentTex = LV1Draw();
                    break;
                case 2:
                    currentTex = LV2Draw();
                    break;
                case 3:
                    currentTex = LV3Draw();
                    break;
            }
        }

        void WaveMovement()
        {
            int speed = isStop ? ySpeed / 3 : ySpeed;

            pos.Y += speed * Game1.instance.deltaTime;
            yTravelled += MathF.Abs(speed) * Game1.instance.deltaTime;

            int RestrictArea = isStop ? 3 : 10;

            if(yTravelled > RestrictArea)
            {
                yTravelled = 0;
                ySpeed *= -1;
            }
        }

        List<ParallaxLayer> LV1Draw()
        {
            ParallaxLayer l1 = new ParallaxLayer("Boat/LV1/1_seperate_anchor_LV1",Vector2.Zero,0);
            ParallaxLayer l2 = new ParallaxLayer("Boat/LV1/2_seperate_boat_LV1",Vector2.Zero,0);
            ParallaxLayer l3 = new ParallaxLayer("Boat/LV1/3_seperate_front_home_LV1",Vector2.Zero,0);
            ParallaxLayer l4 = new ParallaxLayer("Boat/LV1/4_seperate_home_LV1",Vector2.Zero,0);

            return new List<ParallaxLayer> 
            { 
                l4, 
                l3, 
                l2, 
                l1, 
            };
        }

        List<ParallaxLayer> LV2Draw()
        {
            ParallaxLayer l1 = new ParallaxLayer("Boat/LV2/1_seperate_anchor_LV2",Vector2.Zero,0);
            ParallaxLayer l2 = new ParallaxLayer("Boat/LV2/2_seperate_boat_LV2", Vector2.Zero, 0);
            ParallaxLayer l3 = new ParallaxLayer("Boat/LV2/3_seperate_front_sailing_LV2", Vector2.Zero, 0);
            ParallaxLayer l4 = new ParallaxLayer("Boat/LV2/4_seperate_sailing_LV2", Vector2.Zero, 0);
            ParallaxLayer l5 = new ParallaxLayer("Boat/LV2/5_seperate_things_LV2", Vector2.Zero, 0);

            return new List<ParallaxLayer> 
            { 
                l5,
                l4, 
                l3, 
                l2, 
                l1, 
            };
        }

        List<ParallaxLayer> LV3Draw()
        {
            ParallaxLayer l1 = new ParallaxLayer("Boat/LV3/1_seperate_anchor_LV3",Vector2.Zero,0);
            ParallaxLayer l2 = new ParallaxLayer("Boat/LV3/2_seperate_boat_LV3", Vector2.Zero, 0);
            ParallaxLayer l3 = new ParallaxLayer("Boat/LV3/3_seperate_front_sailing_LV3", Vector2.Zero, 0);
            ParallaxLayer l4 = new ParallaxLayer("Boat/LV3/4_seperate_front_home_LV3", Vector2.Zero, 0);
            ParallaxLayer l5 = new ParallaxLayer("Boat/LV3/5_seperate_home_LV3", Vector2.Zero, 0);
            ParallaxLayer l6 = new ParallaxLayer("Boat/LV3/6_seperate_sailing_LV3", Vector2.Zero, 0);

            return new List<ParallaxLayer> 
            { 
                l6,
                l5, 
                l4, 
                l3, 
                l2, 
                l1, 
            };
        }

        void TestBoat()
        {
            if (Game1.instance.Input.IsKeyPressed(Keys.L))
                boatLevel++;
            else if (Game1.instance.Input.IsKeyPressed(Keys.O))
                boatLevel--;
        }

        public void OnCollision(CollisionEventArgs collisionInfo) => Collider.RegisterCollision(collisionInfo.Other);

        public void OnCollisionEnter(ICollisionActor other)
        {
            if(other is ShopLayer && Game1.instance.autoStopAtShop)
            {
                Game1.instance.sceneState = Scene.Default_Stop;
            }
        }
    }
}

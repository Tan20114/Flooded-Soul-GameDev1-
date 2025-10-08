using Flooded_Soul.System.BG;
using Flooded_Soul.System.Collision_System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Color = Microsoft.Xna.Framework.Color;
using Keys = Microsoft.Xna.Framework.Input.Keys;
using RectangleF = MonoGame.Extended.RectangleF;

namespace Flooded_Soul.System.Shop
{
    public class ShopLayer : ParallaxLayer , ICollisionActor
    {
        CollisionTracker Collider;

        public Vector2 pos;

        bool isSpawned = false;

        RectangleF bound;
        public IShapeF Bounds => bound;

        float shopSpawnTime = 2f;
        float elasped = 0;

        public ShopLayer(string path, Vector2 posOffset, int speed,int count , int frameWidth, int frameHeight) : base(path, posOffset, speed, 1, frameWidth,frameHeight)
        {
            Collider = new CollisionTracker();

            isStop = true;

            Collider.CollisionEnter += OnCollisionEnter;
            Collider.CollisionExit += OnCollisionExit;

            texture = Game1.instance.Content.Load<Texture2D>(path);

            pos = new Vector2(posOffset.X, posOffset.Y);

            bound = new RectangleF(.59f * texture.Width * scaleX + posOffset.X, posOffset.Y, .27f * texture.Width * scaleX, texture.Height * scaleY);

            Game1.instance.collisionComponent.Insert(this);
        }

        public void Update(GameTime gt, int speed)
        {
            if(BiomeSystem.type != BiomeType.Ocean) return;

            SpawnShop();

            if (pos.X + texture.Width * scaleX < 0)
                ResetShop();

            if (!isSpawned) return;

            if (Game1.instance.sceneState == Scene.Default)
                isStop = false;
            else
                isStop = true;

            if (isStop) return;

            pos -= new Vector2(speed * Game1.instance.deltaTime * 1.5f, 0);
            bound.Position -= new Vector2(speed * Game1.instance.deltaTime*1.5f,0);

            Collider.Update();
        }

        public void Draw(int i) => Game1.instance._spriteBatch.Draw(
                    texture,
                    pos,
                    null,
                    Color.White,
                    0f,
                    Vector2.Zero,
                    new Vector2(scaleX, scaleY),
                    SpriteEffects.None,
                    0f
                );

        public void OnCollision(CollisionEventArgs collisionInfo) => Collider.RegisterCollision(collisionInfo.Other);

        void OnCollisionEnter(ICollisionActor other)
        {
            if (other is Player && Game1.instance.autoStopAtShop)
            {
                TogglePause();
                Game1.instance.player.stopAtShop = true;
            }
            else if (other is Player)
            {
                Game1.instance.dui.showShop = true;
            }
        }

        void OnCollisionExit(ICollisionActor other)
        {
            if (other is Player)
            {
                Game1.instance.dui.showShop = false;
            }
        }

        public void ResetShop()
        {
            pos = new Vector2(posOffset.X, posOffset.Y);
            isSpawned = false;
            isStop = true;
            Game1.instance.dui.showShop = false;
            bound.Position = new Vector2(.59f * texture.Width * scaleX + posOffset.X, posOffset.Y);
            elasped = 0;
        }

        void SpawnShop()
        {
            if (isSpawned) return;

            Random r = new Random();

            elasped += Game1.instance.deltaTime;

            if (elasped >= shopSpawnTime)
            {
                elasped = 0;
                isSpawned = r.Next(0,2) == 0 ? true:false;
            }
        }
    }
}

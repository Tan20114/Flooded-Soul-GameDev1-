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
using Flooded_Soul.System.Fishing;

namespace Flooded_Soul.System
{
    public class Player : ICollisionActor
    {
        CollisionTracker Collider;

        int minSpeed = 0;
        public int maxSpeed => Game1.instance.speed;

        public int speed;
        public int Speed
        {
            get => speed;
            set => speed = MathHelper.Clamp(value, minSpeed, maxSpeed);
        }

        RectangleF bound;

        public IShapeF Bounds => bound;

        public int distanceTraveled;
        public int fishPoint;

        bool isStop = false;

        int hookLevel = 0;
        public int HookLevel
        {
            get => hookLevel;
            set => hookLevel = MathHelper.Clamp(value, 0, 3);
        }
        int boatLevel = 1;
        public int BoatLevel
        {
            get => boatLevel;
            set => boatLevel = MathHelper.Clamp(value, 1, 3);
        }
        int catlevel = 0;
        public int CatLevel
        {
            get => catlevel;
            set => catlevel = MathHelper.Clamp(value, 0, 3);
        }
        public Dictionary<int,bool> hasCat = new Dictionary<int, bool>()
        {
            { 1,false },
            { 2,false },
            { 3,false },
        };
        int fishPerSec = 0;
        float getFistTime = 5;
        float getFishElasped = 0;

        int ySpeed = 5;
        float yTravelled = 0;

        List<ParallaxLayer> currentTex = new List<ParallaxLayer>();
        Dictionary<string, AnimatedLayer> boatAnims = new Dictionary<string, AnimatedLayer>();
        Dictionary<string, AnimatedLayer> playerLayer = new Dictionary<string, AnimatedLayer>();
        Dictionary<string, ParallaxLayer> catLayer = new Dictionary<string, ParallaxLayer>();
        #region BoatAnim
        AnimatedLayer sail1;
        AnimatedLayer stop1;
        AnimatedLayer sail2;
        AnimatedLayer stop2;
        AnimatedLayer sail3;
        AnimatedLayer stop3;
        #endregion
        #region Cat Layer
        ParallaxLayer cat0_1;
        ParallaxLayer cat0_2;
        ParallaxLayer cat0_3;
        ParallaxLayer cat1_2;
        ParallaxLayer cat1_3;
        ParallaxLayer cat2_3;
        ParallaxLayer cat3_3;
        #endregion
        #region Player
        AnimatedLayer player1;
        AnimatedLayer player2;
        AnimatedLayer player3;
        #endregion
        bool isReload = false;
        AnimatedLayer currentAnim;
        AnimatedLayer playerAnim;

        Vector2 pos;

        public bool stopAtShop = false;

        public Player()
        {
            Collider = new CollisionTracker();

            Collider.CollisionEnter += OnCollisionEnter;

            speed = maxSpeed;

            distanceTraveled = 0;
            fishPoint = 0;

            Texture2D tex = Game1.instance.Content.Load<Texture2D>("Boat/LV1/2_seperate_boat_LV1");
            float scaleX = Game1.instance.viewPortWidth / tex.Width;
            float scaleY = Game1.instance.viewPortHeight / tex.Height;
            bound = new RectangleF(.13f * tex.Width * scaleX, 0, .05f * tex.Width, Game1.instance.viewPortHeight);

            pos = Vector2.Zero;

            LoadBoat();
            LoadCat();
            LoadPlayer();

            currentAnim = boatAnims["sail1"];
            playerAnim = playerLayer["player1"];

            Game1.instance.collisionComponent.Insert(this);
        }

        public void Update(KeyboardState ks, GameTime gameTime)
        {
            CatFishPerSecond();
            LevelVisualize(gameTime);
            WaveMovement();
            Collider.Update();
            UpdateCurrentAnimation();
            ReloadStopAnim();
            currentAnim.Update(gameTime);
            playerAnim.Update(gameTime);

            if (!isStop)
                distanceTraveled += (int)(Speed * (float)gameTime.ElapsedGameTime.TotalSeconds);
        }

        public void Draw()
        {
            for (int i = 0; i < currentTex.Count; i++)
            {
                currentTex[i].Draw(pos);
            }
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

        void LoadBoat()
        {
            #region BoatAnim
            sail1 = new AnimatedLayer("Boat/LV1/Wave_boat_LV1_idle", Vector2.Zero, 0, 1, "sail", 1280, 230, 7, .003f * Speed, true);
            stop1 = new AnimatedLayer("Boat/LV1/Wave_boat_LV1_stop", Vector2.Zero, 0, 1, "stop", 1280, 230, 4, .003f * Speed, false);
            sail2 = new AnimatedLayer("Boat/LV2/Wave_boat_LV2_idle", Vector2.Zero, 0, 1, "sail", 1280, 230, 7, .003f * Speed, true);
            stop2 = new AnimatedLayer("Boat/LV2/Wave_boat_LV2_stop", Vector2.Zero, 0, 1, "stop", 1280, 230, 3, .003f * Speed, false);
            sail3 = new AnimatedLayer("Boat/LV3/Wave_boat_LV3_idle", Vector2.Zero, 0, 1, "sail", 1280, 230, 7, .003f * Speed, true);
            stop3 = new AnimatedLayer("Boat/LV3/Wave_boat_LV3_stop", Vector2.Zero, 0, 1, "stop", 1280, 230, 3, .003f * Speed, false);

            boatAnims["sail1"] = sail1;
            boatAnims["stop1"] = stop1;
            boatAnims["sail2"] = sail2;
            boatAnims["stop2"] = stop2;
            boatAnims["sail3"] = sail3;
            boatAnims["stop3"] = stop3;
            #endregion
        }

        void LoadCat()
        {
            cat0_1 = new ParallaxLayer("Boat/LV1/lv1_whitecat_on_boat", Vector2.Zero, 0, 1);
            cat0_2 = new ParallaxLayer("Boat/LV2/lv2_whitecat_on_boat", Vector2.Zero, 0, 1);
            cat0_3 = new ParallaxLayer("Boat/LV3/lv3_whitecat_on_boat", Vector2.Zero, 0, 1);
            cat1_2 = new ParallaxLayer("Boat/LV2/lv2_calicocat_on_boat", Vector2.Zero, 0, 1);
            cat1_3 = new ParallaxLayer("Boat/LV3/lv3_calicocat_on_boat", Vector2.Zero, 0, 1);
            cat2_3 = new ParallaxLayer("Boat/LV3/lv3_greycat_on_boat", Vector2.Zero, 0, 1);
            cat3_3 = new ParallaxLayer("Boat/LV3/lv3_orangecat_on_boat", Vector2.Zero, 0, 1);

            catLayer["cat0_1"] = cat0_1;
            catLayer["cat0_2"] = cat0_2;
            catLayer["cat0_3"] = cat0_3;
            catLayer["cat1_2"] = cat1_2;
            catLayer["cat1_3"] = cat1_3;
            catLayer["cat2_3"] = cat2_3;
            catLayer["cat3_3"] = cat3_3;
        }

        void LoadPlayer()
        {
            player1 = new AnimatedLayer("Boat/LV1/lv1_player_on_boat-Sheet-export", Vector2.Zero, 0, 1, "Idle", 1280, 230, 4, .3f);
            player2 = new AnimatedLayer("Boat/LV2/lv2_player_on_boat-Sheet-export", Vector2.Zero, 0, 1, "Idle", 1280, 230, 4, .3f);
            player3 = new AnimatedLayer("Boat/LV3/lv3_player_on_boat-Sheet-export", Vector2.Zero, 0, 1, "Idle", 1280, 230, 4, .3f);

            playerLayer["player1"] = player1;
            playerLayer["player2"] = player2;
            playerLayer["player3"] = player3;
        }

        public int GetDistance() => distanceTraveled;

        void LevelVisualize(GameTime gt)
        {
            switch (boatLevel)
            {
                case 1:
                    currentTex = LV1Draw(gt);
                    break;
                case 2:
                    currentTex = LV2Draw(gt);
                    break;
                case 3:
                    currentTex = LV3Draw(gt);
                    break;
            }
        }

        void CatFishPerSecond()
        {
            if (hasCat[1] && BoatLevel >= 2)
                fishPerSec = 1;
            if (hasCat[2] && BoatLevel >= 3)
                fishPerSec = 2;
            if (hasCat[3] && BoatLevel >= 3)
                fishPerSec = 3;

            if (fishPerSec <= 0) return;

            getFishElasped += Game1.instance.deltaTime;

            if (getFishElasped >= getFistTime)
            {
                AudioManager.Instance.PlaySfx("point_up");
                getFishElasped = 0;
                fishPoint += fishPerSec;
                FishPoint.Spawn(fishPerSec, new Vector2(.3f * Game1.instance.viewPortWidth, .4f * Game1.instance.viewPortHeight));
            }
        }

        void WaveMovement()
        {
            int speed = isStop ? ySpeed / 3 : ySpeed;

            pos.Y += speed * Game1.instance.deltaTime;
            yTravelled += MathF.Abs(speed) * Game1.instance.deltaTime;

            int RestrictArea = isStop ? 3 : 10;

            if (yTravelled > RestrictArea)
            {
                yTravelled = 0;
                ySpeed *= -1;
            }
        }

        public void UpdateCurrentAnimation()
        {   
            string state = isStop ? $"stop{BoatLevel}" : $"sail{BoatLevel}";
            string player = $"player{BoatLevel}";

            if (!isStop)
                isReload = false;

            currentAnim = boatAnims[state];
            playerAnim = playerLayer[player];
        }

        public void ReloadStopAnim()
        {
            if (isReload) return;

            boatAnims[$"stop{BoatLevel}"] = null;

            boatAnims[$"stop{BoatLevel}"] = new AnimatedLayer($"Boat/LV{BoatLevel}/Wave_boat_LV{BoatLevel}_stop", Vector2.Zero, 0, 1, "stop", 1280, 230, BoatLevel == 1 ? 4 : 3, .003f * Speed, false);

            isReload = true;
        }

        List<ParallaxLayer> LV1Draw(GameTime gt)
        {
            ParallaxLayer l1 = new ParallaxLayer("Boat/LV1/1_seperate_anchor_LV1", Vector2.Zero, 0);
            ParallaxLayer l2 = new ParallaxLayer("Boat/LV1/2_seperate_boat_LV1", Vector2.Zero, 0);
            ParallaxLayer l3 = new ParallaxLayer("Boat/LV1/3_seperate_front_home_LV1", Vector2.Zero, 0);
            ParallaxLayer l4 = new ParallaxLayer("Boat/LV1/4_seperate_home_LV1", Vector2.Zero, 0);

            return new List<ParallaxLayer>
            {
                l4,
                l3,
                catLayer["cat0_1"],
                playerAnim,
                l2,
                currentAnim,
                l1,
            };
        }

        List<ParallaxLayer> LV2Draw(GameTime gt)
        {
            ParallaxLayer l1 = new ParallaxLayer("Boat/LV2/1_seperate_anchor_LV2", Vector2.Zero, 0);
            ParallaxLayer l2 = new ParallaxLayer("Boat/LV2/2_seperate_boat_LV2", Vector2.Zero, 0);
            ParallaxLayer l3 = new ParallaxLayer("Boat/LV2/3_seperate_front_sailing_LV2", Vector2.Zero, 0);
            ParallaxLayer l4 = new ParallaxLayer("Boat/LV2/4_seperate_sailing_LV2", Vector2.Zero, 0);
            ParallaxLayer l5 = new ParallaxLayer("Boat/LV2/5_seperate_things_LV2", Vector2.Zero, 0);

            List<ParallaxLayer> layers = new List<ParallaxLayer>()
            {
                l5,
                l4,
                l3,
                catLayer["cat0_2"],
                playerAnim,
            };

            if (hasCat[1])
                layers.Add(catLayer["cat1_2"]);

            layers.Add(l2);
            layers.Add(currentAnim);
            layers.Add(l1);

            return layers;
        }

        List<ParallaxLayer> LV3Draw(GameTime gt)
        {
            ParallaxLayer l1 = new ParallaxLayer("Boat/LV3/1_seperate_anchor_LV3", Vector2.Zero, 0);
            ParallaxLayer l2 = new ParallaxLayer("Boat/LV3/2_seperate_boat_LV3", Vector2.Zero, 0);
            ParallaxLayer l3 = new ParallaxLayer("Boat/LV3/3_seperate_front_sailing_LV3", Vector2.Zero, 0);
            ParallaxLayer l4 = new ParallaxLayer("Boat/LV3/4_seperate_front_home_LV3", Vector2.Zero, 0);
            ParallaxLayer l5 = new ParallaxLayer("Boat/LV3/5_seperate_home_LV3", Vector2.Zero, 0);
            ParallaxLayer l6 = new ParallaxLayer("Boat/LV3/6_seperate_sailing_LV3", Vector2.Zero, 0);

            List<ParallaxLayer> layers = new List<ParallaxLayer>()
            {
                l6,
                l5,
                l4,
                l3,
                catLayer["cat0_3"],
                playerAnim,
            };

            if (hasCat[1])
                layers.Add(catLayer["cat1_3"]);

            if (hasCat[2])
                layers.Add(catLayer["cat2_3"]);

            if (hasCat[3])
                layers.Add(catLayer["cat3_3"]);

            layers.Add(l2);
            layers.Add(currentAnim);
            layers.Add(l1);

            return layers;
        }

        public void OnCollision(CollisionEventArgs collisionInfo) => Collider.RegisterCollision(collisionInfo.Other);

        public void OnCollisionEnter(ICollisionActor other)
        {
            if (other is ShopLayer && Game1.instance.autoStopAtShop)
            {
                Game1.instance.sceneState = Scene.Default_Stop;
            }
        }
    }
}

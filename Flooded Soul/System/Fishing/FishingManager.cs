using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Flooded_Soul.System.Fishing.Fishes;
using Flooded_Soul.System.Collision_System;

namespace Flooded_Soul.System.Fishing
{
    public enum FishType { Normal, Uncommon, Rare, Legend }

    public class FishingManager
    {
        Hook hook;
        public bool isMinigame = false;
        public Fish targetFish;
        public List<Fish> otherFishes = new List<Fish>();
        public FishingGameArea minigameArea;
        public FishPointer mousePos;

        Random random = new Random();

        Queue<Normal> normalPool = new Queue<Normal>();
        Queue<Uncommon> uncommonPool = new Queue<Uncommon>();
        Queue<Rare> rarePool = new Queue<Rare>();
        Queue<Legend> legendPool = new Queue<Legend>();

        public List<Fish> fishInScreen = new List<Fish>();

        int maxFish = 8;
        float fishScale = 4.5f;

        public int nCount = 5, rCount = 5, uCount = 5, lCount = 5;
        int maxNCount, maxRCount, maxUCount, maxLCount;

        public bool isPause = false;
        float catchLine => Game1.instance.viewPortHeight * 0.15f + Game1.instance.viewPortHeight;

        public FishingManager()
        {
            hook = new Hook(this);
            Game1.instance.collisionComponent.Insert(hook);

            mousePos = new FishPointer(this);
            FishPoint.Initialize();
            InitializePool();
        }

        public void Update()
        {
            if (isPause) return;

            hook.Update();
            FishPoint.UpdateAll();

            if (Game1.instance.sceneState != Scene.Fishing && SceneManager.moveSuccess)
            {
                if (isMinigame)
                    EndMinigame(false);

                hook.ResetPosition();
                return;
            }

            foreach (Fish fish in fishInScreen.ToArray())
            {
                if (fish.IsActive)
                {
                    fish.Update();
                    fish.vision.Update();
                }
            }

            StartMinigame();
            Minigame();
        }

        public void Draw()
        {
            hook.Draw();

            FishPoint.DrawAll();

            foreach (Fish fish in fishInScreen)
                if (fish.IsActive) fish.Draw();

            if (isMinigame)
                minigameArea?.Draw();
        }

        #region Pool & Spawn
        void InitializePool()
        {
            for (int i = 0; i < 10; i++)
                normalPool.Enqueue(new Normal(NormalFishPool[random.Next(NormalFishPool.Count)], fishScale, this));
            for (int i = 0; i < 5; i++)
                uncommonPool.Enqueue(new Uncommon(UncommonFishPool, fishScale, this));
            for (int i = 0; i < 3; i++)
                rarePool.Enqueue(new Rare(RareFishPool, fishScale, this));
            for (int i = 0; i < 2; i++)
                legendPool.Enqueue(new Legend(LegendFishPool, fishScale, this));
        }

        public void EnterSea()
        {
            RandomFishPossibilities();

            nCount = maxNCount; 
            uCount = maxUCount; 
            rCount = maxRCount; 
            lCount = maxLCount;

            for (int i = 0; i < maxNCount; i++) FishSpawn(FishType.Normal);
            for (int i = 0; i < maxUCount; i++) FishSpawn(FishType.Uncommon);
            for (int i = 0; i < maxRCount; i++) FishSpawn(FishType.Rare);
            for (int i = 0; i < maxLCount; i++) FishSpawn(FishType.Legend);
        }

        public void ExitSea()
        {
            EndMinigame(false);

            foreach (Fish fish in fishInScreen.ToArray())
                ReleaseFish(fish, false);

            fishInScreen.Clear();
            targetFish = null;
            otherFishes.Clear();
        }

        Fish FishSpawn(FishType type)
        {
            Fish fish = type switch
            {
                FishType.Normal => GetNormal(),
                FishType.Uncommon => GetUncommon(),
                FishType.Rare => GetRare(),
                FishType.Legend => GetLegend(),
                _ => null
            };
            return fish;
        }

        Normal GetNormal()
        {
            Normal f = normalPool.Count > 0 ? normalPool.Dequeue() :
                       new Normal(NormalFishPool[random.Next(NormalFishPool.Count)], fishScale, this);
            f.Reset(NormalFishPool[random.Next(NormalFishPool.Count)],this);
            fishInScreen.Add(f);
            return f;
        }

        Uncommon GetUncommon()
        {
            Uncommon f = uncommonPool.Count > 0 ? uncommonPool.Dequeue() :
                         new Uncommon(UncommonFishPool, fishScale, this);
            f.Reset(UncommonFishPool, this);
            fishInScreen.Add(f);
            return f;
        }

        Rare GetRare()
        {
            Rare f = rarePool.Count > 0 ? rarePool.Dequeue() :
                     new Rare(RareFishPool, fishScale, this);
            f.Reset(RareFishPool, this);
            fishInScreen.Add(f);
            return f;
        }

        Legend GetLegend()
        {
            Legend f = legendPool.Count > 0 ? legendPool.Dequeue() :
                       new Legend(LegendFishPool, fishScale, this);
            f.Reset(LegendFishPool, this);
            fishInScreen.Add(f);
            return f;
        }

        void ReleaseFish(Fish fish, bool success)
        {
            if (fish == null) return;

            fish.Destroy(success);
            fishInScreen.Remove(fish);

            switch (fish)
            {
                case Normal n: normalPool.Enqueue(n); break;
                case Uncommon u: uncommonPool.Enqueue(u); break;
                case Rare r: rarePool.Enqueue(r); break;
                case Legend l: legendPool.Enqueue(l); break;
            }

            if (targetFish == fish) targetFish = null;
        }
        #endregion

        #region Minigame
        void StartMinigame()
        {
            if (isMinigame || targetFish == null || !targetFish.IsActive) return;

            targetFish.speed *= 2f;

            otherFishes = fishInScreen.Where(f => f != targetFish && f.IsActive).ToList();
            foreach (Fish other in otherFishes)
            {
                other.Collider.DisableCollision();
                other.vision.Collider.DisableCollision();
            }

            Game1.instance.collisionComponent.Insert(mousePos);
            minigameArea = new FishingGameArea(targetFish.pos, this);
            isMinigame = true;
        }

        void Minigame()
        {
            if (!isMinigame || targetFish == null || !targetFish.IsActive) return;

            minigameArea.Update();
            mousePos.Update();

            if (!minigameArea.fishInArea)
            {
                EndMinigame(false);
                return;
            }

            if (targetFish.pos.Y <= catchLine)
            {
                EndMinigame(true);
                return;
            }

            if (mousePos.canClick && Game1.instance.Input.IsLeftMouse())
            {
                int targetY = (int)(targetFish.pos.Y - (hook.hookUpSpeed / targetFish.Strength));
                targetFish.isClicked = true;
                targetFish.MoveToY(targetY);
                targetFish.OnAlert();
            }
        }

        void EndMinigame(bool success)
        {
            if (!isMinigame) return;

            isMinigame = false;
            Debug.WriteLine(success ? "🎉 Caught the fish!" : "❌ The fish escaped...");

            foreach (Fish other in otherFishes)
            {
                if (other.IsActive)
                {
                    other.Collider.EnableCollision();
                    other.vision.Collider.EnableCollision();
                }
            }

            Game1.instance.collisionComponent.Remove(mousePos);
            minigameArea = null;
            mousePos.canClick = false;

            targetFish.EndMinigame();
            ReleaseFish(targetFish, success);
            targetFish = null;

            hook.ResetPosition();
            hook.Collider.EnableCollision();
            otherFishes.Clear();
        }
        #endregion

        #region Utilities
        void RandomFishPossibilities()
        {
            foreach (Fish fish in fishInScreen.ToArray())
                ReleaseFish(fish, false);

            maxLCount = 0; maxRCount = 0; maxUCount = 0; maxNCount = 0;

            for (int i = 0; i < maxFish; i++)
            {
                int ranVal = random.Next(1, 101);
                if (ranVal <= 2) maxLCount++;
                else if (ranVal <= 20) maxRCount++;
                else if (ranVal <= 50) maxUCount++;
                else maxNCount++;
            }
        }
        #endregion

        #region Fish Pools (paths)
        List<string> NormalFishPool => new List<string> {
            "Fish/common_mudFish_AllBiome",
            "Fish/common_sacabambaspisFish_AllBiome",
            BiomeSystem.type switch
            {
                BiomeType.Ocean => "Fish/common_Wavefinfish_ocean",
                BiomeType.Ice => "Fish/common_Icefin_snow",
                BiomeType.Forest => "Fish/common_HydrascaleFish_forest"
            }
        };

        string UncommonFishPool => BiomeSystem.type switch
        {
            BiomeType.Ocean => "Fish/uncommon_dogFish_ocean",
            BiomeType.Ice => "Fish/uncommon_frostPetalFish_snow",
            BiomeType.Forest => "Fish/uncommon_goofslimeFish_forest"
        };

        string RareFishPool => BiomeSystem.type switch
        {
            BiomeType.Ocean => "Fish/rare_Seaturtle_ocean",
            BiomeType.Ice => "Fish/rare_clingFish_snow",
            BiomeType.Forest => "Fish/rare_red Jelllyfish_forest"
        };

        string LegendFishPool => BiomeSystem.type switch
        {
            BiomeType.Ocean => "Fish/legend_plabFish_ocean",
            BiomeType.Ice => "Fish/legend_jollyFish_snow",
            BiomeType.Forest => "Fish/legend_kelpboneFish_forest"
        };
        #endregion
    }
}

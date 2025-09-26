using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Flooded_Soul.System.Fishing.Fishes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using ButtonState = Microsoft.Xna.Framework.Input.ButtonState;

namespace Flooded_Soul.System.Fishing
{
    public enum FishType
    {
        Normal,
        Uncommon,
        Rare,
        Legend
    }

    public class FishingManager
    {
        Hook hook;

        public bool isMinigame = false;
        public Fish targetFish;
        public List<Fish> otherFishes = new List<Fish>();
        public Queue<Fish> fishToRemove = new Queue<Fish>();
        public FishingGameArea minigameArea;
        public FishPointer mousePos;

        List<string> NormalFishPool
        {
            get
            {
                List<string> normalFishPath = new List<string>() 
                {
                    "Fish/common_mudFish_AllBiome",
                    "Fish/common_sacabambaspisFish_AllBiome",
                };

                switch (BiomeSystem.type)
                {
                    case BiomeType.Ocean:
                        normalFishPath.Add("Fish/common_Wavefinfish_ocean");
                        break;
                    case BiomeType.Ice:
                        normalFishPath.Add("Fish/common_Icefin_snow");
                        break;
                    case BiomeType.Forest:
                        normalFishPath.Add("Fish/common_HydrascaleFish_forest");
                        break;
                }
                return normalFishPath;
            }
        }

        string UncommonFishPool
        {
            get
            {
                string path = "";

                switch (BiomeSystem.type)
                {
                    case BiomeType.Ocean:
                        path = "Fish/uncommon_dogFish_ocean";
                        break;
                    case BiomeType.Ice:
                        path = "Fish/uncommon_frostPetalFish_snow";
                        break;
                    case BiomeType.Forest:
                        path = "Fish/uncommon_goofslimeFish_forest";
                        break;
                }
                return path;
            }
        }

        string RareFishPool
        {
            get
            {
                string path = "";

                switch (BiomeSystem.type)
                {
                    case BiomeType.Ocean:
                        path = "Fish/rare_Seaturtle_ocean";
                        break;
                    case BiomeType.Ice:
                        path = "Fish/rare_clingFish_snow";
                        break;
                    case BiomeType.Forest:
                        path = "Fish/rare_red Jelllyfish_forest";
                        break;
                }
                return path;
            }
        }

        string LegendFishPool
        {
            get
            {
                string path = "";

                switch (BiomeSystem.type)
                {
                    case BiomeType.Ocean:
                        path = "Fish/legend_plabFish_ocean";
                        break;
                    case BiomeType.Ice:
                        path = "Fish/legend_jollyFish_snow";
                        break;
                    case BiomeType.Forest:
                        path = "Fish/legend_kelpboneFish_forest";
                        break;
                }
                return path;
            }
        }

        int maxFish = 8;
        float fishScale = 4.5f;

        public int nCount = 5;
        int maxNCount = 0;
        public int rCount = 5;
        int maxRCount = 0;
        public int uCount = 5;
        int maxUCount = 0;
        public int lCount = 5;
        int maxLCount = 0;

        public int bonus = 0;
        bool plus1 = false;
        bool plus2 = false;
        bool plus3 = false;

        public List<Fish> fishInScreen = new List<Fish>();

        public bool isPause = false;
        float catchLine = Game1.instance.viewPortHeight * 0.15f + Game1.instance.viewPortHeight;

        public FishingManager()
        {
            hook = new Hook(this);
            Game1.instance.collisionComponent.Insert(hook);

            mousePos = new FishPointer(this);
        }

        public void Update()
        {
            if (isPause) return;

            UpdateFishBonus();

            hook.Update();
            if (Game1.instance.sceneState != Scene.Fishing && SceneManager.moveSuccess)
            {
                if (isMinigame)
                {
                    isMinigame = false;
                    minigameArea = null;
                    mousePos.canClick = false;
                    targetFish = null;
                }
                hook.ResetPosition();
                return;
            }
            foreach (Fish fish in fishInScreen)
            {
                fish.Update();
                fish.vision.Update();
            }

            StartMinigame();
            Minigame();
        }

        public void Draw()
        {
            hook.Draw();
            foreach (Fish fish in fishInScreen)
                fish.Draw();
            if (isMinigame)
                minigameArea.Draw();
        }

        public void EnterSea()
        {
            RandomFishPossibilities();

            nCount = maxNCount;
            rCount = maxRCount;
            uCount = maxUCount;
            lCount = maxLCount;

            for (int i = 0; i < maxNCount; i++)
                FishSpawn(FishType.Normal);
            for (int i = 0; i < maxUCount; i++)
                FishSpawn(FishType.Uncommon);
            for (int i = 0; i < maxRCount; i++)
                FishSpawn(FishType.Rare);
            for (int i = 0; i < maxLCount; i++)
                FishSpawn(FishType.Legend);
        }

        public void ExitSea()
        {
            targetFish = null;
            otherFishes.Clear();

            if (isMinigame)
                EndMinigame(false);

            var fishesToProcess = new List<Fish>(fishInScreen);

            foreach (var fish in fishesToProcess)
            {
                fish.Destroy(false);
                fishToRemove.Enqueue(fish);
            }

            fishInScreen.Clear();
        }

        void StartMinigame()
        {
            if (isMinigame) return;


            if (targetFish != null)
            {
                targetFish.speed *= 2f;
                foreach (Fish other in otherFishes)
                {
                    other.Collider.DisableCollision();
                    other.vision.Collider.DisableCollision();
                }

                Game1.instance.collisionComponent.Insert(mousePos);

                minigameArea = new FishingGameArea(targetFish.pos, this);
                isMinigame = true;
            }
        }

        void Minigame()
        {
            int targetY = 0;
            if (isMinigame)
            {
                minigameArea.Update();
                mousePos.Update();

                if (!minigameArea.fishInArea)
                    EndMinigame(false);
                else if (targetFish.pos.Y <= catchLine)
                    EndMinigame(true);
                else
                {
                    if (mousePos.canClick)
                        if (Game1.instance.Input.IsLeftMouse())
                        {
                            targetY = (int)(targetFish.pos.Y - (int)(hook.hookUpSpeed / targetFish.Strength));
                            targetFish.isClicked = true;

                            targetFish.MoveToY(targetY);

                            targetFish.OnAlert();
                        }
                }
            }
        }

        void EndMinigame(bool success = false)
        {
            if (!isMinigame) return;

            isMinigame = false;
            Debug.WriteLine(success ? "🎉 Caught the fish!" : "❌ The fish escaped...");

            foreach (Fish other in otherFishes)
            {
                other.Collider.EnableCollision();
                other.vision.Collider.EnableCollision();
            }

            Game1.instance.collisionComponent.Remove(mousePos);
            minigameArea = null;
            mousePos.canClick = false;

            if (success)
            {
                targetFish.Destroy(success);
                hook.ResetPosition();
                hook.Collider.EnableCollision();
            }
            else
            {
                targetFish?.Destroy(false);
                hook.Collider.EnableCollision();
            }

            targetFish = null;
            otherFishes.Clear();
        }

        void FishSpawn(FishType type)
        {
            switch (type)
            {
                case FishType.Normal:
                    Random r = new Random();

                    int ranVal = r.Next(0, NormalFishPool.Count);

                    Fish normalFish = new Normal(NormalFishPool[ranVal], fishScale, this);
                    fishInScreen.Add(normalFish);
                    break;
                case FishType.Uncommon:
                    Fish uncommonFish = new Uncommon(UncommonFishPool, fishScale, this);
                    fishInScreen.Add(uncommonFish);
                    break;
                case FishType.Rare:
                    Fish rareFish = new Rare(RareFishPool, fishScale, this);
                    fishInScreen.Add(rareFish);
                    break;
                case FishType.Legend:
                    Fish legendFish = new Legend(LegendFishPool, fishScale, this);
                    fishInScreen.Add(legendFish);
                    break;
            }
        }

        public void RandomFishPossibilities()
        {
            fishInScreen.Clear();

            maxLCount = 0;
            maxRCount = 0;
            maxUCount = 0;
            maxNCount = 0;

            Random random = new Random();

            for (int i = 0; i < maxFish; i++)
            {
                int ranVal = random.Next(1, 101);

                if (ranVal <= 2)
                    maxLCount++;
                else if (ranVal <= 20)
                    maxRCount++;
                else if (ranVal <= 50)
                    maxUCount++;
                else
                    maxNCount++;
            }
        }

        void UpdateFishBonus()
        {
            if (Game1.instance.player.hasCat[1] && !plus1)
            {
                bonus++;
                plus1 = true;
            }
            if (Game1.instance.player.hasCat[2] && !plus2)
            {
                bonus++;
                plus2 = true;
            }
            if (Game1.instance.player.hasCat[3] && !plus3)
            {
                bonus++;
                plus3 = true;
            }
        }
    }
}

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
        Rare,
        Legend
    }

    internal class FishingManager
    {
        Hook hook;

        public bool isMinigame = false;
        public Fish targetFish;
        public List<Fish> otherFishes = new List<Fish>();
        public FishingGameArea minigameArea;
        public FishPointer mousePos;

        int maxFish = 6;

        public int nCount = 5;
        int maxNCount = 0;
        public int rCount = 5;
        int maxRCount = 0;
        public int lCount = 5;
        int maxLCount = 0;

        public List<Fish> fishInScreen = new List<Fish>();

        public FishingManager()
        {
            hook = new Hook(this);
            Game1.instance.collisionComponent.Insert(hook);
            
            mousePos = new FishPointer(this);
        }

        public void Update()
        {
            hook.Update();
            if (Game1.instance.sceneState != Scene.Fishing && Game1.instance.scene.moveSuccess)
            {
                if(isMinigame)
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
            {
                minigameArea.Draw();
            }
        }

        public void EnterSea()
        {
            RandomFishPossibilities();

            nCount = maxNCount;
            rCount = maxRCount;
            lCount = maxLCount;

            for (int i = 0; i < maxNCount; i++)
                FishSpawn(FishType.Normal);
            for (int i = 0; i < maxRCount; i++)
                FishSpawn(FishType.Rare);
            for (int i = 0; i < maxLCount; i++)
                FishSpawn(FishType.Legend);
        }

        void StartMinigame()
        {
            if (isMinigame) return;

            if (targetFish != null)
            {
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
                else if(targetFish.pos.Y < Game1.instance.viewPortHeight * .15f + Game1.instance.viewPortHeight)
                    EndMinigame(true);
                else
                {
                    float distance = MathF.Abs(targetFish.pos.Y - targetY);

                    if (distance <= 20)
                        targetFish.isClicked = false;

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
            }
            else
                targetFish.Destroy(success);

            targetFish = null;
            otherFishes.Clear();
        }

        void FishSpawn(FishType type)
        {
            switch(type)
            {
                case FishType.Normal:
                    Fish normalFish = new Normal("Legend_fish", 0.1f, this);
                    fishInScreen.Add(normalFish);
                    break;
                case FishType.Rare:
                    Fish rareFish = new Rare("Legend_fish", .1f, this);
                    fishInScreen.Add(rareFish);
                    break;
                case FishType.Legend:
                    Fish legendFish = new Legend("Legend_fish", .1f, this);
                    fishInScreen.Add(legendFish);
                    break;
            }
        }

        public void RandomFishPossibilities()
        {
            fishInScreen.Clear();

            maxLCount = 0;
            maxRCount = 0;
            maxNCount = 0;

            for(int i = 0; i< maxFish; i++)
            {
                int ranVal = new Random().Next(1, 101);

                if(ranVal <= 5)
                    maxLCount++;
                else if(ranVal > 2 && ranVal <= 40)
                    maxRCount++;
                else
                    maxNCount++;
            }
        }
    }
}

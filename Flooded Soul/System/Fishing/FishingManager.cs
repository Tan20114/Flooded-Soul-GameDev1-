using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Flooded_Soul.System.Fishing.Fishes;
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
        InputHandler Input;

        Hook hook;

        public bool isMinigame = false;
        public Fish targetFish;
        public List<Fish> otherFishes = new List<Fish>();
        public FishingGameArea minigameArea;
        public FishPointer mousePos;

        public int nCount = 5;
        public int rCount = 5;
        public int lCount = 5;

        public List<Fish> fishInScreen = new List<Fish>();

        public FishingManager(int normalCount, int rareCount, int legendCount)
        {
            Input = new InputHandler();

            nCount = normalCount;
            rCount = rareCount;
            lCount = legendCount;

            hook = new Hook(this);
            Game1.instance.collisionComponent.Insert(hook);
            for (int i = 0; i < nCount; i++)
                FishSpawn(FishType.Normal);
            for (int i = 0; i < rCount; i++)
                FishSpawn(FishType.Rare);
            for (int i = 0; i < lCount; i++)
                FishSpawn(FishType.Legend);

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
                ResetAllFish();
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
                mousePos.Draw();
            }
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
            if (isMinigame)
            {
                minigameArea.Update();
                mousePos.Update();

                if (!minigameArea.fishInArea)
                    EndMinigame(false);
                else if(targetFish.pos.Y < Game1.instance.viewPortHeight * .2f + Game1.instance.viewPortHeight)
                {
                    fishInScreen.Remove(targetFish);
                    EndMinigame(true);
                }
                else
                {
                    if (mousePos.canClick)
                        if (Input.IsLeftMouse())
                        {
                            targetFish.pos.Y -= hook.hookUpSpeed;
                            targetFish.speed *= -1;
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
                fishInScreen.Remove(targetFish);
            else
                targetFish.vision.Collider.EnableCollision();

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
                    Fish rareFish = new Rare("Legend_fish", 0.07f, this);
                    fishInScreen.Add(rareFish);
                    break;
                case FishType.Legend:
                    Fish legendFish = new Legend("Legend_fish", 0.05f, this);
                    fishInScreen.Add(legendFish);
                    break;
            }
        }

        void ResetAllFish()
        {
            foreach (Fish fish in fishInScreen)
            {
                fish.Reset();
                fish.Collider.EnableCollision();
                fish.vision.Collider.EnableCollision();
            }
        }
    }
}

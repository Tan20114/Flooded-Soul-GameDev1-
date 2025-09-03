using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Flooded_Soul.System.Fishing.Fishes;

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

        public int nCount = 5;
        public int rCount = 5;
        public int lCount = 5;

        public List<Fish> fishInScreen = new List<Fish>();

        public FishingManager(int normalCount, int rareCount, int legendCount)
        {
            nCount = normalCount;
            rCount = rareCount;
            lCount = legendCount;

            hook = new Hook();
            Game1.instance.collisionComponent.Insert(hook);
            for (int i = 0; i < nCount; i++)
                FishSpawn(FishType.Normal);
            for (int i = 0; i < rCount; i++)
                FishSpawn(FishType.Rare);
            for (int i = 0; i < lCount; i++)
                FishSpawn(FishType.Legend);
        }

        public void Update()
        {
            hook.Update();
            foreach (Fish fish in fishInScreen)
            {
                fish.Update();
                fish.vision.Update();
            }
        }

        public void Draw()
        {
            hook.Draw();
            foreach (Fish fish in fishInScreen)
                fish.Draw();
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
    }
}

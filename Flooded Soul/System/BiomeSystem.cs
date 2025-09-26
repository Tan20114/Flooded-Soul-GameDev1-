using Flooded_Soul.System.BG;
using Microsoft.Xna.Framework;
using System;
using System.Diagnostics;
using Color = Microsoft.Xna.Framework.Color;

namespace Flooded_Soul.System
{
    public enum BiomeType
    {
        Ocean,
        Ice,
        Forest
    }

    internal class BiomeSystem
    {
        float changeInterval = 30f;
        float elapsedTime = 0f;

        public static bool isTransition = false;
        float transitionTime = 5f;
        float freezeTime = 2f;
        float transitionElapsed = 0f;

        bool isRandomized = false;

        AnimatedLayer transitionLayer;
        Vector2 pos;

        bool isStop = false;

        public static BiomeType type;

        public BiomeSystem(Vector2 posOffset)
        {
            type = BiomeType.Ocean; 

            pos = Vector2.Zero;

            transitionLayer = new AnimatedLayer("ParallaxBG/transition_SpriteSheet", posOffset, 0, 1,"trans",1280,230,10);
        }

        public void Update(GameTime gt)
        {
            transitionLayer?.Update(gt);

            if (Game1.instance.sceneState != Scene.Default) return;

            if (isStop) return;

            if (!isTransition)
            {
                elapsedTime += Game1.instance.deltaTime;
                Debug.WriteLine($"{(int)elapsedTime}/{changeInterval}");

                if (elapsedTime > changeInterval)
                {
                    elapsedTime = 0f;
                    isTransition = true;
                    transitionElapsed = 0f;
                }
            }
            else
            {
                transitionElapsed += Game1.instance.deltaTime;

                if (transitionElapsed >= transitionTime)
                {
                    isTransition = false;
                    isRandomized = false;
                    transitionElapsed = 0f;
                }
            }
        }

        public void Draw()
        {
            if (isTransition)
                transitionLayer.Draw(pos, Transition());
        }

        Color Transition()
        {
            float halfTime = (transitionTime - freezeTime) / 2f;
            float t = transitionElapsed;

            if (t < halfTime)
            {                
                float subT = t / halfTime;
                return Color.Lerp(Color.Transparent, Color.White, subT);
            }
            else if (t < halfTime + freezeTime)
            {
                RandomBiome();
                Game1.instance.shop.ResetShop();
                return Color.White;
            }
            else
            {
                float subT = (t - halfTime - freezeTime) / halfTime;
                return Color.Lerp(Color.White, Color.Transparent, subT);
            }
        }

        void RandomBiome()
        {
            if (isRandomized) return;

            Random random = new Random();

            int biomeIndex = 0;

            do
            {
                biomeIndex = random.Next(0, 3);
            } while ((BiomeType)biomeIndex == type);

            switch (biomeIndex)
            {
                case 0:
                {
                    Game1.instance.bg.ChangeBiome(Game1.ocean.overWater);
                    Game1.instance.underSeaBg.ChangeBiome(Game1.ocean.underWater);
                    type = BiomeType.Ocean;
                    break;
                }
                case 1:
                {
                    Game1.instance.bg.ChangeBiome(Game1.ice.overWater);
                    Game1.instance.underSeaBg.ChangeBiome(Game1.ice.underWater);
                    type = BiomeType.Ice;
                    break;
                }
                case 2:
                {
                    Game1.instance.bg.ChangeBiome(Game1.forest.overWater);
                    Game1.instance.underSeaBg.ChangeBiome(Game1.forest.underWater);
                    type = BiomeType.Forest;
                    break;
                }
            }
            isRandomized = true;
        }
    }
}

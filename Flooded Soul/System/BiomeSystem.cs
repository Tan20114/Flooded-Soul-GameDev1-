using Flooded_Soul.System.BG;
using Microsoft.Xna.Framework;
using System;
using Color = Microsoft.Xna.Framework.Color;

namespace Flooded_Soul.System
{
    enum BiomeType
    {
        Ocean,
        Ice,
        Forest
    }

    internal class BiomeSystem
    {
        float changeInterval = 5f;
        float elapsedTime = 0f;

        public static bool isTransition = false;
        float transitionTime = 5f;
        float freezeTime = 2f;
        float transitionElapsed = 0f;

        ParallaxLayer transitionLayer;
        Vector2 pos;

        bool isStop = false;

        BiomeType type;

        public BiomeSystem(Vector2 posOffset)
        {
            type = BiomeType.Ocean; 

            pos = Vector2.Zero;

            transitionLayer = new ParallaxLayer("ParallaxBG/transition_Screen", posOffset, 0, 1);
        }

        public void Update()
        {
            if (Game1.instance.sceneState == Scene.Default_Stop)
                isStop = true;
            else if (Game1.instance.sceneState == Scene.Default)
                isStop = false;

            if (isStop) return;

            if (!isTransition)
            {
                elapsedTime += Game1.instance.deltaTime;

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
            Random random = new Random();

            int biomeIndex = random.Next(0, 3);

            switch (biomeIndex)
            {
                case 0:
                {
                    if (type != BiomeType.Ocean)
                    {
                        Game1.instance.bg.ChangeBiome(Game1.ocean.overWater);
                        Game1.instance.underSeaBg.ChangeBiome(Game1.ocean.underWater);
                        type = BiomeType.Ocean;
                        }
                    break;
                }
                case 1:
                {
                    if (type != BiomeType.Ice)
                    {
                        Game1.instance.bg.ChangeBiome(Game1.ice.overWater);
                        Game1.instance.underSeaBg.ChangeBiome(Game1.ice.underWater);
                        type = BiomeType.Ice;
                        }
                    break;
                }
                case 2:
                {
                    if (type != BiomeType.Forest)
                    {
                        Game1.instance.bg.ChangeBiome(Game1.forest.overWater);
                        Game1.instance.underSeaBg.ChangeBiome(Game1.forest.underWater);
                        type = BiomeType.Forest;
                    }
                    break;
                }
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Color = Microsoft.Xna.Framework.Color;


namespace Flooded_Soul.System.Fishing.Fishes
{
    internal class Rare : Fish
    {
        public Rare(string textureName, float scale, FishingManager manager) : base(textureName, scale, manager)
        {
            Strength = 1.5f;
            speed = 150;
            point = 2;
            test = Color.Green;
        }

        protected override void RandomPos()
        {
            float minSpawnRatio = normalMaxSpawnRatio;
            float maxSpawnRatio = rareMaxSpawnRatio;

            minSpawnHeight = (int)(Game1.instance.viewPortHeight * minSpawnRatio);
            maxSpawnHeight = (int)(Game1.instance.viewPortHeight * maxSpawnRatio);

            pos.X = random.Next(0, Game1.instance.viewPortWidth - (int)(texture.Width * scale));
            pos.Y = random.Next(minSpawnHeight, maxSpawnHeight) + Game1.instance.viewPortHeight;
        }

        public override void Destroy(bool S)
        {
            base.Destroy(S);
            fishingManager.rCount--;
        }
    }
}

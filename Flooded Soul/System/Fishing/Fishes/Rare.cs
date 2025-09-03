using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flooded_Soul.System.Fishing.Fishes
{
    internal class Rare : Fish
    {
        public Rare(string textureName, float scale, FishingManager manager) : base(textureName, scale, manager)
        {

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
    }
}

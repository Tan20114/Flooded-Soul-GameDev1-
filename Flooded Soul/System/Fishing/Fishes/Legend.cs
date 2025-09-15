using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Color = Microsoft.Xna.Framework.Color;


namespace Flooded_Soul.System.Fishing.Fishes
{
    internal class Legend : Fish
    {
        public Legend(string textureName, float scale, FishingManager manager) : base(textureName, scale, manager)
        {
            Strength = 2;
            speed = 200;
            point = 3;
            test = Color.Black;
        }

        protected override void RandomPos()
        {
            float minSpawnRatio = rareMaxSpawnRatio;
            float maxSpawnRatio = legendMaxSpawnRatio;

            minSpawnHeight = (int)(Game1.instance.viewPortHeight * minSpawnRatio);
            maxSpawnHeight = (int)(Game1.instance.viewPortHeight * maxSpawnRatio);

            pos.X = random.Next(0, Game1.instance.viewPortWidth - (int)(texture.Width * scale));
            pos.Y = random.Next(minSpawnHeight, maxSpawnHeight) + Game1.instance.viewPortHeight;
        }

        public override void Destroy(bool Success)
        {
            base.Destroy(Success);
            fishingManager.lCount--;
        }
    }
}

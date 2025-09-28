using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Color = Microsoft.Xna.Framework.Color;

namespace Flooded_Soul.System.Fishing.Fishes
{
    internal class Normal : Fish
    {


        public Normal(string textureName, float scale, FishingManager manager) : base(textureName, scale, manager)
        {
            Strength = 1f;
            initialSpeed = 100;
            point = 1;
        }

        protected override void RandomPos()
        {
            float minSpawnRatio = initialSpawnRatio;
            float maxSpawnRatio = normalMaxSpawnRatio;

            minSpawnHeight = (int)(Game1.instance.viewPortHeight * minSpawnRatio);
            maxSpawnHeight = (int)(Game1.instance.viewPortHeight * maxSpawnRatio);

            pos.X = random.Next(0, Game1.instance.viewPortWidth - (int)(texture.Width * scale));
            pos.Y = random.Next(minSpawnHeight, maxSpawnHeight) + Game1.instance.viewPortHeight;
        }

        public override void Destroy(bool Success)
        {
            base.Destroy(Success);
            fishingManager.nCount--;
        }

        public override void Reset()
        {
            base.Reset();
            speed = initialSpeed;
        }
    }
}

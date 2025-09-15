using Flooded_Soul.System.BG;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flooded_Soul.System.Shop
{
    internal class ShopManager
    {
        Player player;

        ParallaxManager bg;

        Button upgradeBoat1Button;
        Vector2 boat1Pos;
        Button upgradeBoat2Button;
        Vector2 boat2Pos;
        Button upgradeHook1Button;
        Vector2 hook1Pos;
        Button upgradeHook2Button;
        Vector2 hook2Pos;

        public ShopManager(Vector2 posOffset, Player player)
        {
            this.player = player;

            bg = new ParallaxManager("mockup_Shop", posOffset);
        }

        public void Update()
        {

        }

        public void Draw()
        {
            bg.Draw();
        }
    }
}

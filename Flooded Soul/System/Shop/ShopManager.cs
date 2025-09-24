using Flooded_Soul.System.BG;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Button = Flooded_Soul.System.UI.Button;

namespace Flooded_Soul.System.Shop
{
    internal class ShopManager
    {
        ParallaxManager bg;

        Button upgradeBoatButton;
        Vector2 boatPos = new Vector2(0.67f * Game1.instance.viewPortWidth,0.55f * Game1.instance.viewPortHeight);
        Button upgradeHookButton;
        Vector2 hookPos = new Vector2(0.67f * Game1.instance.viewPortWidth,0.25f * Game1.instance.viewPortHeight);

        int boatCost = 70;
        int hookCost = 50;

        public ShopManager(string shop,Vector2 posOffset)
        {
            bg = new ParallaxManager("Shop/shop_inside", posOffset);

            #region Boat
            upgradeBoatButton = new Button("Shop/ui_upgrade_items", boatPos,posOffset,7,4,1);
            upgradeBoatButton.OnClick += UpgradeBoat;
            upgradeBoatButton.ChangeSprite(1);
            #endregion
            #region Hook
            upgradeHookButton = new Button("Shop/ui_upgrade_items", hookPos,posOffset,7,4,1);
            upgradeHookButton.OnClick += UpgradeHook;
            upgradeHookButton.ChangeSprite(0);
            #endregion
        }

        public void Update()
        {
            upgradeBoatButton.Update();
            upgradeHookButton.Update();
            UpgradeButtonVisualize();
            UpgradeCostUpdate();
        }

        public void Draw()
        {
            bg.Draw(1);
            upgradeBoatButton.Draw();
            upgradeHookButton.Draw();
        }

        void UpgradeBoat()
        {
            if (Game1.instance.player.fishPoint >= boatCost)
            {
                Game1.instance.player.fishPoint -= boatCost;
                Game1.instance.player.BoatLevel++;
            }
        }

        void UpgradeHook()
        {
            if (Game1.instance.player.fishPoint >= hookCost)
            {
                Game1.instance.player.fishPoint -= hookCost;
                Game1.instance.player.HookLevel++;
            }
        }

        void UpgradeButtonVisualize()
        {
            upgradeBoatButton.ChangeSprite(Game1.instance.player.BoatLevel);
            upgradeHookButton.ChangeSprite(Game1.instance.player.HookLevel);
        }

        void UpgradeCostUpdate()
        {
            switch (Game1.instance.player.BoatLevel)
            {
                case 1:
                    boatCost = 400;
                    break;
                case 2:
                    boatCost = 600;
                    break;
            }

            switch (Game1.instance.player.HookLevel)
            {
                case 0:
                    hookCost = 70;
                    break;
                case 1:
                    hookCost = 150;
                    break;
                case 2:
                    hookCost = 400;
                    break;
            }
        }
    }
}

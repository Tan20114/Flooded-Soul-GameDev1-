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

        #region Upgrade Button
        Button upgradeBoatButton;
        Vector2 boatPos = new Vector2(0.67f * Game1.instance.viewPortWidth, 0.55f * Game1.instance.viewPortHeight);

        Button upgradeHookButton;
        Vector2 hookPos = new Vector2(0.67f * Game1.instance.viewPortWidth, 0.25f * Game1.instance.viewPortHeight);
        #endregion

        #region Cat
        Button cat1Button;
        Vector2 cat1Pos = new Vector2(0.832f * Game1.instance.viewPortWidth, 0.36f * Game1.instance.viewPortHeight);
        int cat1Cost = 100;

        Button cat2Button;
        Vector2 cat2Pos = new Vector2(0.882f * Game1.instance.viewPortWidth, 0.493f * Game1.instance.viewPortHeight);
        int cat2Cost = 200;

        Button cat3Button;
        Vector2 cat3Pos = new Vector2(0.915f * Game1.instance.viewPortWidth, 0.395f * Game1.instance.viewPortHeight);
        int cat3Cost = 300;
        #endregion

        #region Back Button
        Button backButton;
        Vector2 backButtPos = new Vector2(.01f * Game1.instance.viewPortWidth, .8f * Game1.instance.viewPortHeight);
        #endregion

        int boatCost = 70;
        int hookCost = 50;

        public ShopManager(string shop, Vector2 posOffset)
        {
            bg = new ParallaxManager("Shop/shop_inside", posOffset);

            #region Boat
            upgradeBoatButton = new Button("Shop/ui_upgrade_items", boatPos, posOffset, 7, 4, 1);
            upgradeBoatButton.OnClick += UpgradeBoat;
            upgradeBoatButton.ChangeSprite(1);
            #endregion
            #region Hook
            upgradeHookButton = new Button("Shop/ui_upgrade_items", hookPos, posOffset, 7, 4, 1);
            upgradeHookButton.OnClick += UpgradeHook;
            upgradeHookButton.ChangeSprite(0);
            #endregion
            #region Cat
            cat1Button = new Button("Shop/cat_1", cat1Pos, posOffset, 7, 2, 1);
            cat1Button.OnClick += BuyCat1;
            cat1Button.ChangeSprite(1);

            cat2Button = new Button("Shop/cat_2", cat2Pos, posOffset, 7, 2, 1);
            cat2Button.OnClick += BuyCat2;
            cat2Button.ChangeSprite(1);

            cat3Button = new Button("Shop/cat_3", cat3Pos, posOffset, 7, 2, 1);
            cat3Button.OnClick += BuyCat3;
            cat3Button.ChangeSprite(1);
            #endregion
            #region BackButton
            backButton = new Button("UI_Icon/ui_return", backButtPos, posOffset, 5.5f);
            backButton.OnClick += BackButtClick;
            #endregion
        }

        public void Update()
        {
            upgradeBoatButton.Update();
            upgradeHookButton.Update();
            cat1Button.Update();
            cat2Button.Update();
            cat3Button.Update();
            backButton.Update();
            UpgradeButtonVisualize();
            UpgradeCostUpdate();
        }

        public void Draw()
        {
            bg.Draw(1);
            upgradeBoatButton.Draw();
            upgradeHookButton.Draw();
            cat1Button.Draw();
            cat3Button.Draw();
            cat2Button.Draw();
            backButton.Draw();
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
                    boatCost = 1;
                    break;
                case 2:
                    boatCost = 1;
                    break;
            }

            switch (Game1.instance.player.HookLevel)
            {
                case 0:
                    hookCost = 1;
                    break;
                case 1:
                    hookCost = 1;
                    break;
                case 2:
                    hookCost = 1;
                    break;
            }
        }

        void BuyCat1()
        {
            if (Game1.instance.player.fishPoint >= cat1Cost && !Game1.instance.player.hasCat[1])
            {
                Game1.instance.player.fishPoint -= cat1Cost;
                Game1.instance.player.hasCat[1] = true;
                cat1Button.ChangeSprite(0);
                cat1Button.OnClick -= BuyCat1;
            }
        }

        void BuyCat2()
        {
            if (Game1.instance.player.fishPoint >= cat2Cost && !Game1.instance.player.hasCat[2])
            {
                Game1.instance.player.fishPoint -= cat2Cost;
                Game1.instance.player.hasCat[2] = true;
                cat2Button.ChangeSprite(0);
                cat2Button.OnClick -= BuyCat2;
            }
        }

        void BuyCat3()
        {
            if (Game1.instance.player.fishPoint >= cat3Cost && !Game1.instance.player.hasCat[3])
            {
                Game1.instance.player.fishPoint -= cat3Cost;
                Game1.instance.player.hasCat[3] = true;
                cat3Button.ChangeSprite(0);
                cat3Button.OnClick -= BuyCat3;
            }
        }

        void BackButtClick()
        { 
            Game1.instance.dui.showShop = false;
            Game1.instance.sceneState = Flooded_Soul.Scene.Default;
            Game1.instance.player.stopAtShop = false;
        } 
    }
}

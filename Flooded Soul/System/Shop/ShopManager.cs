using Flooded_Soul.System.BG;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Button = Flooded_Soul.System.UI.Button;
using Color = Microsoft.Xna.Framework.Color;

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
        Button catUpgradeButton;
        Vector2 catUpgradePos = new Vector2(.805f * Game1.instance.viewPortWidth, .35f * Game1.instance.viewPortHeight);
        #endregion

        #region Back Button
        Button backButton;
        Vector2 backButtPos = new Vector2(.02f * Game1.instance.viewPortWidth, .8f * Game1.instance.viewPortHeight);
        #endregion

        int boatCost = 70;
        int hookCost = 50;
        int catCost = 50;

        SpriteFont font => Game1.instance.Content.Load<SpriteFont>("Fonts/fipps");
        #region Fish Point
        Vector2 fishPointPos = new Vector2(.2f * Game1.instance.viewPortWidth, .1f * Game1.instance.viewPortHeight);
        Texture2D fishPointIcon => Game1.instance.Content.Load<Texture2D>("UI_Icon/ui_fishunit_mainmenu");
        Vector2 fishIconPos = new Vector2(.01f * Game1.instance.viewPortWidth, .2f * Game1.instance.viewPortHeight);
        #endregion

        public ShopManager(string shop, Vector2 posOffset)
        {
            bg = new ParallaxManager("Shop/shop_inside", posOffset);

            #region Boat
            upgradeBoatButton = new Button("Shop/ui_upgrade_items_boat", boatPos, posOffset, 5, 4, 1);
            upgradeBoatButton.OnClick += UpgradeBoat;
            upgradeBoatButton.ChangeSprite(1);
            #endregion
            #region Hook
            upgradeHookButton = new Button("Shop/ui_upgrade_items_fishrod", hookPos, posOffset, 5, 4, 1);
            upgradeHookButton.OnClick += UpgradeHook;
            upgradeHookButton.ChangeSprite(0);
            #endregion
            #region BackButton
            backButton = new Button("UI_Icon/ui_return", backButtPos, posOffset, 4.5f);
            backButton.OnClick += BackButtClick;
            #endregion
            #region Fish Point
            fishPointPos = new Vector2(.022f * Game1.instance.viewPortWidth + posOffset.X, .01f * Game1.instance.viewPortHeight + posOffset.Y);
            fishIconPos = new Vector2(.01f * Game1.instance.viewPortWidth + posOffset.X, .17f * Game1.instance.viewPortHeight + posOffset.Y);
            #endregion
            #region Cat Button
            catUpgradeButton = new Button("Shop/shop_cat_sheet", catUpgradePos, posOffset, 5.5f, 4, 1);
            catUpgradeButton.OnClick += CatClick;
            #endregion
        }

        public void Update()
        {
            upgradeBoatButton.Update();
            upgradeHookButton.Update();
            backButton.Update();
            #region Fish Point
            Vector2 fontSize = font.MeasureString($"{Game1.instance.player.fishPoint}");
            float padding = 3f * Game1.instance.screenRatio;
            fishIconPos = new Vector2(fishPointPos.X + fontSize.X / 1.5f + padding, fishPointPos.Y + .125f * fontSize.Y);
            #endregion
            catUpgradeButton.Update();
            UpgradeButtonVisualize();
            UpgradeCostUpdate();
        }

        public void Draw()
        {
            bg.Draw(1);
            upgradeBoatButton.Draw();
            upgradeHookButton.Draw();
            backButton.Draw();
            #region Fish point
            Game1.instance._spriteBatch.DrawString(font, $"{Game1.instance.player.fishPoint}", fishPointPos, Color.White, 0, Vector2.Zero, 1.5f * Game1.instance.screenRatio, SpriteEffects.None, 0);
            Game1.instance._spriteBatch.Draw(fishPointIcon, fishIconPos, null, Color.White, 0, Vector2.Zero, 5.5f * Game1.instance.screenRatio, SpriteEffects.None, 0);
            #endregion
            catUpgradeButton.Draw();
        }

        void UpgradeBoat()
        {
            AudioManager.Instance.PlaySfx("buy_upgrade_shop");
            if (Game1.instance.player.fishPoint >= boatCost)
            {
                Game1.instance.player.fishPoint -= boatCost;
                Game1.instance.player.BoatLevel++;
            }
        }

        void UpgradeHook()
        {
            AudioManager.Instance.PlaySfx("buy_upgrade_shop");
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
            catUpgradeButton.ChangeSprite(Game1.instance.player.CatLevel);
        }

        void UpgradeCostUpdate()
        {
            switch (Game1.instance.player.BoatLevel)
            {
                case 1:
                    boatCost = 50;
                    break;
                case 2:
                    boatCost = 75;
                    break;
            }

            switch (Game1.instance.player.HookLevel)
            {
                case 0:
                    hookCost = 15;
                    break;
                case 1:
                    hookCost = 40;
                    break;
                case 2:
                    hookCost = 70;
                    break;
            }

            if (Game1.instance.player.hasCat[1])
                catCost = 100;
            else if (Game1.instance.player.hasCat[2])
                catCost = 150;
        }

        void CatClick()
        {
            AudioManager.Instance.PlaySfx("buy_upgrade_shop");
            if (Game1.instance.player.fishPoint >= catCost)
            {
                Game1.instance.player.fishPoint -= catCost;
                Game1.instance.player.CatLevel++;
                Game1.instance.player.hasCat[Game1.instance.player.CatLevel] = true;
            }
        }

        void BackButtClick()
        {
            AudioManager.Instance.PlaySfx("button_click");
            Game1.instance.dui.showShop = false;
            Game1.instance.sceneState = Flooded_Soul.Scene.Default;
            Game1.instance.player.stopAtShop = false;
        } 
    }
}

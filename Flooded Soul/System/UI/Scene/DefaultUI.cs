using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Color = Microsoft.Xna.Framework.Color;

namespace Flooded_Soul.System.UI.Scene
{
    public class DefaultUI
    {
        Vector2 posOffset;

        public bool showShop = false;

        #region Music Button
        Button musicButton;
        Vector2 musicButtPos = new Vector2(.01f * Game1.instance.viewPortWidth, .05f * Game1.instance.viewPortHeight);
        #endregion
        #region Collection Button
        Button collectionButton;
        Vector2 collectionButtPos = new Vector2(.01f * Game1.instance.viewPortWidth, .4f * Game1.instance.viewPortHeight);
        #endregion
        #region Toggle Button
        Button toggleAutoSailButton;
        Vector2 toggleAutoSailButtPos = new Vector2(.067f * Game1.instance.viewPortWidth, .06f * Game1.instance.viewPortHeight);
        #endregion
        #region TextSection
        SpriteFont font => Game1.instance.Content.Load<SpriteFont>("Fonts/fipps");
        #region Distance Point
        Vector2 distancePointPos = new Vector2(.01f * Game1.instance.viewPortWidth, .1f * Game1.instance.viewPortHeight);
        #endregion
        #region Fish Point
        Vector2 fishPointPos = new Vector2(.01f * Game1.instance.viewPortWidth , .1f * Game1.instance.viewPortHeight);
        Texture2D fishPointIcon => Game1.instance.Content.Load<Texture2D>("UI_Icon/ui_fishunit_mainmenu");
        Vector2 fishIconPos = new Vector2(.01f * Game1.instance.viewPortWidth, .2f * Game1.instance.viewPortHeight);
        #endregion
        #endregion
        #region Go Down Button
        Button goDownButton;
        Vector2 goDownPos = new Vector2(.01f * Game1.instance.viewPortWidth, .75f * Game1.instance.viewPortHeight);
        #endregion
        #region Shop Button
        Button shopButton;
        Vector2 shopButtPos = new Vector2(.01f * Game1.instance.viewPortWidth, .75f * Game1.instance.viewPortHeight);
        #endregion
        #region Help Button
        Button helpButton;
        Vector2 helpButtPos = new Vector2(.04f * Game1.instance.viewPortWidth, .05f * Game1.instance.viewPortHeight);
        #endregion

        public DefaultUI(Vector2 offset)
        {
            posOffset = offset;

            #region Music Button
            musicButton = new Button("UI_Icon/ui_music_mainmenu", musicButtPos, posOffset, 8.5f);
            #endregion
            #region Collection Button
            collectionButton = new Button("UI_Icon/ui_collection_mainmenu", collectionButtPos, posOffset, 6);
            collectionButton.OnClick += CollectionButtClick;
            #endregion
            #region Toggle Button
            toggleAutoSailButton = new Button("UI_Icon/ui_autodrive_mainmenu", toggleAutoSailButtPos, posOffset,7,2,1);
            toggleAutoSailButton.OnClick += ToggleAutoSailButtClick;
            toggleAutoSailButton.ChangeSprite(1);
            #endregion
            #region Distance Text
            distancePointPos = new Vector2(.01f * Game1.instance.viewPortWidth + posOffset.X, .155f * Game1.instance.viewPortHeight + posOffset.Y);
            #endregion
            #region Fish Point
            fishPointPos = new Vector2(.01f * Game1.instance.viewPortWidth + posOffset.X, .26f * Game1.instance.viewPortHeight + posOffset.Y);
            #endregion
            #region Go Down Button
            goDownButton = new Button("UI_Icon/sprite_icon_test", goDownPos, posOffset,7,5,1);
            goDownButton.OnClick += GoDown;
            goDownButton.ChangeSprite(4);
            #endregion
            #region Shop Button
            shopButton = new Button("UI_Icon/sprite_icon_test", shopButtPos, posOffset, 7,5,1);
            shopButton.OnClick += OpenShop;
            shopButton.ChangeSprite(0);
            #endregion
            #region Help Button
            helpButton = new Button("UI_Icon/ui_help_botton", helpButtPos, posOffset, 8.5f);
            #endregion
        }

        public void Update()
        {
            #region Music Button
            musicButton.Update();
            #endregion
            #region Collection Button
            collectionButton.Update();
            #endregion
            #region Toggle Button
            toggleAutoSailButton.Update();
            #endregion
            #region Fish Point
            Vector2 fontSize = font.MeasureString($"{Game1.instance.player.fishPoint}");
            float padding = 3f * Game1.instance.screenRatio;
            fishIconPos = new Vector2(fishPointPos.X + fontSize.X / 1.5f + padding, fishPointPos.Y + .125f * fontSize.Y);
            #endregion
            if (BiomeSystem.isTransition) return;

            if (Game1.instance.player.stopAtShop)
                showShop = true;

            if (showShop)
                shopButton.Update();
            else
            {
                if (SceneManager.moveSuccess)
                    goDownButton.Update();
            }

            #region Help Button
            helpButton.Update();
            #endregion
        }

        public void Draw()
        {
            musicButton.Draw();
            collectionButton.Draw();
            toggleAutoSailButton.Draw();
            Game1.instance._spriteBatch.DrawString(font, $"{(int)(Game1.instance.player.distanceTraveled / 1000)} km",distancePointPos, Color.White, 0, Vector2.Zero, 1.5f * Game1.instance.screenRatio, SpriteEffects.None, 0);
            #region Fish point
            Game1.instance._spriteBatch.DrawString(font, $"{Game1.instance.player.fishPoint}",fishPointPos, Color.White, 0, Vector2.Zero, 1.5f * Game1.instance.screenRatio, SpriteEffects.None, 0);
            Game1.instance._spriteBatch.Draw(fishPointIcon, fishIconPos, null, Color.White, 0, Vector2.Zero, 7f * Game1.instance.screenRatio, SpriteEffects.None, 0);
            #endregion
            if (BiomeSystem.isTransition) return;

            if (showShop)
                shopButton.Draw();
            else
            {
                if(SceneManager.moveSuccess)
                    goDownButton.Draw();
            }

            helpButton.Draw();
        }

        void CollectionButtClick()
        {
            if(Game1.instance.sceneState == Flooded_Soul.Scene.Default || Game1.instance.sceneState == Flooded_Soul.Scene.Default_Stop)
                Game1.instance.sceneState = Flooded_Soul.Scene.Collection;
        }

        void ToggleAutoSailButtClick()
        {
            if(Game1.instance.autoStopAtShop)
            {
                Game1.instance.autoStopAtShop = false;
                toggleAutoSailButton.ChangeSprite(1);
            }
            else
            {
                Game1.instance.autoStopAtShop = true;
                toggleAutoSailButton.ChangeSprite(0);
            }
        }

        void GoDown()
        {
            Game1.instance.fm.EnterSea();
            Game1.instance.sceneState = Flooded_Soul.Scene.Fishing;
        }

        void OpenShop()
        {
            Game1.instance.sceneState = Flooded_Soul.Scene.Default_Stop;
            Game1.instance.sceneState = Flooded_Soul.Scene.Shop;
        }
    }
}

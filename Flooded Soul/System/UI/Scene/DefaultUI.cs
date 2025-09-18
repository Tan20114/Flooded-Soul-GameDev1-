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

        #region Collection Button
        Button collectionButton;
        Vector2 collectionButtPos = new Vector2(.01f * Game1.instance.viewPortWidth, .05f * Game1.instance.viewPortHeight);
        #endregion
        #region Toggle Button
        Button toggleAutoSailButton;
        Vector2 toggleAutoSailButtPos = new Vector2(.05f * Game1.instance.viewPortWidth, .06f * Game1.instance.viewPortHeight);
        #endregion
        #region Fish Point
        SpriteFont font => Game1.instance.Content.Load<SpriteFont>("Fonts/fipps");
        Vector2 fishPointPos = new Vector2(.01f * Game1.instance.viewPortWidth , .1f * Game1.instance.viewPortHeight);
        Texture2D fishPointIcon => Game1.instance.Content.Load<Texture2D>("UI_Icon/ui_fishunit_mainmenu");
        Vector2 fishIconPos = new Vector2(.01f * Game1.instance.viewPortWidth, .2f * Game1.instance.viewPortHeight);
        #endregion
        #region Sail/Stop Button
        Button sailStopButton;
        Vector2 sailStopPos = new Vector2(.02f * Game1.instance.viewPortWidth, .75f * Game1.instance.viewPortHeight);
        #endregion
        #region Go Down Button
        Button goDownButton;
        Vector2 goDownPos = new Vector2(.065f * Game1.instance.viewPortWidth, .8f * Game1.instance.viewPortHeight);
        #endregion

        public DefaultUI(Vector2 offset)
        {
            posOffset = offset;

            #region Collection Button
            collectionButton = new Button("UI_Icon/ui_collection_mainmenu", collectionButtPos, posOffset, 6);
            collectionButton.OnClick += CollectionButtClick;
            #endregion
            #region Toggle Button
            toggleAutoSailButton = new Button("UI_Icon/ui_autodrive_mainmenu", toggleAutoSailButtPos, posOffset,7,2,1);
            toggleAutoSailButton.OnClick += ToggleAutoSailButtClick;
            toggleAutoSailButton.ChangeSprite(1);
            #endregion
            #region Fish Point
            fishPointPos = new Vector2(.01f * Game1.instance.viewPortWidth + posOffset.X, .2f * Game1.instance.viewPortHeight + posOffset.Y);
            #endregion
            #region Sail/Stop Button
            sailStopButton = new Button("UI_Icon/sprite_anchor_LV3", sailStopPos, posOffset, 7);
            sailStopButton.OnClick += SailStop;
            #endregion
            #region Go Down Button
            goDownButton = new Button("UI_Icon/ui_fishunit_mainmenu", goDownPos, posOffset,9);
            goDownButton.OnClick += GoDown;
            #endregion
        }

        public void Update()
        {
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
            #region Sail/Stop Button
            sailStopButton.Update();
            #endregion
            #region Go Down Button
            goDownButton.Update();
            #endregion
        }

        public void Draw()
        {
            collectionButton.Draw();
            toggleAutoSailButton.Draw(1);
            #region Fish point
            Game1.instance._spriteBatch.DrawString(font, $"{Game1.instance.player.fishPoint}",fishPointPos, Color.White, 0, Vector2.Zero, 1.5f * Game1.instance.screenRatio, SpriteEffects.None, 0);
            Game1.instance._spriteBatch.Draw(fishPointIcon, fishIconPos, null, Color.White, 0, Vector2.Zero, 7f * Game1.instance.screenRatio, SpriteEffects.None, 0);
            #endregion
            sailStopButton.Draw();
            if (Game1.instance.sceneState == Flooded_Soul.Scene.Default_Stop)
                goDownButton.Draw();
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

        void SailStop()
        {
            if (Game1.instance.sceneState == Flooded_Soul.Scene.Default)
                Game1.instance.sceneState = Flooded_Soul.Scene.Default_Stop;
            else if (Game1.instance.sceneState == Flooded_Soul.Scene.Default_Stop)
                Game1.instance.sceneState = Flooded_Soul.Scene.Default;
        }

        void GoDown()
        {
            Game1.instance.fm.EnterSea();
            Game1.instance.sceneState = Flooded_Soul.Scene.Fishing;
        }
    }
}

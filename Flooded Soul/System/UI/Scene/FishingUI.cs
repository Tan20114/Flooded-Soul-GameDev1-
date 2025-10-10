using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Color = Microsoft.Xna.Framework.Color;

namespace Flooded_Soul.System.UI.Scene
{
    public class FishingUI
    {
        Vector2 posOffset;

        #region Pause Button
        Button pauseButton;
        Vector2 pauseButtPos = new Vector2(.01f * Game1.instance.viewPortWidth, .05f * Game1.instance.viewPortHeight);
        #endregion
        #region Back Button
        Button backButton;
        Vector2 backButtPos = new Vector2(.01f * Game1.instance.viewPortWidth, .8f * Game1.instance.viewPortHeight);
        #endregion
        SpriteFont font => Game1.instance.Content.Load<SpriteFont>("Fonts/fipps");
        #region Fish Point
        Vector2 fishPointPos = new Vector2(.01f * Game1.instance.viewPortWidth, .1f * Game1.instance.viewPortHeight);
        Texture2D fishPointIcon => Game1.instance.Content.Load<Texture2D>("UI_Icon/ui_fishunit_mainmenu");
        Vector2 fishIconPos = new Vector2(.01f * Game1.instance.viewPortWidth, .2f * Game1.instance.viewPortHeight);
        #endregion

        public FishingUI(Vector2 offset)
        {
            posOffset = offset;

            #region PauseButton
            pauseButton = new Button("UI_Icon/ui_pause_mainmenu", pauseButtPos, posOffset, 6.5f);
            pauseButton.OnClick += PauseButtClick;
            #endregion
            #region BackButton
            backButton = new Button("UI_Icon/ui_return", backButtPos, posOffset, 4.5f);
            backButton.OnClick += BackButtClick;
            #endregion
            #region Fish Point
            fishPointPos = new Vector2(.01f * Game1.instance.viewPortWidth + posOffset.X, .2f * Game1.instance.viewPortHeight + posOffset.Y);
            #endregion
        }

        public void Update()
        {
            #region PauseButton
            pauseButton.Update();
            if (Game1.instance.fm.isPause)
                pauseButton.ChangeSprite("UI_Icon/ui_unpause_mainmenu");
            else
                pauseButton.ChangeSprite("UI_Icon/ui_pause_mainmenu");
            #endregion
            #region Fish Point
            Vector2 fontSize = font.MeasureString($"{Game1.instance.player.fishPoint}");
            float padding = 3f * Game1.instance.screenRatio;
            fishIconPos = new Vector2(fishPointPos.X + fontSize.X / 1.5f + padding, fishPointPos.Y + .125f * fontSize.Y);
            #endregion
            #region BackButton
            if (SceneManager.moveSuccess)
                backButton.Update();
            #endregion
        }

        public void Draw()
        {
            pauseButton.Draw();
            #region Fish point
            Game1.instance._spriteBatch.DrawString(font, $"{Game1.instance.player.fishPoint}", fishPointPos, Color.White, 0, Vector2.Zero, 1.5f * Game1.instance.screenRatio, SpriteEffects.None, 0);
            Game1.instance._spriteBatch.Draw(fishPointIcon, fishIconPos, null, Color.White, 0, Vector2.Zero, 5.5f * Game1.instance.screenRatio, SpriteEffects.None, 0);
            #endregion
            backButton.Draw();
        }

        void PauseButtClick()
        {
            AudioManager.Instance.PlaySfx("button_click");
            Game1.instance.fm.isPause = !Game1.instance.fm.isPause;
        }

        void BackButtClick()
        {
            AudioManager.Instance.PlaySfx("between_water");
            Game1.instance.sceneState = Flooded_Soul.Scene.Default;
            Game1.instance.fm.ExitSea();
            Game1.instance.fm.isPause = false;
        }
    }
}

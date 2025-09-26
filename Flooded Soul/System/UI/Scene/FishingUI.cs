using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public FishingUI(Vector2 offset)
        {
            posOffset = offset;

            #region PauseButton
            pauseButton = new Button("UI_Icon/ui_pause_mainmenu", pauseButtPos, posOffset, 8.5f);
            pauseButton.OnClick += PauseButtClick;
            #endregion
            #region BackButton
            backButton = new Button("UI_Icon/ui_return", backButtPos, posOffset, 5.5f);
            backButton.OnClick += BackButtClick;
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
            #region BackButton
            if (SceneManager.moveSuccess)
                backButton.Update();
            #endregion
        }

        public void Draw()
        {
            pauseButton.Draw();
            backButton.Draw();
        }

        void PauseButtClick()
        {
            Game1.instance.fm.isPause = !Game1.instance.fm.isPause;
        }

        void BackButtClick()
        {
            Game1.instance.sceneState = Flooded_Soul.Scene.Default;
            Game1.instance.fm.ExitSea();
            Game1.instance.fm.isPause = false;
        }
    }
}

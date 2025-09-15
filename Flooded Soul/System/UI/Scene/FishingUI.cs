using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flooded_Soul.System.UI.Scene
{
    internal class FishingUI
    {
        Vector2 posOffset;

        #region Pause Button
        Button pauseButton;
        Vector2 pauseButtPos = new Vector2(.02f * Game1.instance.viewPortWidth, .05f * Game1.instance.viewPortHeight);
        #endregion
        #region Back Button
        Button backButton;
        Vector2 backButtPos = new Vector2(.9f * Game1.instance.viewPortWidth, .05f * Game1.instance.viewPortHeight);
        #endregion

        bool isPause = false;

        public FishingUI(Vector2 offset)
        {
            posOffset = offset;

            #region PauseButton
            pauseButton = new Button("UI_Icon/ui_pause_mainmenu", pauseButtPos, posOffset, 7);
            pauseButton.OnClick += PauseButtClick;
            #endregion
            #region BackButton
            backButton = new Button("UI_Icon/ui_return", backButtPos, posOffset, 7);
            backButton.OnClick += BackButtClick;
            #endregion
        }

        public void Update()
        {
            #region PauseButton
            pauseButton.Update();
            if (isPause)
                pauseButton.ChangeSprite("UI_Icon/ui_unpause_mainmenu");
            else
                pauseButton.ChangeSprite("UI_Icon/ui_pause_mainmenu");
            #endregion
            #region BackButton
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
            isPause = !isPause;
        }

        void BackButtClick()
        {
            Game1.instance.sceneState = Flooded_Soul.Scene.Default_Stop;
            isPause = false;
        }
    }
}

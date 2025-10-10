using Flooded_Soul.System.BG;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

using Button = Flooded_Soul.System.UI.Button;

namespace Flooded_Soul.System.Collection
{
    public class CollectionUI
    {
        ParallaxLayer fishPage1;
        ParallaxLayer fishPage2;
        ParallaxLayer catPage;
        ParallaxLayer tutorialPage;
        ParallaxLayer storyPage;

        ParallaxLayer currentPage;

        Button leftButton;
        Vector2 leftPos = new Vector2(.11f * Game1.instance.viewPortWidth, .4f * Game1.instance.viewPortHeight);
        Button rightButton;
        Vector2 rightPos = new Vector2(.45f * Game1.instance.viewPortWidth, .4f * Game1.instance.viewPortHeight);
        Button upButton;
        Vector2 upPos = new Vector2(.91f * Game1.instance.viewPortWidth, .05f * Game1.instance.viewPortHeight);
        Button downButton;
        Vector2 downPos = new Vector2(.91f * Game1.instance.viewPortWidth, .85f * Game1.instance.viewPortHeight);
        Button backButton;
        Vector2 backPos = new Vector2(.95f * Game1.instance.viewPortWidth, .8f * Game1.instance.viewPortHeight);

        bool isSheet = false;
        bool isFish = false;
        bool isCat = false;
        int fishType = 0;
        int FishType
        {
            get => fishType;
            set
            {
                if (value > 6)
                    fishType = 0;
                else if (value < 0)
                    fishType = 6;
                else
                    fishType = value;
            }
        }
        int fishPage = 1;
        int FishPage
        {
            get => fishPage;
            set
            {
                if (value > 1)
                    fishPage = 0;
                else if (value < 0)
                    fishPage = 1;
                else
                    fishPage = value;
            }
        }
        int catType = 0;
        int CatType
        {
            get => catType;
            set
            {
                if (value > 3)
                    catType = 0;
                else if (value < 0)
                    catType = 3;
                else
                    catType = value;
            }
        }
        int category = 2;
        public int Category
        {
            get => category;
            set
            {
                if (value > 3)
                    category = 0;
                else if (value < 0)
                    category = 3;
                else
                    category = value;
            }
        }

        public CollectionUI(Vector2 posOffset)
        {
            Initialize(posOffset);

            leftButton = new Button("UI_Icon/ui_collection_Leftbutton", leftPos, posOffset, 5.5f);
            leftButton.OnClick += LeftButtClick;
            rightButton = new Button("UI_Icon/ui_collection_Rightbutton", rightPos, posOffset, 5.5f);
            rightButton.OnClick += RightButtClick;
            upButton = new Button("UI_Icon/ui_collection_UpButton", upPos, posOffset, 5.5f);
            upButton.OnClick += UpButtClick;
            downButton = new Button("UI_Icon/ui_collection_DownButton", downPos, posOffset, 5.5f);
            downButton.OnClick += DownButtClick;
            backButton = new Button("UI_Icon/ui_return", backPos, posOffset, 5.5f);
            backButton.OnClick += BackButtClick;

            currentPage = tutorialPage;
        }

        void Initialize(Vector2 posOffset)
        {
            fishPage1 = new ParallaxLayer("Collection/set1_fish_collection", posOffset, 0, 1, 1280, 230);
            fishPage2 = new ParallaxLayer("Collection/set2_fish_collection", posOffset, 0, 1, 1280, 230);
            catPage = new ParallaxLayer("Collection/collection_cat-Sheet", posOffset, 0, 1, 1280, 230);
            tutorialPage = new ParallaxLayer("Collection/collection_tutorial", posOffset, 0, 1, 1280, 230);
            storyPage = new ParallaxLayer("Collection/collection_story", posOffset, 0, 1, 1280, 230);
        }

        public void Update()
        {
            SetCategory();

            if (isSheet)
            {
                leftButton.Update();
                rightButton.Update();
            }
            upButton.Update();
            downButton.Update();
            backButton.Update();
        }

        public void Draw()
        {
            if (isSheet)
            {
                if (isFish)
                    currentPage.Draw(true, FishType);
                else if (isCat)
                    currentPage.Draw(true, CatType);
                leftButton.Draw();
                rightButton.Draw();
            }
            else
                currentPage.Draw(true,0);

            upButton.Draw();
            downButton.Draw();
            backButton.Draw();
        }

        void SetCategory()
        {
            switch (Category)
            {
                case 0:
                    currentPage = GetFishPage();
                    isSheet = true;
                    isFish = true;
                    isCat = false;
                    break;
                case 1:
                    currentPage = catPage;
                    isSheet = true;
                    isFish = false;
                    isCat = true;
                    break;
                case 2:
                    currentPage = storyPage;
                    CatType = 0;
                    FishType = 0;
                    isSheet = false;
                    isFish = false;
                    isCat = false;
                    break;
                case 3:
                    currentPage = tutorialPage;
                    CatType = 0;
                    FishType = 0;
                    isSheet = false;
                    isFish = false;
                    isCat = false;
                    break;
            }
        }

        ParallaxLayer GetFishPage() => FishPage == 1 ? fishPage1 : fishPage2;

        void LeftButtClick()
        {
            AudioManager.Instance.PlaySfx("collection_arrow");
            if (isFish)
            {
                if (FishType == 0)
                {
                    FishType = 6;
                    FishPage--;
                }
                else
                    FishType--;
            }

            if (isCat)
                CatType--;
        }

        void RightButtClick()
        {
            AudioManager.Instance.PlaySfx("collection_arrow");
            if (isFish)
            {
                if (FishType == 6)
                {
                    FishType = 0;
                    FishPage++;
                }
                else
                    FishType++;
            }

            if (isCat)
                CatType++;
        }


        void UpButtClick()
        {
            AudioManager.Instance.PlaySfx("collection_arrow");
            Category--;
        }

        void DownButtClick()
        {
            AudioManager.Instance.PlaySfx("collection_arrow");
            Category++;
        }

        void BackButtClick()
        {
            AudioManager.Instance.PlaySfx("button_click");
            Game1.instance.sceneState = Scene.Default;
        }
    }
}

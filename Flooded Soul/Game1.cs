using System.Diagnostics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.ViewportAdapters;

using Gum.Forms;
using Gum.Forms.Controls;
using MonoGameGum;

using ButtonState = Microsoft.Xna.Framework.Input.ButtonState;
using Point = Microsoft.Xna.Framework.Point;
using Keys = Microsoft.Xna.Framework.Input.Keys;
using Color = Microsoft.Xna.Framework.Color;

using Flooded_Soul.Screens;
using Flooded_Soul.System;
using Flooded_Soul.System.BG;
using Flooded_Soul.System.Fishing;
using MonoGame.Extended.Collisions;
using RectangleF = MonoGame.Extended.RectangleF;

namespace Flooded_Soul
{
    public enum Scene
    {
        Default,
        Fishing
    }

    struct Biome
    {
        public List<List<string>> overWater;
        public string underWater;
    }

    public class Game1 : Game
    {
        #region Base System
        public static Game1 instance;
        public float deltaTime;
        public InputHandler Input = new InputHandler();
        public CollisionComponent collisionComponent;
        RectangleF collisionRect;
        #endregion

        #region Scene Management
        public OrthographicCamera mainCam;
        MonitorSize monitorSize = new MonitorSize();
        SceneManager scene = new SceneManager();
        #endregion

        #region Work Space
        public int monitorWidth => monitorSize.GetScreenResolution().width;
        public int monitorHeight => monitorSize.GetScreenResolution().height;

        public int viewPortWidth => _graphics.PreferredBackBufferWidth;
        public int viewPortHeight => _graphics.PreferredBackBufferHeight;

        public float aspectRatio => (float)viewPortWidth / (float)viewPortHeight;
        public float widthRatio => (float)viewPortWidth / (float)monitorWidth;
        public float heightRatio => (float)viewPortHeight / (float)monitorHeight;
        public float screenRatio => MathF.Min(widthRatio, heightRatio);
        #endregion

        public Scene sceneState = Scene.Default;
        #region Scene Point
        Vector2 fishingPoint => new Vector2(0, viewPortHeight);
        #endregion

        #region Game System
        public GraphicsDeviceManager _graphics;
        public SpriteBatch _spriteBatch;
        public KeyboardState keyboardState;
        public KeyboardState previousState;
        public MouseState mouseState;

        FishingManager fm;
        #endregion

        #region Background
        ParallaxManager bg;
        ParallaxManager underSeaBg;

        Biome ocean = new Biome()
        {
            overWater = new List<List<string>>()
            {
                new List<string>()
                {
                    "ParallaxBG/OceanBiome/01_ocean_biome_light_pole_1",
                    "ParallaxBG/OceanBiome/01_ocean_biome_light_pole_2",
                    "ParallaxBG/OceanBiome/01_ocean_biome_wood_log_1",
                    "ParallaxBG/OceanBiome/01_ocean_biome_wood_log_2"
                },
                new List<string>()
                {
                    "ParallaxBG/OceanBiome/02_ocean_biome_coconut_island1",
                    "ParallaxBG/115_trans",
                    "ParallaxBG/115_trans"
                },
                new List<string>()
                {
                    "ParallaxBG/OceanBiome/03_ocean_biome_island_small_1",
                    "ParallaxBG/OceanBiome/03_ocean_biome_island_small_2",
                    "ParallaxBG/OceanBiome/03_ocean_biome_island_small_3",
                    "ParallaxBG/OceanBiome/03_ocean_biome_island_small_4",
                    "ParallaxBG/OceanBiome/03_ocean_biome_sign"
                },
                new List<string>()
                {
                    "ParallaxBG/OceanBiome/04_ocean_biome_building",
                    "ParallaxBG/115_trans"
                },
                new List<string>()
                {
                    "ParallaxBG/OceanBiome/05_ocean_biome_clound_1",
                    "ParallaxBG/OceanBiome/05_ocean_biome_clound_2",
                    "ParallaxBG/OceanBiome/05_ocean_biome_clound_3",
                    "ParallaxBG/OceanBiome/05_ocean_biome_moutain"
                },
                new List<string>()
                {
                    "ParallaxBG/OceanBiome/bg_ocean_biome_sky",
                    "ParallaxBG/OceanBiome/foreground_ocean_biome_wave"
                }
            },
            underWater = "ParallaxBG/OceanBiome/undersea_biome_ocean"
        };
        Biome ice = new Biome
        {
        };

        int speed = 100;
        #endregion

        Player player;

        public Game1()
        {
            instance = this;

            #region Viewport Set
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferWidth = monitorSize.GetScreenResolution().width;
            _graphics.PreferredBackBufferHeight = (int)(0.3f * monitorSize.GetScreenResolution().height);
            #endregion

            #region Collision
            collisionRect = new RectangleF(0, viewPortHeight, viewPortWidth, viewPortHeight);
            collisionComponent = new CollisionComponent(collisionRect);
            #endregion

            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            Window.IsBorderless = true;
            Window.Position = new Point(0,monitorHeight -viewPortHeight);

            //Activated += (s, e) => WindowAPI.SetTopMost(true);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here

            #region System
            var viewportAdaptor = new BoxingViewportAdapter(Window,GraphicsDevice,viewPortWidth,viewPortHeight);
            mainCam = new OrthographicCamera(viewportAdaptor);
            #endregion

            #region Background
            bg = new ParallaxManager(ocean.overWater,Vector2.Zero, 500);
            underSeaBg = new ParallaxManager(ocean.underWater, fishingPoint);
            #endregion

            #region Entity
            fm = new FishingManager(3,1,1);

            player = new Player();
            #endregion
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            keyboardState = Keyboard.GetState();
            mouseState = Mouse.GetState();
            deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // TODO: Add your update logic here

            #region System
            SceneState();
            CamTest();
            #endregion
            #region Background
            bg.Update(gameTime);
            #endregion
            #region Entity
            fm.Update();

            player.Update(keyboardState,speed, gameTime);
            #endregion

            #region Collision
            collisionComponent.Update(gameTime);
            #endregion

            previousState = keyboardState;
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            var transformMatrix = mainCam.GetViewMatrix();

            // TODO: Add your drawing code here
            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend,transformMatrix : transformMatrix);


            #region Background
            bg.Draw();
            underSeaBg.Draw();
            #endregion

            #region Entity
            fm.Draw();

            player.Draw(Content.Load<SpriteFont>("font"));
            #endregion

            _spriteBatch.End();
            base.Draw(gameTime);
        }

        void SceneState()
        {
            if (Input.IsKeyPressed(Keys.A))
                sceneState = Scene.Fishing;
            else if (Input.IsKeyPressed(Keys.B))         
                sceneState = Scene.Default;
        }

        void CamTest()
        {
            switch(sceneState)
            {
                case Scene.Fishing:
                    {
                        bg.Stop();
                        scene.CamMoveTo(fishingPoint, 1000);
                        break;
                    }
                case Scene.Default:
                    {
                        bg.Start();
                        scene.CamMoveTo(Vector2.Zero, 1000);
                        break;
                    }
            }
        }
    }
}

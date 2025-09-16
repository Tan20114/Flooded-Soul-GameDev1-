using System.Diagnostics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.ViewportAdapters;

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
using Flooded_Soul.System.Shop;
using Flooded_Soul.System.UI.Scene;
using SizeF = MonoGame.Extended.SizeF;

namespace Flooded_Soul
{
    public enum Scene
    {
        Default,
        Default_Stop,
        Shop,
        Collection,
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
        public SceneManager scene = new SceneManager();
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
        Vector2 defaultPoint => Vector2.Zero;
        Vector2 fishingPoint => new Vector2(0, viewPortHeight);
        Vector2 shopPoint => new Vector2(viewPortWidth, 0);
        Vector2 CollectionPoint => new Vector2(-viewPortWidth, 0);
        #endregion

        #region UI
        DefaultUI dui;
        FishingUI fui;
        #endregion

        #region Game System
        public GraphicsDeviceManager _graphics;
        public SpriteBatch _spriteBatch;
        public KeyboardState keyboardState;
        public KeyboardState previousState;
        public MouseState mouseState;
        public MouseState prevMouse;

        public System.UI.Mouse mouse;

        public FishingManager fm;
        ShopManager sm;
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
                    "ParallaxBG/OceanBiome/04_ocean_biome_moutain",
                    "ParallaxBG/115_trans"
                },
                new List<string>()
                {
                    "ParallaxBG/OceanBiome/05_ocean_biome_clound_1",
                    "ParallaxBG/OceanBiome/05_ocean_biome_clound_2",
                    "ParallaxBG/OceanBiome/05_ocean_biome_clound_3",
                },
                new List<string>()
                {
                    "ParallaxBG/OceanBiome/shop_ocean_biome_shop",
                    "ParallaxBG/OceanBiome/bg_ocean_biome_sky",
                    "ParallaxBG/OceanBiome/foreground_ocean_biome_wave"
                }
            },
            underWater = "ParallaxBG/OceanBiome/undersea_biome_ocean"
        };
        Biome ice = new Biome
        {
        };

        public int speed = 100;
        #endregion

        public Player player;

        ParallaxLayer mockCollection ;

        public Game1()
        {
            instance = this;

            #region Viewport Set
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferWidth = monitorSize.GetScreenResolution().width;
            _graphics.PreferredBackBufferHeight = (int)(0.3f * monitorSize.GetScreenResolution().height);
            #endregion

            #region Collision
            collisionRect = new RectangleF(CollectionPoint,new SizeF(viewPortWidth * 3,viewPortHeight * 2));
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

            Activated += (s, e) => WindowAPI.SetTopMost(true);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here

            #region System
            var viewportAdaptor = new BoxingViewportAdapter(Window,GraphicsDevice,viewPortWidth,viewPortHeight);
            mainCam = new OrthographicCamera(viewportAdaptor);

            mouse = new System.UI.Mouse();
            #endregion

            #region UI
            dui = new DefaultUI(defaultPoint);
            fui = new FishingUI(fishingPoint);
            #endregion

            #region Background
            bg = new ParallaxManager(ocean.overWater,Vector2.Zero, 150);
            underSeaBg = new ParallaxManager(ocean.underWater, fishingPoint);
            #endregion

            #region Entity
            fm = new FishingManager();

            player = new Player();
            #endregion

            #region Shop
            sm = new ShopManager(shopPoint,player);
            #endregion

            mockCollection = new ParallaxLayer("mockup_Collection",CollectionPoint,0,1);
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
            SceneManagement();

            mouse.Update();
            #endregion
            #region UI
            dui.Update();
            fui.Update();
            #endregion
            #region Background
            bg.Update(gameTime, player.speed);
            #endregion
            #region Entity
            fm.Update();

            player.Update(keyboardState, gameTime);
            #endregion

            #region Collision
            collisionComponent.Update(gameTime);
            #endregion

            previousState = keyboardState;
            prevMouse = mouseState;
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
            underSeaBg.Draw(1);
            sm.Draw();
            mockCollection.Draw(1);
            #endregion

            #region Entity
            fm.Draw();

            player.Draw(Content.Load<SpriteFont>("Fonts/fipps"));
            #endregion

            #region UI
            dui.Draw();
            fui.Draw();
            #endregion
            _spriteBatch.End();
            base.Draw(gameTime);
        }

        void SceneState()
        {
            if (sceneState == Scene.Default)
            {
                player.Sail();

                if (Input.IsKeyPressed(Keys.A))
                    sceneState = Scene.Default_Stop;
                if (Input.IsKeyPressed(Keys.D))
                    sceneState = Scene.Collection;
            }    
            else if (sceneState == Scene.Default_Stop)
            {
                player.Stop();

                if (Input.IsKeyPressed(Keys.A))
                    sceneState = Scene.Default;
                if (Input.IsKeyPressed(Keys.B))
                {
                    sceneState = Scene.Fishing;
                    fm.EnterSea();
                }
                if (Input.IsKeyPressed(Keys.C))
                    sceneState = Scene.Shop;
                if (Input.IsKeyPressed(Keys.D))
                    sceneState = Scene.Collection;
            }
            else if (sceneState == Scene.Fishing)
            {
                if (Input.IsKeyPressed(Keys.B))
                    sceneState = Scene.Default_Stop;
            }
            else if (sceneState == Scene.Shop)
            {
                if (Input.IsKeyPressed(Keys.C))
                    sceneState = Scene.Default_Stop;
            }
            else if (sceneState == Scene.Collection)
            {
                if (Input.IsKeyPressed(Keys.D))
                    sceneState = Scene.Default;
            }
        }

        void SceneManagement()
        {
            switch(sceneState)
            {
                case Scene.Fishing:
                    {
                        bg.Stop();
                        scene.CamMoveTo(fishingPoint, 1000);
                        MouseOffset(fishingPoint);
                        break;
                    }
                case Scene.Default:
                    {
                        bg.Start();
                        scene.CamMoveTo(defaultPoint, 1000);
                        MouseOffset(defaultPoint);
                        break;
                    }
                case Scene.Default_Stop:
                    {
                        bg.Stop();
                        scene.CamMoveTo(defaultPoint, 1000);
                        MouseOffset(defaultPoint);
                        break;
                    }
                case Scene.Shop:
                    {
                        bg.Stop();
                        scene.CamMoveTo(shopPoint, 1000);
                        MouseOffset(shopPoint);
                        break;
                    }
                case Scene.Collection:
                    {
                        scene.CamMoveTo(CollectionPoint, 1000);
                        MouseOffset(CollectionPoint);
                        break;
                    }
            }
        }

        void MouseOffset(Vector2 posOffset) => mouse.posOffset = posOffset;
    }
}
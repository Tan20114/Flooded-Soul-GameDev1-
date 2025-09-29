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
using Flooded_Soul.System.Collection;

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

    public struct Biome
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
        Vector2 transitionPoint => new Vector2(0, -viewPortHeight);
        Vector2 shopPoint => new Vector2(viewPortWidth, 0);
        Vector2 CollectionPoint => new Vector2(-viewPortWidth, 0);
        #endregion

        #region UI
        public DefaultUI dui;
        public FishingUI fui;
        CollectionUI cui;
        #endregion

        #region Game System
        public GraphicsDeviceManager _graphics;
        public SpriteBatch _spriteBatch;
        public KeyboardState keyboardState;
        public KeyboardState previousState;
        public MouseState mouseState;
        public MouseState prevMouse;

        public System.UI.Mouse mouse;

        BiomeSystem bs;

        public FishingManager fm;

        ShopManager sm;
        public bool autoStopAtShop = false;
        public ShopLayer shop;

        public CollectionSystem collection;
        #endregion

        #region Background

        public ParallaxManager bg;
        public ParallaxManager underSeaBg;

        public static Biome ocean = new Biome()
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
        public static Biome ice = new Biome
        {
            overWater = new List<List<string>>()
            {
                new List<string>()
                {
                    "ParallaxBG/IceBiome/01_snow_biome_ice_igloo_1",
                },
                new List<string>()
                {
                    "ParallaxBG/IceBiome/02_snow_biome_ice_igloo_2",
                },
                new List<string>()
                {
                    "ParallaxBG/IceBiome/03_snow_biome_ice_float_1",
                    "ParallaxBG/IceBiome/03_snow_biome_ice_float_2",
                },
                new List<string>()
                {
                    "ParallaxBG/IceBiome/05_snow_biome_ice_bg_1",
                    "ParallaxBG/IceBiome/05_snow_biome_ice_bg_3",
                },
                new List<string>()
                {
                    "ParallaxBG/IceBiome/05_snow_biome_ice_moutain",
                    "ParallaxBG/115_trans"
                },
                new List<string>()
                {
                    "ParallaxBG/IceBiome/bg_snow_biome_sky",
                    "ParallaxBG/IceBiome/bg_snow_biome_wave"
                }
            },
            underWater = "ParallaxBG/IceBiome/under_Snow"
        };
        public static Biome forest = new Biome
        {
            overWater = new List<List<string>>()
            {
                new List<string>()
                {
                    "ParallaxBG/ForestBiome/01_forest_tree_1",
                    "ParallaxBG/ForestBiome/01_forest_tree_2",
                    "ParallaxBG/ForestBiome/01_forest_tree_3",
                    "ParallaxBG/ForestBiome/01_forest_tree_group",
                    "ParallaxBG/ForestBiome/01_forest_tree_no1",
                    "ParallaxBG/ForestBiome/01_forest_tree_no2",
                    "ParallaxBG/ForestBiome/01_forest_tree_no3",
                },
                new List<string>()
                {
                    "ParallaxBG/ForestBiome/02_forest_flylight_1",
                    "ParallaxBG/ForestBiome/02_forest_flylight_2",
                    "ParallaxBG/ForestBiome/02_forest_flylight_3",
                    "ParallaxBG/ForestBiome/02_forest_flylight_group",
                    "ParallaxBG/ForestBiome/02_forest_wood_1",
                    "ParallaxBG/ForestBiome/02_forest_wood_2",
                    "ParallaxBG/ForestBiome/02_forest_wood_3",
                    "ParallaxBG/ForestBiome/02_forest_wood_group"
                },
                new List<string>()
                {
                    "ParallaxBG/ForestBiome/03_forest_tree_group",
                    "ParallaxBG/ForestBiome/03_forest_tree_groupAll",
                    "ParallaxBG/ForestBiome/03_forest_tree_no12",
                    "ParallaxBG/ForestBiome/03_forest_tree_no23",
                    "ParallaxBG/ForestBiome/03_forest_tree_no24",
                },
                new List<string>()
                {
                    "ParallaxBG/ForestBiome/04_forest_cloud_1",
                    "ParallaxBG/ForestBiome/04_forest_island_1",
                    "ParallaxBG/ForestBiome/04_forest_island_2",
                    "ParallaxBG/ForestBiome/04_forest_island_group",
                },
                new List<string>()
                {
                    "ParallaxBG/ForestBiome/05_forest_cloud_1",
                    "ParallaxBG/ForestBiome/05_forest_cloud_2",
                    "ParallaxBG/ForestBiome/05_forest_cloud_group",
                    "ParallaxBG/ForestBiome/05_forest_mountain_1",
                    "ParallaxBG/ForestBiome/05_forest_mountain_2",
                    "ParallaxBG/ForestBiome/05_forest_mountain_group",
                },
                new List<string>()
                {
                    "ParallaxBG/ForestBiome/bg_forest_biome_sky",
                    "ParallaxBG/ForestBiome/bg_forest_biome_wave"
                }
            },
            underWater = "ParallaxBG/ForestBiome/under_Forest"
        };

        public int speed = 100;
        #endregion

        public Player player;


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

            bs = new BiomeSystem(transitionPoint);
            collection = new CollectionSystem();
            #endregion

            #region UI
            dui = new DefaultUI(defaultPoint);
            fui = new FishingUI(fishingPoint);
            cui = new CollectionUI(CollectionPoint);
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
            sm = new ShopManager(ocean.overWater[5][0],shopPoint);
            shop = new ShopLayer(ocean.overWater[5][0], shopPoint, 100);
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
            SceneManagement();

            mouse.Update();

            bs.Update(gameTime);
            #endregion
            #region UI
            if (sceneState == Scene.Default || sceneState == Scene.Default_Stop)
                dui.Update();
            else if (sceneState == Scene.Fishing)
                fui.Update();
            else if (sceneState == Scene.Shop)
                sm.Update();
            else if (sceneState == Scene.Collection)
                cui.Update();
            #endregion
            #region Background
            bg.Update(gameTime);
            #endregion
            #region Entity
            fm.Update();

            player.Update(keyboardState, gameTime);
            #endregion

            #region Collision
            collisionComponent.Update(gameTime);
            #endregion

            shop.Update(gameTime,speed);

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
            shop.Draw(1);
            underSeaBg.Draw(1);
            sm.Draw();
            cui.Draw();
            #endregion

            #region Entity
            fm.Draw();

            player.Draw();
            #endregion

            #region UI
            if (sceneState == Scene.Default || sceneState == Scene.Default_Stop)
                dui.Draw();
            else if (sceneState == Scene.Fishing)
                fui.Draw();
            bs.Draw();
            FishPoint.DrawAll();
            #endregion

            _spriteBatch.End();
            base.Draw(gameTime);
        }

        void SceneState()
        {
            if (sceneState == Scene.Default)
                player.Sail();  
            else if (sceneState == Scene.Default_Stop)
                player.Stop();
            
            if (sceneState == Scene.Collection)
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
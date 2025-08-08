using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Flooded_Soul.Screens;

using ButtonState = Microsoft.Xna.Framework.Input.ButtonState;
using Point = Microsoft.Xna.Framework.Point;
using Keys = Microsoft.Xna.Framework.Input.Keys;
using Color = Microsoft.Xna.Framework.Color;
using System.Diagnostics;
using Flooded_Soul.System;
using MonoGame.Extended;
using MonoGame.Extended.ViewportAdapters;

namespace Flooded_Soul
{
    public class Game1 : Game
    {
        public static Game1 instance;
        public float deltaTime;
        InputHandler Input = new InputHandler();

        public OrthographicCamera mainCam;
        MonitorSize monitorSize = new MonitorSize();
        SceneManager scene = new SceneManager();

        int monitorWidth => monitorSize.GetScreenResolution().width;
        int monitorHeight => monitorSize.GetScreenResolution().height;

        int viewPortWidth => _graphics.PreferredBackBufferWidth;
        int viewPortHeight => _graphics.PreferredBackBufferHeight;

        string sceneState = "";
        #region Scene Point
        Vector2 fishingPoint => new Vector2(0, viewPortHeight);
        #endregion

        public GraphicsDeviceManager _graphics;
        public SpriteBatch _spriteBatch;
        public KeyboardState keyboardState;
        public KeyboardState previousState;

        ParallaxManager bg;
        ParallaxManager fishBg;
        Player player;

        List<string> layer = new List<string>() {"1","2","3","4","5","6","7","8","9","10","11"};
        int speed = 1000;
        List<int> speeds = new List<int>() {1000,800,600,400,100};

        public Game1()
        {
            instance = this;
            
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferWidth = monitorSize.GetScreenResolution().width;
            _graphics.PreferredBackBufferHeight = (int)(0.3f * monitorSize.GetScreenResolution().height);

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
            var viewportAdaptor = new BoxingViewportAdapter(Window,GraphicsDevice,viewPortWidth,viewPortHeight);
            mainCam = new OrthographicCamera(viewportAdaptor);

            bg = new ParallaxManager(Content, layer, viewPortWidth, viewPortHeight, speed);
            player = new Player();
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            keyboardState = Keyboard.GetState();
            deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // TODO: Add your update logic here

            SceneState();
            CamTest();
            bg.Update(gameTime);
            player.Update(keyboardState,speed, gameTime);

            previousState = keyboardState;
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            var transformMatrix = mainCam.GetViewMatrix();

            // TODO: Add your drawing code here
            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend,transformMatrix : transformMatrix);

            bg.Draw();
            player.Draw(Content.Load<SpriteFont>("font"));

            _spriteBatch.End();
            base.Draw(gameTime);
        }

        void SceneState()
        {
            if (Input.IsKeyPressed(Keys.A))
            {
                bg.ToggleStop();
                sceneState = "fishing";
            }
            else if (Input.IsKeyPressed(Keys.B))
            {
                bg.ToggleStop();
                sceneState = "default";
            }
        }

        void CamTest()
        {
            switch(sceneState)
            {
                case "fishing":
                    {
                        scene.CamMoveTo(fishingPoint, 1000);
                        break;
                    }
                case "default":
                    {
                        scene.CamMoveTo(Vector2.Zero, 1000);
                        break;
                    }
            }
        }
    }
}

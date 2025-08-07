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

namespace Flooded_Soul
{
    public class Game1 : Game
    {
        MonitorSize monitorSize = new MonitorSize();

        int monitorWidth => monitorSize.GetScreenResolution().width;
        int monitorHeight => monitorSize.GetScreenResolution().height;

        int viewPortWidth => _graphics.PreferredBackBufferWidth;
        int viewPortHeight => _graphics.PreferredBackBufferHeight;


        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        ParallaxManager bg;

        List<string> layer = new List<string>() {"1","10","11"};
        List<int> speeds = new List<int>() {1000,800,100};

        public Game1()
        {
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
            bg = new ParallaxManager(Content, _spriteBatch, layer, viewPortWidth, viewPortHeight, speeds);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            bg.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

            bg.Draw();

            base.Draw(gameTime);
        }
    }
}

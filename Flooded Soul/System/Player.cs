using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Color = Microsoft.Xna.Framework.Color;
using Keys = Microsoft.Xna.Framework.Input.Keys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;

namespace Flooded_Soul.System
{
    internal class Player
    {
        int distanceTraveled;
        public int fishPoint;

        bool isStop = false;

        public Player()
        {
            distanceTraveled = 0;
            fishPoint = 0;
        }

        public void Update(KeyboardState ks,int speed, GameTime gameTime)
        {
            if(Game1.instance.Input.IsKeyPressed(Keys.S))
                SailOrStop();

            if (!isStop)
                distanceTraveled += (int)(speed * (float)gameTime.ElapsedGameTime.TotalSeconds);
            else
                distanceTraveled += 0;
        }

        public void Draw(SpriteFont font)
        {
            Game1.instance._spriteBatch.DrawString(font, $"Distance: {distanceTraveled} units", new Vector2(10, 10), Color.White);
        }

        void SailOrStop() => isStop = !isStop;

        public int GetDistance() => distanceTraveled;
    }
}

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
        int speed = 10;

        bool isStop = false;

        public Player()
        {
            distanceTraveled = 0;
        }

        public void Update(KeyboardState ks,int speed, GameTime gameTime)
        {
            // Update the distance traveled based on speed and time
            if(ks.IsKeyDown(Keys.S))
            {
                SailOrStop();
            }

            if (!isStop)
                distanceTraveled += (int)(speed * (float)gameTime.ElapsedGameTime.TotalSeconds);
            else
                distanceTraveled += 0;
        }

        public void Draw(SpriteFont font)
        {
            // Draw the distance traveled on the screen
            Game1.instance._spriteBatch.DrawString(font, $"Distance: {distanceTraveled} units", new Vector2(10, 10), Color.White);
        }

        void SailOrStop()
        {
            isStop = !isStop;
        }

        public int GetDistance() => distanceTraveled;
    }
}

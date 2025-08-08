using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Content;
using MonoGame.Extended.Timers;

namespace Flooded_Soul.System
{
    internal class ParallaxManager
    {
        SpriteBatch spriteBatch;

        List<ParallaxLayer> layer = new List<ParallaxLayer>();
        List<int> layerSpeed;

        private int screenWidth;
        private int screenHeight;

        int startSpeed;
        float decrement;
        bool isStop = false;

        public ParallaxManager(ContentManager content, List<string> texture, int screenWidth, int screenHeight, int speed)
        {
            this.startSpeed = speed;
            this.screenWidth = screenWidth;
            this.screenHeight = screenHeight;

            decrement = speed/texture.Count;

            for (int i = 0; i < texture.Count; i++)
            {
                //Debug.WriteLine($"Loading texture: {texture[i]} with speed {(int)(startSpeed - (decrement * i))}");
                layer.Add(new ParallaxLayer(content, texture[i], screenWidth, screenHeight, (int)(startSpeed -(decrement * i))));
            }
        }

        public void Update(GameTime gameTime)
        {
            if(isStop) return;

            foreach (ParallaxLayer l in layer)
                l.Update(gameTime);
        }

        public void Draw()
        {
            for (int i = layer.Count - 1; i >= 0; i--)
            {
                layer[i].Draw();
            }
        }

        public void ToggleStop() => isStop = !isStop;
    }
}

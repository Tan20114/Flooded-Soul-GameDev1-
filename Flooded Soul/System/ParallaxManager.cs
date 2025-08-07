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

        

        public ParallaxManager(ContentManager content, SpriteBatch sb, List<string> texture, int screenWidth, int screenHeight, List<int> speed)
        {
            this.spriteBatch = sb;
            this.layerSpeed = speed;
            this.screenWidth = screenWidth;
            this.screenHeight = screenHeight;
            for (int i = 0; i < texture.Count; i++)
            {
                Debug.WriteLine($"Loading texture: {texture[i]} with speed {layerSpeed[i]}");
                layer.Add(new ParallaxLayer(content, sb, texture[i], screenWidth, screenHeight, layerSpeed[i]));
            }
        }

        public void Update(GameTime gameTime)
        {
            foreach (ParallaxLayer l in layer)
                l.Update(gameTime);
        }

        public void Draw()
        {
            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
            for (int i = layer.Count - 1; i >= 0; i--)
            {
                layer[i].Draw();
            }
            spriteBatch.End();
        }
    }
}

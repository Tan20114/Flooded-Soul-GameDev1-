using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Flooded_Soul.System.Shop;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Content;
using MonoGame.Extended.Timers;

namespace Flooded_Soul.System.BG
{
    public class ParallaxManager
    {
        List<ParallaxLayer> layer = new List<ParallaxLayer>();

        Vector2 posOffset;

        private int screenWidth;
        private int screenHeight;

        int startSpeed;
        float decrement;
        bool isStop = false;

        int layerCount = 0;

        void Initialize(Vector2 posOffset)
        {
            screenWidth = Game1.instance.viewPortWidth;
            screenHeight = Game1.instance.viewPortHeight;
            this.posOffset = posOffset;
        }

        public ParallaxManager(List<List<string>> texture,Vector2 posOffset,int speed)
        {
            startSpeed = speed;
            Initialize(posOffset);

            decrement = speed/texture.Count/2;

            LoadLayer(texture[0]);
            LoadLayer(texture[1]);
            LoadLayer(texture[2]);
            LoadLayer(texture[3]);
            LoadWave(texture[^1]);
            LoadLayer(texture[4]);
            LoadSky(texture[^1]);
        }

        public ParallaxManager(string texture, Vector2 posOffset)
        {
            Initialize(posOffset);

            layer.Add(new ParallaxLayer(texture, posOffset, 0,1,1280,230));
        }

        void LoadWave(List<string> textures) 
        {
            int speed = (int)(startSpeed - decrement * layerCount);
            layer.Add(new ParallaxLayer(textures[^1], posOffset, speed));
            layerCount++;
        }

        void LoadLayer(List<string> textures) 
        { 
            int speed = (int)(startSpeed - decrement * layerCount);
            layer.Add(new ParallaxLayer(textures, posOffset, speed));
            layerCount++;
        }

        void LoadSky(List<string> textures) 
        {
            int speed = (int)(startSpeed - decrement * layerCount);
            layer.Add(new ParallaxLayer(textures[^2], posOffset, speed));
            layerCount++;
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
                layer[i].Draw();
        }

        public void Draw(int index)
        {
            for (int i = layer.Count - 1; i >= 0; i--)
                layer[i].Draw(true,0);
        }

        public void Stop() => isStop = true;

        public void Start() => isStop = false;

        public void ChangeBiome(List<List<string>> texture)
        {
            layer.Clear();

            layerCount = 0;

            LoadLayer(texture[0]);
            LoadLayer(texture[1]);
            LoadLayer(texture[2]);
            LoadLayer(texture[3]);
            LoadWave(texture[^1]);
            LoadLayer(texture[4]);
            LoadSky(texture[^1]);
        }

        public void ChangeBiome(string texture)
        {
            layer.Clear();
            layer.Add(new ParallaxLayer(texture, posOffset, 0));
        }
    }
}

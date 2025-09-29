using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Color = Microsoft.Xna.Framework.Color;
using System.Diagnostics;
using MonoGame.Extended.Collisions.Layers;
using Microsoft.Xna.Framework.Content;
using MonoGame.Extended.Content;
using MonoGame.Extended.Graphics;

namespace Flooded_Soul.System.BG
{
    public class ParallaxLayer
    {
        Random random = new Random();

        private List<Vector2> positions = new List<Vector2>();
        private List<Texture2D> slotTextures = new List<Texture2D>();
        private int textureWidth;
        public Texture2D texture;
        private List<Texture2D> textures = new List<Texture2D>();
        private int screenWidth;
        private int screenHeight;
        Texture2DAtlas atlas;

        protected Vector2 posOffset;

        int speed;
        protected float scale;
        protected float scaleX;
        protected float scaleY;
        public bool isStop = false;

        void Initialize(Vector2 posOffset,int speed,Texture2D texture)
        {
            screenWidth = Game1.instance.viewPortWidth;
            screenHeight = Game1.instance.viewPortHeight;
            this.posOffset = posOffset;
            this.speed = speed;
            scale = screenHeight / (float)texture.Height;
            GeneratePositions();
        }

        void Initialize(Vector2 posOffset, int speed, Texture2DRegion texture)
        {
            screenWidth = Game1.instance.viewPortWidth;
            screenHeight = Game1.instance.viewPortHeight;
            this.posOffset = posOffset;
            this.speed = speed;
            scale = screenHeight / (float)texture.Height;
            GeneratePositions();
        }

        public ParallaxLayer(string texture,Vector2 posOffset,int speed)
        {
            this.texture = Game1.instance.Content.Load<Texture2D>(texture);
            textureWidth = this.texture.Width;

            Initialize(posOffset,speed,this.texture);

            scaleX = screenWidth / textureWidth;
            scaleY = scale;
        }

        public ParallaxLayer(List<string> textures, Vector2 posOffset, int speed)
        {
            foreach (string path in textures)
                this.textures.Add(Game1.instance.Content.Load<Texture2D>(path));

            textureWidth = this.textures[0].Width;

            Initialize(posOffset, speed, this.textures[0]);
        }

        public ParallaxLayer(string tex,Vector2 posOffset, int speed, int count)
        {
            this.texture = Game1.instance.Content.Load<Texture2D>(tex);
            textureWidth = this.texture.Width;

            Initialize(posOffset, speed, this.texture);
            positions.Clear();
            slotTextures.Clear();
            GeneratePositions(count);

            scaleX = screenWidth / textureWidth;
            scaleY = scale;
        }

        public ParallaxLayer(string tex,Vector2 posOffset, int speed, int count, int frameWidth, int frameHeight)
        {
            texture = Game1.instance.Content.Load<Texture2D>(tex);
            atlas = Texture2DAtlas.Create("atlas",texture, frameWidth, frameHeight);

            Texture2DRegion firstRegion = atlas.GetRegion(0);

            Initialize(posOffset, speed, firstRegion);
            positions.Clear();
            slotTextures.Clear();
            GeneratePositions(count);

            scaleX = screenWidth / (float)firstRegion.Width;
            scaleY = scale;
        }

        void GeneratePositions()
        {
            positions.Clear();
            slotTextures.Clear();

            Texture2D firstTex = textures.Count > 0 ? textures[0] : texture;
            float texWidthScaled = firstTex.Width * scale;
            float texHeightScaled = firstTex.Height * scale;

            int count = screenWidth / (int)texWidthScaled + 2;
            float yPos = screenHeight - texHeightScaled + posOffset.Y;

            for (int i = 0; i < count; i++)
            {
                positions.Add(new Vector2(i * texWidthScaled + posOffset.X, yPos));

                if (textures.Count > 0)
                {
                    int index = random.Next(textures.Count);
                    slotTextures.Add(textures[index]);
                }
                else
                    slotTextures.Add(texture);
            }
        }

        void GeneratePositions(int count)
        {
            positions.Clear();
            slotTextures.Clear();

            Texture2D firstTex = textures.Count > 0 ? textures[0] : texture;
            float texWidthScaled = firstTex.Width * scale;
            float texHeightScaled = firstTex.Height * scale;

            float yPos = screenHeight - texHeightScaled + posOffset.Y;

            for (int i = 0; i < count; i++)
            {
                positions.Add(new Vector2(i * texWidthScaled + posOffset.X, yPos));

                if (textures.Count > 0)
                {
                    int index = random.Next(textures.Count);
                    slotTextures.Add(textures[index]);
                }
                else
                    slotTextures.Add(texture);
            }
        }

        public void Update(GameTime gameTime)
        {
            if (isStop) return;

            float delta = speed * (float)gameTime.ElapsedGameTime.TotalSeconds;

            for (int i = 0; i < positions.Count; i++)
                positions[i] = new Vector2(positions[i].X - delta, positions[i].Y);

            float texWidthScaled = (textures.Count > 0 ? textures[0].Width : texture.Width) * scale;

            while (positions.Count > 0 && positions[0].X <= -texWidthScaled)
            {
                Vector2 firstPos = positions[0];
                positions.RemoveAt(0);
                Vector2 lastPos = positions[positions.Count - 1];
                firstPos.X = lastPos.X + texWidthScaled;
                positions.Add(firstPos);

                if (textures.Count > 0)
                {
                    Random rng = new Random();
                    Texture2D firstTex = textures[rng.Next(textures.Count)];
                    slotTextures.RemoveAt(0);
                    slotTextures.Add(firstTex);
                }
            }
        }


        public void Draw()
        {
            for (int i = 0; i < positions.Count; i++)
            {
                Game1.instance._spriteBatch.Draw(
                    slotTextures[i],
                    positions[i],
                    null,
                    Color.White,
                    0f,
                    Vector2.Zero,
                    new Vector2(scale),
                    SpriteEffects.None,
                    0f
                );
            }
        }

        public void Draw(int index)
        {
            Game1.instance._spriteBatch.Draw(
                    texture,
                    posOffset,
                    null,
                    Color.White,
                    0f,
                    Vector2.Zero,
                    new Vector2(scaleX,scaleY),
                    SpriteEffects.None,
                    0f
                );
        }

        public virtual void Draw(Vector2 pos)
        {
            Game1.instance._spriteBatch.Draw(
                    texture,
                    pos,
                    null,
                    Color.White,
                    0f,
                    Vector2.Zero,
                    new Vector2(scaleX, scaleY),
                    SpriteEffects.None,
                    0f
                );
        }

        public virtual void Draw(Vector2 pos, Color color)
        {
            Game1.instance._spriteBatch.Draw(
                    texture,
                    pos,
                    null,
                    color,
                    0f,
                    Vector2.Zero,
                    new Vector2(scaleX, scaleY),
                    SpriteEffects.None,
                    0f
                );
        }

        public void Draw(bool isRegion,int regionIndex)
        {
            Game1.instance._spriteBatch.Draw(
                    atlas.GetRegion(regionIndex),
                    posOffset,
                    Color.White,
                    0f,
                    Vector2.Zero,
                    new Vector2(scaleX, scaleY),
                    SpriteEffects.None,
                    0f
                );
        }

        public void TogglePause() => isStop = !isStop;
    }
}

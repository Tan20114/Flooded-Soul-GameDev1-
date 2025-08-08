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

namespace Flooded_Soul.System
{
    internal class ParallaxLayer
    {
        SpriteFont font;

        private List<Vector2> positions = new List<Vector2>();
        private int textureWidth;
        private Texture2D texture;
        private int screenWidth;
        private int screenHeight;

        int speed;
        float scale;
        bool isStop = false;

        public ParallaxLayer(ContentManager content,string texture,int screenWidth,int screenHeight,int speed)
        {
            this.texture = content.Load<Texture2D>(texture);
            textureWidth = this.texture.Width;
            this.screenWidth = screenWidth;
            this.screenHeight = screenHeight;
            this.speed = speed;
            scale = (float)screenHeight/ (float)this.texture.Height;
            GeneratePositions();
            Debug.WriteLine($"ScreenHeight: {screenHeight}");
            Debug.WriteLine($"TextureHeight: {this.texture.Height}");
            Debug.WriteLine($"Scale: {scale}");

            font = content.Load<SpriteFont>("font");
        }

        void GeneratePositions()
        {
            float scaledTextureWidth = textureWidth * scale;
            float scaledTextureHeight = texture.Height * scale;
            int count = screenWidth / (int)scaledTextureWidth + 2;

            positions.Clear();

            float yPos = screenHeight - scaledTextureHeight;

            for (int i = 0; i < count; i++)
            {
                positions.Add(new Vector2(i * scaledTextureWidth, yPos));
            }
        }

        public void Update(GameTime gameTime)
        {
            if (isStop) return;

            float delta = speed * (float)gameTime.ElapsedGameTime.TotalSeconds;

            for (int i = 0; i < positions.Count; i++)
            {
                positions[i] = new Vector2(positions[i].X - delta, positions[i].Y);
            }

            float scaledTextureWidth = textureWidth * scale;
            if (positions.Count > 0 && positions[0].X <= -scaledTextureWidth)
            {
                Vector2 first = positions[0];
                positions.RemoveAt(0);
                Vector2 last = positions[positions.Count - 1];
                first.X = last.X + scaledTextureWidth;
                positions.Add(first);
            }
        }

        public void Draw()
        {
            foreach (var pos in positions)
            {
                Game1.instance._spriteBatch.Draw(texture, pos, null, Color.White, 0f, Vector2.Zero, new Vector2(scale, scale), SpriteEffects.None, 0f);
            }
        }

        public void TogglePause() => isStop = !isStop;
    }
}

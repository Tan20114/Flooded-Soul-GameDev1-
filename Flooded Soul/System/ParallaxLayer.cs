using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Color = Microsoft.Xna.Framework.Color;
using System.Diagnostics;

namespace Flooded_Soul.System
{
    internal class ParallaxLayer
    {
        SpriteBatch spriteBatch;

        private List<Vector2> positions = new List<Vector2>();
        private int textureWidth;
        private Texture2D texture;
        private int screenWidth;
        private int screenHeight;

        int speed;

        public ParallaxLayer(SpriteBatch sb,Texture2D texture,int screenWidth,int screenHeight,int speed)
        {
            this.spriteBatch = sb;
            this.texture = texture;
            textureWidth = texture.Width;
            this.screenWidth = screenWidth;
            this.screenHeight = screenHeight;
            GeneratePositions();
        }

        void GeneratePositions()
        {
            int count = screenWidth/textureWidth + 2;
            positions.Clear();

            for(int i = 0; i < count;i++)
            {
                positions.Add(new Vector2(i * textureWidth, screenHeight - texture.Height));
            }
        }

        public void Update(GameTime gameTime)
        {
            float delta = speed * (float)gameTime.ElapsedGameTime.TotalSeconds;

            for (int i = 0; i < positions.Count; i++)
            {
                Vector2 p = positions[i];
                p.X -= delta;
                positions[i] = p;
                Debug.WriteLine($"Parallax first X: {positions[0].X:F2}");
            }

            if (positions.Count > 0 && positions[0].X <= -textureWidth)
            {
                Debug.WriteLine("Move");
                Vector2 first = positions[0];
                positions.RemoveAt(0);
                Vector2 last = positions[positions.Count - 1];
                first.X = last.X + textureWidth;
                positions.Add(first);
            }
        }

        public void Draw()
        {
            foreach (var pos in positions)
            {
                spriteBatch.Draw(texture, pos, null, Color.White, 0f, Vector2.Zero, Vector2.One, SpriteEffects.None, 0f);
            }
        }
    }
}

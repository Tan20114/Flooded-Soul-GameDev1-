using Flooded_Soul.System.BG;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Animations;
using MonoGame.Extended.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Color = Microsoft.Xna.Framework.Color;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace Flooded_Soul.System
{
    internal class AnimatedLayer : ParallaxLayer
    {
        SpriteSheet _sheet;
        public AnimationController controller;

        public AnimatedLayer(string tex, Vector2 posOffset, int speed, int count,string name,int frameWidth ,int frameHeight, int frameCount,float frameDuration = 0.1f, bool loop = true) : base(tex, posOffset, speed, count)
        {
            Texture2D texture = Game1.instance.Content.Load<Texture2D>(tex);
            Texture2DAtlas atlas = Texture2DAtlas.Create($"{name}",texture,frameWidth,frameHeight);
            _sheet = new SpriteSheet($"sheet/{tex}",atlas);

            scaleX = Game1.instance.viewPortWidth / (float)frameWidth;
            scaleY = Game1.instance.viewPortHeight / (float)frameHeight;

            CreateAnimation(name, frameCount,frameDuration,loop);
            controller = Animations(name);
        }

        public void Update(GameTime gameTime)
        {
            controller?.Update(gameTime);
        }

        public override void Draw(Vector2 pos)
        {
            Texture2DRegion currentFrameTexture = _sheet.TextureAtlas[controller.CurrentFrame];

            Game1.instance._spriteBatch.Draw(
                    currentFrameTexture,
                    pos,
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
            Texture2DRegion currentFrameTexture = _sheet.TextureAtlas[controller.CurrentFrame];

            Game1.instance._spriteBatch.Draw(
                    currentFrameTexture,
                    pos,
                    color,
                    0f,
                    Vector2.Zero,
                    new Vector2(scaleX, scaleY),
                    SpriteEffects.None,
                    0f
                );
        }

        public void CreateAnimation(string name, int frameCount, float frameDuration,bool loop)
        {
            _sheet.DefineAnimation(name, builder =>
            {
                builder.IsLooping(loop);

                for (int i = 0; i < frameCount; i++)
                    builder.AddFrame(i,TimeSpan.FromSeconds(frameDuration));
            });
        }

        SpriteSheetAnimation GetAnimation(string animationName) => _sheet.GetAnimation(animationName);

        AnimationController Animations(string name) => new AnimationController(GetAnimation(name));
    }
}

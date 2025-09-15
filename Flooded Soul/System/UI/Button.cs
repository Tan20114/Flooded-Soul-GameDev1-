using Flooded_Soul.System.Collision_System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Color = Microsoft.Xna.Framework.Color;
using Rectangle = Microsoft.Xna.Framework.Rectangle;
using RectangleF = MonoGame.Extended.RectangleF;
using MonoGame.Extended;
using MonoGame.Extended.Graphics;

namespace Flooded_Soul.System.UI
{
    internal class Button : ICollisionActor
    {
        CollisionTracker Collider = new CollisionTracker();

        public event Action OnClick;

        Texture2D tex;

        Texture2DAtlas atlas;
        Texture2DRegion frameToDraw;

        RectangleF rect;

        public IShapeF Bounds => rect;

        Color buttonColor = Color.White;

        Vector2 pos;
        float scale = 5f;

        public bool canClick = false;


        public Button(string texture,Vector2 pos,Vector2 posOffset, float scale)
        {
            tex = Game1.instance.Content.Load<Texture2D>(texture);

            this.pos = pos + posOffset;
            this.scale = scale * Game1.instance.screenRatio;

            Collider.CollisionEnter += OnCollisionEnter;
            Collider.CollisionStay += OnCollisionStay;
            Collider.CollisionExit += OnCollisionExit;

            rect = new RectangleF(pos.X,pos.Y, tex.Width * this.scale,tex.Height * this.scale);

            Game1.instance.collisionComponent.Insert(this);
        }

        public Button(string texture,Vector2 pos,Vector2 posOffset, float scale, int frameColumn,int frameRow)
        {
            tex = Game1.instance.Content.Load<Texture2D>(texture);
            int frameHeight = tex.Height / frameRow;
            int frameWidth = tex.Width / frameColumn;
            atlas = Texture2DAtlas.Create($"Atlas/{texture}",tex, frameWidth, frameHeight);

            int allFrame = (tex.Width / frameWidth) * (tex.Height / frameHeight);

            frameToDraw = atlas.CreateSprite(0).TextureRegion;

            this.pos = pos + posOffset;
            this.scale = scale * Game1.instance.screenRatio;

            Collider.CollisionEnter += OnCollisionEnter;
            Collider.CollisionStay += OnCollisionStay;
            Collider.CollisionExit += OnCollisionExit;

            rect = new RectangleF(pos.X,pos.Y, tex.Width * this.scale,tex.Height * this.scale);

            Game1.instance.collisionComponent.Insert(this);
        }

        public void Update()
        {
            if(canClick && Game1.instance.Input.IsLeftMouse())
                OnClick?.Invoke();

            Collider.Update();
        }

        public void Draw() => Game1.instance._spriteBatch.Draw(tex, pos,null, buttonColor,0,Vector2.Zero,scale,SpriteEffects.None,0);

        public void Draw(int i) => Game1.instance._spriteBatch.Draw(frameToDraw,pos,buttonColor, 0f,Vector2.Zero,new Vector2(scale),SpriteEffects.None,0);

        public void OnCollision(CollisionEventArgs collisionInfo) => Collider.RegisterCollision(collisionInfo.Other);

        void OnCollisionEnter(ICollisionActor other)
        {
            if(other is Mouse)
            {
                buttonColor = Color.Gray;
                canClick = true;
            }
        }

        void OnCollisionStay(ICollisionActor other)
        {
            if (other is Mouse)
            {
                buttonColor = Color.Gray;
                canClick = true;
            }
        }

        void OnCollisionExit(ICollisionActor other)
        {
            if (other is Mouse)
            {
                buttonColor = Color.White;
                canClick = false;
            }
        }

        public void ChangeSprite(string texture) => tex = Game1.instance.Content.Load<Texture2D>(texture);

        public void ChangeSprite(int index) => frameToDraw = atlas.CreateSprite(index).TextureRegion;
    }
}

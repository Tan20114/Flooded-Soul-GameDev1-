using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using MonoGame.Extended.Graphics;
using System.Collections.Generic;
using Color = Microsoft.Xna.Framework.Color;

namespace Flooded_Soul.System.Fishing
{
    public class FishPoint
    {
        public static List<FishPoint> activePool = new List<FishPoint>();
        private static Queue<FishPoint> inactivePool = new Queue<FishPoint>();

        static Dictionary<int, Texture2DRegion> pointSprites;
        static Texture2DAtlas atlas;

        Texture2DRegion sprite;
        Vector2 startPos;
        Vector2 position;
        float lifetime;
        float timer;
        float floatDistance;
        float alpha;

        public bool IsDead => timer >= lifetime;

        private FishPoint() { }

        public static void Initialize()
        {
            if (atlas != null) return;

            Texture2D tex = Game1.instance.Content.Load<Texture2D>("UI_Icon/numberlist");
            atlas = Texture2DAtlas.Create("number", tex, 75, 40);

            pointSprites = new Dictionary<int, Texture2DRegion>();
            for (int i = 1; i <= 10; i++)
                pointSprites[i] = atlas.GetRegion(i - 1);
        }

        public static void Spawn(int point, Vector2 pos)
        {
            FishPoint fp;

            fp = (inactivePool.Count > 0) ? inactivePool.Dequeue() : new FishPoint();

            fp.sprite = pointSprites.ContainsKey(point) ? pointSprites[point] : pointSprites[1];
            fp.startPos = pos;
            fp.position = pos;
            fp.lifetime = 1.5f;
            fp.timer = 0f;
            fp.floatDistance = 40f;
            fp.alpha = 1f;

            activePool.Add(fp);
        }

        void Update()
        {
            float dt = Game1.instance.deltaTime;
            timer += dt;

            float t = MathHelper.Clamp(timer / lifetime, 0f, 1f);

            float eased = 1f - (1f - t) * (1f - t);
            position = startPos - new Vector2(0, floatDistance * eased);

            alpha = 1f - t;
        }

        void Draw()
        {
            if (IsDead) return;
            Game1.instance._spriteBatch.Draw(sprite, position, Color.White * alpha);
        }

        public static void UpdateAll()
        {
            for (int i = activePool.Count - 1; i >= 0; i--)
            {
                var fp = activePool[i];
                fp.Update();

                if (fp.IsDead)
                {
                    inactivePool.Enqueue(fp);
                    activePool.RemoveAt(i);
                }
            }
        }

        public static void DrawAll()
        {
            foreach (var fp in activePool)
                fp.Draw();
        }
    }
}

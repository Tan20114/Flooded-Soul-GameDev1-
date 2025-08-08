using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using System.Text;
using System.Threading.Tasks;

namespace Flooded_Soul.System
{
    internal class SceneManager
    {
        public SceneManager()
        {

        }

        public void CamMoveTo(Vector2 point, int transitionSpeed)
        {
            Vector2 startPoint = Game1.instance.mainCam.Position;
            Vector2 endPoint = point;

            Vector2 distance = endPoint - startPoint;
            Vector2 moveDir = Vector2.Normalize(distance);

            float distanceToMove = Vector2.Distance(startPoint, endPoint);

            if (distanceToMove > 30)
            {
                Game1.instance.mainCam.Move(moveDir * transitionSpeed * Game1.instance.deltaTime);
            }
            else
            {
                Game1.instance.mainCam.Position = endPoint;
            }
        }
    }
}

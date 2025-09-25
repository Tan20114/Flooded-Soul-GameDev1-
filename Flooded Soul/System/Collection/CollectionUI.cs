using Flooded_Soul.System.BG;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Flooded_Soul.System.Collection
{
    public class CollectionUI
    {
        ParallaxLayer bg;

        public CollectionUI(Vector2 posOffset) 
        {
            bg = new ParallaxLayer("UI/collection_BG", new Vector2(0, 0), 0, 1);
        }
    }
}

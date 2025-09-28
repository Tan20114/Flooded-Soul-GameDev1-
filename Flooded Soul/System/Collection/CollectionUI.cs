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
        Dictionary<string, int> fishPageList = new Dictionary<string, int>();
        Dictionary<string, int> fishPageList2 = new Dictionary<string, int>();
        Dictionary<bool, int> catPageList = new Dictionary<bool, int>();

        ParallaxLayer currentPage;

        public CollectionUI(Vector2 posOffset) 
        {
            currentPage = new ParallaxLayer("UI/collection_BG", posOffset, 0, 1);
        }
    }
}

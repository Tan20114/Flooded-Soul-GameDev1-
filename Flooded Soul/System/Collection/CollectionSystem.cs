using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flooded_Soul.System.Collection
{
    public class CollectionSystem
    {
        Dictionary<string, bool> fishCollection = new Dictionary<string, bool>();

        public CollectionSystem() 
        {

        }

        public void AddFish(string fishId)
        {
            if (!fishCollection.ContainsKey(fishId))
            {
                fishCollection[fishId] = true;
            }
        }

        public bool HasFish(string fishId) => fishCollection.ContainsKey(fishId);

        public List<string> GetAllCollected() => new List<string>(fishCollection.Keys);
    }
}

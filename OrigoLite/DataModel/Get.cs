using System.Collections.Generic;

namespace OrigoLite
{
    public class Get : ReadTransaction<Dictionary<int, int>, int>
    {
        public readonly int Key;

        public Get(int key)
        {
            Key = key;
        }

        public override int Apply(Dictionary<int, int> system)
        {
            int result;
            system.TryGetValue(Key, out result);
            return result;
        }
    }
}
using System;
using System.Collections.Generic;

namespace OrigoLite
{
    [Serializable]
    public class Remove : WriteTransaction<Dictionary<int, int>>
    {
        public readonly int Key;

        public Remove(int key)
        {
            Key = key;
        }
        public override void Apply(Dictionary<int, int> system)
        {
            system.Remove(Key);
        }
    }
}
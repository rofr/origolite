using System;
using System.Collections.Generic;

namespace OrigoLite
{
    [Serializable]
    public class Set : WriteTransaction<Dictionary<int, int>>
    {
        public readonly int Key;
        public readonly int Value;

        public Set(int key, int value)
        {
            Key = key;
            Value = value;
        }

        public override void Apply(Dictionary<int, int> system)
        {
            system[Key] = Value;
        }
    }
}
using System;

namespace OrigoLite
{
    [Serializable]
    public abstract class WriteTransaction<T>
    {
        public abstract void Apply(T system);
    }
}
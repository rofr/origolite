namespace OrigoLite
{
    public abstract class ReadTransaction<T,R>
    {
        public abstract R Apply(T system);
    }
}
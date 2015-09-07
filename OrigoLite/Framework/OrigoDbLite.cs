using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;

namespace OrigoLite
{
    public class OrigoDbLite<T> where T : new()
    {
        private readonly FileStream _journal;
        private readonly IFormatter _formatter;
        private readonly T _system;
        private readonly ReaderWriterLockSlim _lock;

        public void ExecuteWrite(WriteTransaction<T> writeTransaction)
        {
            lock (_journal)
            {
                _formatter.Serialize(_journal, writeTransaction);
                _journal.Flush();
                _lock.EnterWriteLock();
                writeTransaction.Apply(_system);
                _lock.ExitWriteLock();
            }
        }

        public R ExecuteRead<R>(ReadTransaction<T, R> readTransaction)
        {
            _lock.EnterReadLock();
            var result = readTransaction.Apply(_system);
            _lock.ExitReadLock();
            return result;
        }

        public OrigoDbLite(string journalFile)
        {
            _lock = new ReaderWriterLockSlim();
            _formatter = new BinaryFormatter();
            if (File.Exists(journalFile)) _system = Load(journalFile);
            else _system = new T();
            _journal = File.Create(journalFile, 4096, FileOptions.WriteThrough);
        }

        /// <summary>
        /// Replay transactions from a file
        /// </summary>
        private T Load(string file)
        {
            var state = new T();
            var stream = File.OpenRead(file);
            while (stream.Position < stream.Length)
            {
                var writeTransaction = (WriteTransaction<T>) _formatter.Deserialize(stream);
                writeTransaction.Apply(state);
            }
            stream.Close();
            return state;
        }

        public void Close()
        {
            lock (_journal) _journal.Close();
        }
    }
}
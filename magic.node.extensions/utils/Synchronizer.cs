/*
 * Magic, Copyright(c) Thomas Hansen 2019 - thomas@gaiasoul.com
 * Licensed as Affero GPL unless an explicitly proprietary license has been obtained.
 */

using System;
using System.Threading;

namespace magic.node.extensions.utils
{
    /// <summary>
    /// Helper class to synchronize access to some shared resource.
    /// </summary>
    /// <typeparam name="TImpl">Class implementing the shared resource</typeparam>
    /// <typeparam name="TIRead">Optional read-only interface to be used during read-only operations</typeparam>
    /// <typeparam name="TIWrite">Optional write interface to be used during write operations</typeparam>
    public class Synchronizer<TImpl, TIRead, TIWrite> where TImpl : TIWrite, TIRead
    {
        readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();
        readonly TImpl _shared;

        /// <summary>
        /// Constructor creating a synchronizer wrapping the specified shared resource.
        /// </summary>
        /// <param name="shared"></param>
        public Synchronizer(TImpl shared)
        {
            _shared = shared;
        }

        /// <summary>
        /// Gives you synchronized access to read from the shared resource.
        /// </summary>
        /// <param name="functor">Function from where you want to read from the shared resource.</param>
        public void Read(Action<TIRead> functor)
        {
            _lock.EnterReadLock();
            try
            {
                functor(_shared);
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        /// <summary>
        /// Give you synchronized read access from the shared resource, 
        /// allowing you to return an instance of typeof(T) during your synchronized read operation.
        /// </summary>
        /// <typeparam name="T">Type you want to return from your reader function.</typeparam>
        /// <param name="functor">Function from where you'll extract your type T from within.</param>
        /// <returns></returns>
        public T Read<T>(Func<TIRead, T> functor)
        {
            _lock.EnterReadLock();
            try
            {
                return functor(_shared);
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        /// <summary>
        /// Give you synchronized write access to the shared resource.
        /// While this function is evaluated, all reader attempts will be blocked.
        /// </summary>
        /// <param name="functor">Function from where you want to write to your shared resource.</param>
        public void Write(Action<TIWrite> functor)
        {
            _lock.EnterWriteLock();
            try
            {
                functor(_shared);
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }
    }

    /// <summary>
    /// Convenience helper class for those cases where you don't have a read or write interface on your shared resource.
    /// </summary>
    /// <typeparam name="TImpl"></typeparam>
    public sealed class Synchronizer<TImpl> : Synchronizer<TImpl, TImpl, TImpl>
    {
        public Synchronizer(TImpl shared)
            : base(shared)
        { }
    }
}

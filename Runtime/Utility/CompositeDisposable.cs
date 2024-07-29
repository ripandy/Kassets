#if !KASSETS_R3

using System;
using System.Collections.Generic;

namespace Kadinche.Kassets
{
    internal class CompositeDisposable : IDisposable
    {
        private readonly IList<IDisposable> disposables = new List<IDisposable>();
        
        public void Add(IDisposable disposable)
        {
            disposables.Add(disposable);
        }

        public void Dispose()
        {
            disposables.Dispose();
        }
    }

    internal static class DisposableExtension
    {
        internal static void Dispose(this IList<IDisposable> disposables)
        {
            var i = 0;
            while (i < disposables.Count)
            {
                disposables[i++].Dispose();
            }

            disposables.Clear();
        }

        internal static void AddTo(this IDisposable disposable, IList<IDisposable> disposables)
        {
            disposables.Add(disposable);
        }
        
        internal static void AddTo(this IDisposable disposable, CompositeDisposable disposables)
        {
            disposables.Add(disposable);
        }
    }
}

#endif
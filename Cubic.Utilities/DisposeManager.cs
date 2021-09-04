using System;
using System.Collections.Generic;

namespace Cubic.Utilities
{
    public static class DisposeManager
    {
        private static List<IDisposable> _disposables = new List<IDisposable>();

        public static void Add(IDisposable disposable)
        {
            _disposables.Add(disposable);
        }

        public static void DisposeAll()
        {
            foreach (IDisposable disposable in _disposables)
                disposable.Dispose();
        }
    }
}
﻿#if !KASSETS_R3

using System;
using System.Collections.Generic;

namespace Kadinche.Kassets
{
    internal class Subscription : IDisposable
    {
        private Action _action;
        private IList<IDisposable> _disposables;
        
        public Subscription(
            Action action,
            IList<IDisposable> disposables)
        {
            _action = action;
            _disposables = disposables;
        }

        public void Invoke()
        {
            _action.Invoke();
        }

        public void Dispose()
        {
            if (_disposables.Contains(this))
            {
                _disposables.Remove(this);
            }
            
            _action = null;
            _disposables = null;
        }
    }
    
    internal class Subscription<T> : IDisposable
    {
        private Action<T> _action;
        private IList<IDisposable> _disposables;
        
        public Subscription(
            Action<T> action,
            IList<IDisposable> disposables)
        {
            _action = action;
            _disposables = disposables;
        }

        public void Invoke(T param)
        {
            _action.Invoke(param);
        }

        public void Dispose()
        {
            _action = null;
            
            if (_disposables == null)
            {
                return;
            }
            
            if (_disposables.Contains(this))
            {
                _disposables.Remove(this);
            }

            _disposables = null;
        }
    }
}

#endif
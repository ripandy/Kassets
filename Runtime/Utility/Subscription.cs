#if !KASSETS_R3

using System;
using System.Collections.Generic;
using Kadinche.Kassets.Transaction;

namespace Kadinche.Kassets
{
    internal struct Subscription : IDisposable
    {
        private Action action;
        private IList<IDisposable> disposables;
        
        public Subscription(
            Action action,
            IList<IDisposable> disposables)
        {
            this.action = action;
            this.disposables = disposables;
        }

        public void Invoke()
        {
            action.Invoke();
        }

        public void Dispose()
        {
            if (disposables.Contains(this))
            {
                disposables.Remove(this);
            }
            
            action = null;
            disposables = null;
        }
    }
    
    internal struct Subscription<T> : IDisposable
    {
        private Action<T> action;
        private IList<IDisposable> disposables;
        
        public Subscription(
            Action<T> action,
            IList<IDisposable> disposables)
        {
            this.action = action;
            this.disposables = disposables;
        }

        public void Invoke(T param)
        {
            action.Invoke(param);
        }

        public void Dispose()
        {
            action = null;
            
            if (disposables == null)
            {
                return;
            }
            
            if (disposables.Contains(this))
            {
                disposables.Remove(this);
            }

            disposables = null;
        }
    }
    
    internal struct ResponseSubscription<TRequest, TResponse> : IDisposable
    {
        private TransactionCore<TRequest, TResponse> source;
        private Func<TRequest, TResponse> responseFunc;
        
        public ResponseSubscription(
            TransactionCore<TRequest, TResponse> source,
            Func<TRequest, TResponse> responseFunc)
        {
            this.source = source;
            this.responseFunc = responseFunc;
        }

        public TResponse Invoke(TRequest param)
        {
            return responseFunc.Invoke(param);
        }

        public void Dispose()
        {
            responseFunc = null;
            source.responseSubscription = null;
            source = null;
        }
    }
}

#endif
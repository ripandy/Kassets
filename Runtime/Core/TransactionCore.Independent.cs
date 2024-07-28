#if !KASSETS_R3

using System;
using System.Collections.Generic;

namespace Kadinche.Kassets.Transaction
{
    public abstract partial class TransactionCore<TRequest, TResponse>
    {
        private readonly List<IDisposable> responseSubscribers = new();
        
        private partial void TryRespond()
        {
            if (responseSubscription is ResponseSubscription<TRequest, TResponse> subscription)
            {
                Response(subscription.Invoke);
            }
        }

        private partial void RaiseResponse(TResponse param)
        {
            responseValue = param;
            foreach (var disposable in responseSubscribers)
            {
                switch (disposable)
                {
                    case Subscription<TResponse> typedSubscription:
                        typedSubscription.Invoke(responseValue);
                        break;
                    case Subscription subscription:
                        subscription.Invoke();
                        break;
                }
            }
        }

        private partial IDisposable HandleSubscribe(Func<TRequest, TResponse> responseFunc)
        {
            return new ResponseSubscription<TRequest, TResponse>(this, responseFunc);
        }

        private partial IDisposable HandleSubscribeToResponse(Action action)
        {
            return HandleSubscribeToResponse(action, false);
        }

        private IDisposable HandleSubscribeToResponse(Action action, bool buffered)
        {
            var subscription = new Subscription(action, responseSubscribers);
            
            if (responseSubscribers.Contains(subscription))
            {
                return subscription;
            }

            responseSubscribers.Add(subscription);

            if (buffered)
            {
                subscription.Invoke();
            }

            return subscription;
        }

        private partial IDisposable HandleSubscribeToResponse(Action<TResponse> action)
        {
            return HandleSubscribeToResponse(action, false);
        }
        
        private IDisposable HandleSubscribeToResponse(Action<TResponse> action, bool buffered)
        {
            var subscription = new Subscription<TResponse>(action, responseSubscribers);
            
            if (responseSubscribers.Contains(subscription))
            {
                return subscription;
            }

            responseSubscribers.Add(subscription);

            if (buffered)
            {
                subscription.Invoke(responseValue);
            }

            return subscription;
        }

        private partial void DisposeSubscriptions()
        {
            _requests.Clear();
            responseSubscription?.Dispose();
        }
    }
    
    internal class ResponseSubscription<TRequest, TResponse> : IDisposable
    {
        private TransactionCore<TRequest, TResponse> _source;
        private Func<TRequest, TResponse> _responseFunc;
        
        public ResponseSubscription(
            TransactionCore<TRequest, TResponse> source,
            Func<TRequest, TResponse> responseFunc)
        {
            _source = source;
            _responseFunc = responseFunc;
        }

        public TResponse Invoke(TRequest param) => _responseFunc.Invoke(param);
        
        public void Dispose()
        {
            _responseFunc = null;
            _source.responseSubscription = null;
            _source = null;
        }
    }
}

#endif

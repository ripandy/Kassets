using System;
using System.Collections.Generic;
using System.Linq;
using Kadinche.Kassets.EventSystem;
using UnityEngine;

namespace Kadinche.Kassets.Transaction
{
    public abstract partial class TransactionCore<TRequest, TResponse> : GameEvent<TRequest>
    {
        [SerializeField] protected TResponse responseValue;
        
        private readonly Queue<(TRequest, Action<TResponse>)> requests = new();
        private readonly object syncRoot = new();

        internal IDisposable responseSubscription;
        
        public Type ResponseType => typeof(TResponse);

        public void Request(Action onResponse)
        {
            Request(default, _ => onResponse.Invoke());
        }

        public void Request(TRequest request, Action<TResponse> onResponse)
        {
            lock (syncRoot)
            {
                requests.Enqueue(new ValueTuple<TRequest, Action<TResponse>>(request, onResponse));
                Raise(request);
                TryRespond();
            }
        }
        
        public void Response(Func<TRequest, TResponse> responseFunc)
        {
            lock (syncRoot)
            {
                if (!requests.Any()) return;

                var (request, onResponse) = requests.Dequeue();
                var response = responseFunc.Invoke(request);
                onResponse.Invoke(response);
                RaiseResponse(response);
            }
        }
        
        public IDisposable RegisterResponse(Func<TRequest, TResponse> responseFunc, bool overrideResponse = true)
        {
            lock (syncRoot)
            {
                if (!overrideResponse && responseSubscription != null)
                {
                    Debug.LogWarning("Registered Response already exist. Set overrideResponse to true to override the existing response.");
                    return responseSubscription;
                }

                responseSubscription?.Dispose();
                responseSubscription ??= HandleSubscribe(responseFunc);

                while (requests.Count > 0)
                {
                    TryRespond();
                }

                return responseSubscription;
            }
        }

        protected override void ResetInternal()
        {
            base.ResetInternal();
            responseValue = default;
        }

        public override void Dispose()
        {
            base.Dispose();
            DisposeSubscriptions();
        }

        public IDisposable SubscribeToRequest(Action action) => Subscribe(action);
        public IDisposable SubscribeToRequest(Action<TRequest> action) => Subscribe(action);
        public IDisposable SubscribeToResponse(Action action) => HandleSubscribeToResponse(action);
        public IDisposable SubscribeToResponse(Action<TResponse> action) => Subscribe(action);
        public IDisposable Subscribe(Action<TResponse> action) => HandleSubscribeToResponse(action);
        
        // List of Partial methods. Implemented in each respective integrated Library.
        private partial void TryRespond();
        private partial void RaiseResponse(TResponse response);
        private partial IDisposable HandleSubscribe(Func<TRequest, TResponse> responseFunc);
        private partial IDisposable HandleSubscribeToResponse(Action action);
        private partial IDisposable HandleSubscribeToResponse(Action<TResponse> action);
        private partial void DisposeSubscriptions();
    }

    public abstract class TransactionCore<T> : TransactionCore<T, T>
    {
    }
}
#if KASSETS_R3

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using R3;
using UnityEngine;

namespace Kadinche.Kassets.Transaction
{
    public abstract partial class TransactionCore<TRequest, TResponse>
    {
        private readonly Subject<object> requestSubject = new();
        private readonly Subject<TResponse> responseSubject = new();
        
        private readonly Queue<(TRequest, Subject<TResponse>)> subjectRequests = new();
        
        public Observable<TRequest> RequestAsObservable() => AsObservable();
        public Observable<TResponse> ResponseAsObservable() => responseSubject;
    
        public async ValueTask RequestAsync(CancellationToken cancellationToken = default)
        {
            await RequestAsync(default, cancellationToken);
        }

        public ValueTask<TResponse> RequestAsync(TRequest request, CancellationToken cancellationToken = default)
        {
            var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, DefaultToken);
            var token = linkedTokenSource.Token;
            token.ThrowIfCancellationRequested();
            
            var responseSubj = new Subject<TResponse>();
            DefaultToken.Register(() => responseSubj.Dispose());
            subjectRequests.Enqueue(new ValueTuple<TRequest, Subject<TResponse>>(request, responseSubj));
            requestSubject.OnNext(this);
            Raise(request);
            return WaitResponseAsync(cancellationToken);
        }
        
        private partial void TryRespond()
        {
            requestSubject.OnNext(this);
        }

        private partial void RaiseResponse(TResponse response)
        {
            responseValue = response;
            responseSubject.OnNext(response);
        }
        
        public async ValueTask<TResponse> WaitResponseAsync(CancellationToken cancellationToken)
        {
            var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, DefaultToken);
            return await responseSubject.LastOrDefaultAsync(cancellationToken: linkedTokenSource.Token);
        }
        
        public async ValueTask ResponseAsync(Func<TRequest, ValueTask<TResponse>> responseFunc, CancellationToken cancellationToken = default)
        {
            var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, DefaultToken);
            var token = linkedTokenSource.Token;
            token.ThrowIfCancellationRequested();
        
            if (requests.Count > 0)
            {
                var (request, responseAction) = requests.Dequeue();
                var response = await responseFunc.Invoke(request);
                responseAction.Invoke(response);
                responseSubject.OnNext(response);
            }
            else if (subjectRequests.Count > 0)
            {
                var (request, subjectRequest) = subjectRequests.Dequeue();
                var response = await responseFunc.Invoke(request);
                subjectRequest.OnNext(response);
                responseSubject.OnNext(response);
            }
        }
        
        public async ValueTask ResponseAsync(Func<TRequest, CancellationToken, ValueTask<TResponse>> responseFunc, CancellationToken cancellationToken = default)
        {
            var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, DefaultToken);
            var token = linkedTokenSource.Token;
            token.ThrowIfCancellationRequested();
            
            if (requests.Count > 0)
            {
                var (request, responseAction) = requests.Dequeue();
                var response = await responseFunc.Invoke(request, token);
                responseAction.Invoke(response);
                responseSubject.OnNext(response);
            }
            else if (subjectRequests.Count > 0)
            {
                var (request, subjectRequest) = subjectRequests.Dequeue();
                var response = await responseFunc.Invoke(request, token);
                subjectRequest.OnNext(response);
                responseSubject.OnNext(response);
            }
        }
        
        public IDisposable RegisterResponse(Func<TRequest, ValueTask<TResponse>> responseFunc,
            CancellationToken cancellationToken = default,
            bool overrideResponse = true,
            bool responseAndForget = false)
        {
            if (!overrideResponse && responseSubscription != null)
            {
                Debug.LogWarning("Registered Response already exist. Set overrideResponse to true to override the existing response.");
                return responseSubscription;
            }
        
            responseSubscription?.Dispose();

            responseSubscription = responseAndForget?
                requestSubject.Subscribe(ResponseAndForget) :
                requestSubject.SubscribeAwait(async (_, token) => await ResponseAsync(responseFunc, token));
        
            while (requests.Count > 0 && !cancellationToken.IsCancellationRequested)
            {
                ResponseAndForget(this);
            }
        
            return responseSubscription;
            
            async void ResponseAndForget(object _) => await ResponseAsync(responseFunc, cancellationToken);
        }
        
        public IDisposable RegisterResponse(Func<TRequest, CancellationToken, ValueTask<TResponse>> responseFunc,
            CancellationToken cancellationToken = default,
            bool overrideResponse = true,
            bool responseAndForget = false)
        {
            if (!overrideResponse && responseSubscription != null)
            {
                Debug.LogWarning("Registered Response already exist. Set overrideResponse to true to override the existing response.");
                return responseSubscription;
            }
        
            responseSubscription?.Dispose();
            responseSubscription = responseAndForget?
                requestSubject.Subscribe(ResponseAndForget) :
                requestSubject.SubscribeAwait(async (_, token) => await ResponseAsync(responseFunc, token));
        
            while (requests.Count > 0 && !cancellationToken.IsCancellationRequested)
            {
                ResponseAndForget(this);
            }
        
            return responseSubscription;
            
            async void ResponseAndForget(object _) => await ResponseAsync(responseFunc, cancellationToken);
        }

        private partial IDisposable HandleSubscribe(Func<TRequest, TResponse> responseFunc)
        {
            return requestSubject.Subscribe(_ => Response(responseFunc));
        }
        
        private partial IDisposable HandleSubscribeToResponse(Action action)
        {
            return responseSubject.Subscribe(_ => action.Invoke());
        }
        
        private partial IDisposable HandleSubscribeToResponse(Action<TResponse> action)
        {
            return responseSubject.Subscribe(action);
        }

        private partial void DisposeSubscriptions()
        {
            requests.Clear();
            responseSubscription?.Dispose();
            requestSubject.Dispose();
            responseSubject.Dispose();
        }
    }
}

#endif

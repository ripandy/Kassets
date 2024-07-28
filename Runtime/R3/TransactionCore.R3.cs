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
        private readonly Subject<object> _requestSubject = new();
        private readonly Subject<TResponse> _responseSubject = new();
        
        private readonly Queue<(TRequest, Subject<TResponse>)> _subjectRequests = new();
        
        public Observable<TRequest> RequestAsObservable() => AsObservable();
        public Observable<TResponse> ResponseAsObservable() => _responseSubject;
    
        public async ValueTask RequestAsync(CancellationToken cancellationToken = default)
        {
            await RequestAsync(default, cancellationToken);
        }

        public ValueTask<TResponse> RequestAsync(TRequest request, CancellationToken cancellationToken = default)
        {
            var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, DefaultToken);
            var token = linkedTokenSource.Token;
            token.ThrowIfCancellationRequested();
            
            var responseSubject = new Subject<TResponse>();
            DefaultToken.Register(() => responseSubject.Dispose());
            _subjectRequests.Enqueue(new ValueTuple<TRequest, Subject<TResponse>>(request, responseSubject));
            _requestSubject.OnNext(this);
            Raise(request);
            return WaitResponseAsync(cancellationToken);
        }
        
        private partial void TryRespond()
        {
            _requestSubject.OnNext(this);
        }

        private partial void RaiseResponse(TResponse response)
        {
            responseValue = response;
            _responseSubject.OnNext(response);
        }
        
        public async ValueTask<TResponse> WaitResponseAsync(CancellationToken cancellationToken)
        {
            var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, DefaultToken);
            return await _responseSubject.LastOrDefaultAsync(cancellationToken: linkedTokenSource.Token);
        }
        
        public async ValueTask ResponseAsync(Func<TRequest, ValueTask<TResponse>> responseFunc, CancellationToken cancellationToken = default)
        {
            var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, DefaultToken);
            var token = linkedTokenSource.Token;
            token.ThrowIfCancellationRequested();
        
            if (_requests.Count > 0)
            {
                var (request, responseAction) = _requests.Dequeue();
                var response = await responseFunc.Invoke(request);
                responseAction.Invoke(response);
                _responseSubject.OnNext(response);
            }
            else if (_subjectRequests.Count > 0)
            {
                var (request, subjectRequest) = _subjectRequests.Dequeue();
                var response = await responseFunc.Invoke(request);
                subjectRequest.OnNext(response);
                _responseSubject.OnNext(response);
            }
        }
        
        public async ValueTask ResponseAsync(Func<TRequest, CancellationToken, ValueTask<TResponse>> responseFunc, CancellationToken cancellationToken = default)
        {
            var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, DefaultToken);
            var token = linkedTokenSource.Token;
            token.ThrowIfCancellationRequested();
            
            if (_requests.Count > 0)
            {
                var (request, responseAction) = _requests.Dequeue();
                var response = await responseFunc.Invoke(request, token);
                responseAction.Invoke(response);
                _responseSubject.OnNext(response);
            }
            else if (_subjectRequests.Count > 0)
            {
                var (request, subjectRequest) = _subjectRequests.Dequeue();
                var response = await responseFunc.Invoke(request, token);
                subjectRequest.OnNext(response);
                _responseSubject.OnNext(response);
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
                _requestSubject.Subscribe(ResponseAndForget) :
                _requestSubject.SubscribeAwait(async (_, token) => await ResponseAsync(responseFunc, token));
        
            while (_requests.Count > 0 && !cancellationToken.IsCancellationRequested)
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
                _requestSubject.Subscribe(ResponseAndForget) :
                _requestSubject.SubscribeAwait(async (_, token) => await ResponseAsync(responseFunc, token));
        
            while (_requests.Count > 0 && !cancellationToken.IsCancellationRequested)
            {
                ResponseAndForget(this);
            }
        
            return responseSubscription;
            
            async void ResponseAndForget(object _) => await ResponseAsync(responseFunc, cancellationToken);
        }

        private partial IDisposable HandleSubscribe(Func<TRequest, TResponse> responseFunc)
        {
            return _requestSubject.Subscribe(_ => Response(responseFunc));
        }
        
        private partial IDisposable HandleSubscribeToResponse(Action action)
        {
            return _responseSubject.Subscribe(_ => action.Invoke());
        }
        
        private partial IDisposable HandleSubscribeToResponse(Action<TResponse> action)
        {
            return _responseSubject.Subscribe(action);
        }

        private partial void DisposeSubscriptions()
        {
            _requests.Clear();
            responseSubscription?.Dispose();
            _requestSubject.Dispose();
            _responseSubject.Dispose();
        }
    }
}

#endif

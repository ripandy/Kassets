#if KASSETS_R3

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using R3;

namespace Kadinche.Kassets.EventSystem
{
    public partial class GameEvent
    {
        private CancellationTokenSource cts;
        protected CancellationToken DefaultToken
        {
            get
            {
                cts ??= new CancellationTokenSource();
                return cts.Token;
            }
        }
        
        private readonly Subject<object> _subject = new();
        
        /// <summary>
        /// Raise the event.
        /// </summary>
        public virtual void Raise() => _subject.OnNext(this);
        public IDisposable Subscribe(Action action) => _subject.Subscribe(_ => action.Invoke());
        
        public async ValueTask EventAsync(CancellationToken cancellationToken = default)
        {
            var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, DefaultToken);
            await _subject.WaitAsync(cancellationToken: linkedTokenSource.Token);
        }

        public override void Dispose()
        {
            _subject.Dispose();
            cts?.CancelAndDispose();
            cts = null;
        }
    }

    public abstract partial class GameEvent<T>
    {
        private readonly Subject<T> _valueSubject = new();
        
        public Observable<T> AsObservable() => _valueSubject;
        public IObservable<T> AsSystemObservable() => _valueSubject.AsSystemObservable();
        public IAsyncEnumerable<T> ToAsyncEnumerable(CancellationToken cancellationToken = default) => _valueSubject.ToAsyncEnumerable(cancellationToken: cancellationToken);
    
        /// <summary>
        /// Raise the event with parameter.
        /// </summary>
        /// /// <param name="param"></param>
        public virtual void Raise(T param)
        {
            _value = param;
            base.Raise();
            _valueSubject.OnNext(param);
        }

        public IDisposable Subscribe(Action<T> action) => _valueSubject.Subscribe(action);
        
        public new async ValueTask<T> EventAsync(CancellationToken cancellationToken = default)
        {
            var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, DefaultToken);
            return await _valueSubject.LastOrDefaultAsync(cancellationToken: linkedTokenSource.Token);
        }
        
        public override void Dispose()
        {
            _valueSubject.Dispose();
            base.Dispose();
        }
    }
}

#endif

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
        
        private readonly Subject<object> subject = new();
        
        /// <summary>
        /// Raise the event.
        /// </summary>
        public virtual void Raise() => subject.OnNext(this);
        public IDisposable Subscribe(Action action) => subject.Subscribe(_ => action.Invoke());
        
        public async ValueTask EventAsync(CancellationToken cancellationToken = default)
        {
            var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, DefaultToken);
            await subject.WaitAsync(cancellationToken: linkedTokenSource.Token);
        }

        public override void Dispose()
        {
            subject.Dispose();
            cts?.CancelAndDispose();
            cts = null;
        }
    }

    public abstract partial class GameEvent<T>
    {
        private readonly Subject<T> valueSubject = new();
        
        public Observable<T> AsObservable() => valueSubject;
        public IObservable<T> AsSystemObservable() => valueSubject.AsSystemObservable();
        public IAsyncEnumerable<T> ToAsyncEnumerable(CancellationToken cancellationToken = default) => valueSubject.ToAsyncEnumerable(cancellationToken: cancellationToken);
    
        /// <summary>
        /// Raise the event with parameter.
        /// </summary>
        /// /// <param name="valueToRaise"></param>
        public virtual void Raise(T valueToRaise)
        {
            value = valueToRaise;
            base.Raise();
            valueSubject.OnNext(valueToRaise);
        }

        public IDisposable Subscribe(Action<T> action) => valueSubject.Subscribe(action);
        
        public new async ValueTask<T> EventAsync(CancellationToken cancellationToken = default)
        {
            var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, DefaultToken);
            return await valueSubject.FirstOrDefaultAsync(cancellationToken: linkedTokenSource.Token);
        }
        
        public override void Dispose()
        {
            valueSubject.Dispose();
            base.Dispose();
        }
    }
}

#endif

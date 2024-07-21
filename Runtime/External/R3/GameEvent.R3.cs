#if KASSETS_R3

using System;
using System.Collections.Generic;
using System.Threading;
using R3;

namespace Kadinche.Kassets.EventSystem
{
    public partial class GameEvent
    {
        private readonly Subject<object> _subject = new();
    }

    public abstract partial class GameEvent<T>
    {
        private readonly Subject<T> _valueSubject = new();
        
        public Observable<T> AsObservable() => _valueSubject;
        public IObservable<T> AsSystemObservable() => _valueSubject.AsSystemObservable();
        public IAsyncEnumerable<T> ToAsyncEnumerable(CancellationToken cancellationToken = default) => _valueSubject.ToAsyncEnumerable(cancellationToken: cancellationToken);
    }
    
    public partial class GameEvent
    {
        /// <summary>
        /// Raise the event.
        /// </summary>
        public virtual void Raise() => _subject.OnNext(this);
        public IDisposable Subscribe(Action action) => _subject.Subscribe(_ => action.Invoke());
        public override void Dispose()
        {
            _subject.Dispose();
            base.Dispose();
        }
    }
    
    public abstract partial class GameEvent<T>
    {
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
        
        public override void Dispose()
        {
            _valueSubject.Dispose();
            base.Dispose();
        }
    }
}

#endif

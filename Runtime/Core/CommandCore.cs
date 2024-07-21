using System;
using System.Threading;
using System.Threading.Tasks;

namespace Kadinche.Kassets.CommandSystem
{
    public abstract class CommandCore : KassetsCore, ICommand
    {
        protected CancellationTokenSource cts;
        
        public abstract void Execute();

        public virtual ValueTask ExecuteAsync(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                throw new OperationCanceledException();
            }
            
            Execute();
            return new ValueTask(Task.CompletedTask);
        }
        
        public ValueTask ExecuteAsync() => ExecuteAsync(cts.Token);
        
        protected override void Initialize()
        {
            cts = new CancellationTokenSource();
            base.Initialize();
        }

        public override void Dispose()
        {
            cts?.CancelAndDispose();
            cts = null;
        }
    }

    public abstract class CommandCore<T> : CommandCore, ICommand<T>
    {
        public override void Execute() => Execute(default);
        public abstract void Execute(T param);

        public virtual ValueTask ExecuteAsync(T param, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                throw new OperationCanceledException();
            }
            
            Execute(param);
            return new ValueTask(Task.CompletedTask);
        }
        
        public ValueTask ExecuteAsync(T param) => ExecuteAsync(param, cts.Token);
    }
}
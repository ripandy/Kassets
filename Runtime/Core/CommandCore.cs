using System.Threading;
using System.Threading.Tasks;

namespace Kadinche.Kassets.CommandSystem
{
    public abstract class CommandCore : KassetsCore
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
        
        public abstract void Execute();

        public virtual async ValueTask ExecuteAsync(CancellationToken cancellationToken)
        {
            var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, DefaultToken);
            var token = linkedTokenSource.Token;
            token.ThrowIfCancellationRequested();
            
            Execute();
            await Task.CompletedTask;
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

    public abstract class CommandCore<T> : CommandCore
    {
        public override void Execute() => Execute(default);
        public abstract void Execute(T param);

        public virtual async ValueTask ExecuteAsync(T param, CancellationToken cancellationToken)
        {
            var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, DefaultToken);
            var token = linkedTokenSource.Token;
            token.ThrowIfCancellationRequested();
            
            Execute(param);
            await Task.CompletedTask;
        }
        
        public async ValueTask ExecuteAsync(T param) => await ExecuteAsync(param, DefaultToken);
    }
}
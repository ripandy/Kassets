namespace Kadinche.Kassets.CommandSystem
{
    public abstract partial class CommandCore : KassetsCore, ICommand
    {
        public override void Dispose() { }
    }

    public abstract partial class CommandCore<T> : CommandCore, ICommand<T>
    {
    }

#if !KASSETS_UNITASK
    public abstract partial class CommandBase
    {
        public abstract void Execute();
    }

    public abstract partial class CommandBase<T>
    {
        public abstract void Execute(T param);
    }
#endif
}
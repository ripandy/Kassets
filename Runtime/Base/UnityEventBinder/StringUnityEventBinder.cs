namespace Kadinche.Kassets.EventSystem
{
    public sealed class StringUnityEventBinder : UnityEventBinder<string>
    {
        protected override void Start()
        {
            Subscriptions.Add(gameEventToListen.Subscribe(() => onTypedGameEventRaised.Invoke(gameEventToListen.ToString())));
        }
    }
}
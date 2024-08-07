using UnityEngine;

namespace Kadinche.Kassets.EventSystem
{
    public sealed class BoolUnityEventBinder : UnityEventBinder<bool>
    {
        [SerializeField] private TypedUnityEvent onNegatedBoolEventRaised;

        protected override void Start()
        {
            base.Start();
            if (gameEventToListen is GameEvent<bool> typedEvent)
            {
                Subscriptions.Add(typedEvent.Subscribe(value => onNegatedBoolEventRaised.Invoke(!value)));
            }
        }
    }
}
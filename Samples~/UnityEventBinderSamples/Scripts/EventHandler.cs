using System;
using Kadinche.Kassets.Variable;
using UnityEngine;

namespace Kadinche.Kassets.EventSystem.Sample
{
    public class EventHandler : MonoBehaviour
    {
        [SerializeField] private GameEvent gameEvent;
        [SerializeField] private StringVariable textVariable;
        [SerializeField] private string[] textsToDisplay = {
            "Every", "time", "event", "is", "fired", "a", "word", "is", "added", "to", "make", "a", "full", "sentence."
        };

        private int index;
        private IDisposable subscription;

        private void Start()
        {
            subscription = gameEvent.Subscribe(OnEventRaised);
        }

        private void OnEventRaised()
        {
            if (index >= textsToDisplay.Length)
            {
                textVariable.Value = "";
                index = 0;
            }
            
            textVariable.Value += textsToDisplay[index] + " ";
            index++;
        }

        private void OnDestroy()
        {
            subscription.Dispose();
        }
    }
}
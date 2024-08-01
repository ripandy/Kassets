using Kadinche.Kassets.EventSystem;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Kadinche.Kassets.Collection.Sample
{
    public class CollectionEventHandler : MonoBehaviour
    {
        [SerializeField] private GameEvent onAddValueClicked;
        [SerializeField] private GameEvent onAddGridClicked;
        [SerializeField] private GameEvent onRemoveGridClicked;
        [SerializeField] private IntCollection intCollection;

        private const int MaxGrid = 8;

        private void Start()
        {
            onAddValueClicked.Subscribe(UpdateRandomCollectionValue);
            onAddGridClicked.Subscribe(AddGrid);
            onRemoveGridClicked.Subscribe(RemoveGrid);
        }

        private void UpdateRandomCollectionValue()
        {
            if (intCollection.Count == 0)
                return;
            
            var randomIdx = Random.Range(0, intCollection.Count);
            intCollection[randomIdx]++;
        }

        private void AddGrid()
        {
            if (intCollection.Count >= MaxGrid)
                return;
            
            intCollection.Add(0);
        }

        private void RemoveGrid()
        {
            if (intCollection.Count == 0)
                return;
            
            intCollection.RemoveAt(intCollection.Count - 1);
        }
    }
}
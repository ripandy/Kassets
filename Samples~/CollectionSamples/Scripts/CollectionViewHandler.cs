using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Kadinche.Kassets.Collection.Sample
{
    public class CollectionViewHandler : MonoBehaviour
    {
        [SerializeField] private IntCollection intCollection;

        private readonly List<TMP_Text> texts = new();

        private void Awake()
        {
            GetComponentsInChildren(true, texts);
        }

        private void Start()
        {
            for (var i = 0; i < texts.Count; i++)
            {
                texts[i].transform.parent.gameObject.SetActive(i < intCollection.Count);
                
                var j = i;
                if (i < intCollection.Count)
                {
                    intCollection.SubscribeToValueAt(i, value => texts[j].text = "" + value);
                }
            }

            intCollection.SubscribeOnAdd(OnCollectionAdded);
            intCollection.SubscribeOnRemove(OnCollectionRemoved);
        }

        private void OnCollectionAdded(int addedValue)
        {
            var idx = intCollection.Count - 1;
            texts[idx].transform.parent.gameObject.SetActive(true);
            intCollection.SubscribeToValueAt(idx, value => texts[idx].text = texts[idx].text = "" + value);
        }
        
        private void OnCollectionRemoved(int removedValue)
        {
            var idx = intCollection.Count;
            texts[idx].transform.parent.gameObject.SetActive(false);
        }
    }
}
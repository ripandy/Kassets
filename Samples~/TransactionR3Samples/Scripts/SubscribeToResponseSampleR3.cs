using TMPro;
using UnityEngine;

namespace Kadinche.Kassets.Transaction.Sample
{
    public class SubscribeToResponseSampleR3 : MonoBehaviour
    {
        [SerializeField] private FloatTransaction dummyProcessTransaction;
        [SerializeField] private TMP_Text label;

#if KASSETS_R3
        private async void Start()
        {
            while (!destroyCancellationToken.IsCancellationRequested)
            {
                var response = await dummyProcessTransaction.WaitResponseAsync(destroyCancellationToken);
                label.text = $"Response received. Response value: {response}";
            }
        }
#else
        private void Start()
        {
            Debug.LogError("UniTask not found. Please import UniTask first");
        }
#endif
    }
}
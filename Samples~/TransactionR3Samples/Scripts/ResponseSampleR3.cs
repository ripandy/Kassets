using UnityEngine;

#if KASSETS_R3
using System;
using System.Threading.Tasks;
using Random = UnityEngine.Random;
#endif

namespace Kadinche.Kassets.Transaction.Sample
{
    public class ResponseSampleR3 : MonoBehaviour
    {
        [SerializeField] private FloatTransaction dummyProcessTransaction;

#if KASSETS_R3
        private IDisposable subscription;

        private void Start()
        {
            subscription = dummyProcessTransaction.RegisterResponse(ProcessRequest);
        }

        private async ValueTask<float> ProcessRequest(float requestValue)
        {
            var delay = requestValue + Random.value * 3;
            await Task.Delay(TimeSpan.FromSeconds(delay), cancellationToken: destroyCancellationToken);
            return delay;
        }

        private void OnDestroy()
        {
            subscription?.Dispose();
        }
#else
        private void Start()
        {
            Debug.LogError("R3 not found. Please import R3 first");
        }
#endif
    }
}
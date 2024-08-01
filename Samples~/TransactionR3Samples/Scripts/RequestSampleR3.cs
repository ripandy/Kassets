using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Kadinche.Kassets.Transaction.Sample
{
    public class RequestSampleR3 : MonoBehaviour
    {
        [SerializeField] private FloatTransaction dummyProcessTransaction;
        [SerializeField] private Button button;
        [SerializeField] private TMP_Text processValue;
        private TMP_Text buttonLabel;

#if KASSETS_R3
        private void Awake()
        {
            buttonLabel = button.GetComponentInChildren<TMP_Text>();
        }

        private void Start()
        {
            button.onClick.AddListener(BeginRequest);
        }

        private async void BeginRequest() => await BeginRequestAsync();

        private async ValueTask BeginRequestAsync()
        {
            button.interactable = false;
            buttonLabel.text = "Waiting for Response..";
            
            var responseValue = await dummyProcessTransaction.RequestAsync(Random.value * 3f);
            
            button.interactable = true;
            buttonLabel.text = "Request";
            processValue.text = $"Time elapsed: {responseValue}";
        }
#else
        private void Start()
        {
            Debug.LogError("R3 not found. Please import R3 first");
        }
#endif
    }
}
using UnityEngine;
using UnityEngine.UI;
using Zenject;

/// <summary>
/// Как только запускается UI панель настройки слоёв нейросети нужно сказать этой панели сколько нейронов у нас на первом слое, а сколько на выходном.
/// </summary>
class NetworkConfigPanel : MonoBehaviour
{
    [Inject] DatasetValidator datasetValidator;

    [Header("Размерности входного и выходного слоёв нейросети:")]
    [SerializeField] InputField inputDim;
    [SerializeField] InputField outDim;

    private void OnEnable()
    {
        if (datasetValidator.isValid)
            InitFirstAndLastLayer();
        else
            Debug.LogWarning("Без валидации датасета неизвестно сколько слоёв на первом и последнем слое.");
    }

    private void OnDisable()
    {
        
    }

    void InitFirstAndLastLayer()
    {
        int inputValue = datasetValidator.imageSize[0] * datasetValidator.imageSize[1];
        int outValue = datasetValidator.categoryNames.Count;

        inputDim.text = inputValue.ToString();
        outDim.text = outValue.ToString();
    }
}
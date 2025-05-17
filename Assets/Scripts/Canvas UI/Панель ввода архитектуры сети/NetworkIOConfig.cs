using UnityEngine;
using UnityEngine.UI;
using Zenject;

/// <summary>
/// Инициализирует поля ввода для размерности входного и выходного слоёв нейросети на UI панели настроек.
/// </summary>
class NetworkIOConfig : MonoBehaviour
{
    [Inject] DatasetValidator datasetValidator;
    [Inject] Network network;

    [Header("Размерности входного и выходного слоёв нейросети:")]
    [SerializeField] InputField inputDim;
    [SerializeField] InputField outDim;

    private void OnEnable()
    {
        InitFirstAndLastLayer();
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
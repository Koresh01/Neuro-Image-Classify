using UnityEngine;
using UnityEngine.UI;
using Zenject;

/// <summary>
/// Инициализирует поля ввода для размерности входного и выходного слоёв нейросети на UI панели настроек.
/// </summary>
class NetworkIOConfig : MonoBehaviour
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
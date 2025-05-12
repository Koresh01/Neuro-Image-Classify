using UnityEngine;
using UnityEngine.UI;
using Zenject;
using System.Collections.Generic;

/// <summary>
/// Обрабатывает действия пользователя по настройке архитектуры нейросети: количество и размеры скрытых слоёв.
/// </summary>
class NetworkConfigSaver : MonoBehaviour
{
    [Inject] private DatasetValidator datasetValidator;
    [Inject] private Network network;

    [Header("Vertical Scroll View")]
    [SerializeField] private Transform content;

    [Header("Сохранение")]
    [SerializeField] private Button saveButton;

    private void OnEnable()
    {
        saveButton.onClick.AddListener(SaveConfig);
    }

    private void OnDisable()
    {
        saveButton.onClick.RemoveListener(SaveConfig);
    }

    /// <summary>
    /// Считывает данные из UI и пересоздаёт архитектуру нейросети.
    /// </summary>
    private void SaveConfig()
    {
        int inputDim = datasetValidator.imageSize[0] * datasetValidator.imageSize[1];
        int outputDim = datasetValidator.categoryNames.Count;

        List<Matrix> layers = new List<Matrix>();

        // Входной слой
        layers.Add(Numpy.Zeros(1, inputDim));

        // Промежуточные слои
        for (int i = 0; i < content.childCount; i++)
        {
            var inputField = content.GetChild(i).GetComponentInChildren<InputField>();

            if (!int.TryParse(inputField.text, out int neurons) || neurons <= 0)
            {
                Debug.LogWarning($"Некорректное значение нейронов в слое {i}: '{inputField.text}'");
                continue;
            }

            layers.Add(Numpy.Zeros(1, neurons));
        }

        // Выходной слой
        layers.Add(Numpy.Zeros(1, outputDim));

        /*
         layers - это есть ни что иное как t[] слои нейросети. Надо теперь как то занова инициализировать нейронную сеть...
         */

        // Передаём конфигурацию в сеть
        network.SetArchitecture(layers);
    }
}

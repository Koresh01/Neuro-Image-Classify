using UnityEngine;
using UnityEngine.UI;
using Zenject;
using System.Collections.Generic;

/// <summary>
/// Считывает настройки архитектуры нейросети из UI и обновляет конфигурацию слоёв.
/// </summary>
class NetworkConfigSaver : MonoBehaviour
{
    [Inject] DatasetValidator datasetValidator;
    [Inject] Network network;
    [Inject] NetworkVizualizer networkVizualizer;

    [Header("Vertical Scroll View")]
    [SerializeField] private Transform content;

    [Header("UI панель архитектуры слоёв сети")]
    [SerializeField] private GameObject architecturePanel;

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
        List<Matrix> t_midterm = new List<Matrix>();
        t_midterm = GenerateMidternLayers();
        

        // Передаём конфигурацию в сеть
        network.Init(t_midterm);
        architecturePanel.SetActive(false);
        networkVizualizer.Vizualize();
    }

    /// <summary>
    /// Создаёт промежуточные слои нейросети. То есть без входного слоя и выходного, основывается при этом на введённые пользователем значения в ScrollView.
    /// </summary>
    List<Matrix> GenerateMidternLayers()
    {
        List<Matrix> t_midterm = new List<Matrix>();    // Из этой UI панели для настройки слоёв нейросети пользователем, берем только информацию о желаемом кол-ве промежуточных слоёв.

        // Входной слой
        // t.Add(Numpy.Zeros(1, inputDim));

        // Промежуточные слои
        for (int i = 0; i < content.childCount; i++)
        {
            var inputField = content.GetChild(i).GetComponentInChildren<InputField>();

            if (!int.TryParse(inputField.text, out int neurons) || neurons <= 0)
            {
                Debug.LogWarning($"Некорректное значение нейронов в слое {i}: '{inputField.text}'");
                continue;
            }

            t_midterm.Add(Numpy.Zeros(1, neurons));
        }

        // Выходной слой
        // t_midterm.Add(Numpy.Zeros(1, outputDim));

        return t_midterm;
    }
}

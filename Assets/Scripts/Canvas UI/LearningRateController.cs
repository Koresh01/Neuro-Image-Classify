using UnityEngine;
using UnityEngine.UI;
using Zenject;

/// <summary>
/// Управляет пользовательским интерфейсом для задания и применения шага обучения нейросети.
/// Позволяет вводить значение вручную или изменять его с помощью кнопок, 
/// а также применять выбранное значение к нейросети.
/// </summary>
public class LearningRateController : MonoBehaviour
{
    [Inject] private Network network;

    [Header("UI Components")]
    [SerializeField] private InputField learningRateInput;

    [SerializeField] private Button increase1;
    [SerializeField] private Button increase2;
    [SerializeField] private Button increase3;

    [SerializeField] private Button decrease1;
    [SerializeField] private Button decrease2;
    [SerializeField] private Button decrease3;

    [SerializeField] private Button applyButton;

    private void Awake()
    {
        increase1.onClick.AddListener(() => AdjustLearningRate(0.1f));
        increase2.onClick.AddListener(() => AdjustLearningRate(0.5f));
        increase3.onClick.AddListener(() => AdjustLearningRate(1.0f));

        decrease1.onClick.AddListener(() => AdjustLearningRate(-0.1f));
        decrease2.onClick.AddListener(() => AdjustLearningRate(-0.5f));
        decrease3.onClick.AddListener(() => AdjustLearningRate(-1.0f));

        applyButton.onClick.AddListener(ApplyLearningRate);
    }

    private void OnEnable()
    {
        // Устанавливаем текущее значение шага обучения в поле ввода
        learningRateInput.text = network.learningRate.ToString("F3");
    }

    /// <summary>
    /// Изменяет значение в input field
    /// </summary>
    /// <param name="delta"></param>
    private void AdjustLearningRate(float delta)
    {
        if (float.TryParse(learningRateInput.text, out float current))
        {
            current += delta;
            current = Mathf.Max(0f, (float)current); // предотвращаем отрицательные значения
            learningRateInput.text = current.ToString("F3");
        }
    }

    /// <summary>
    /// Устанавливает шаг обучения.
    /// </summary>
    private void ApplyLearningRate()
    {
        if (float.TryParse(learningRateInput.text, out float value))
        {
            network.learningRate = value;
            Debug.Log($"Learning rate set to: {value}");

            gameObject.SetActive(false);    // выключаем панельку с шагом.
        }
        else
        {
            Debug.LogWarning("Invalid input for learning rate.");
        }
    }
}

using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

[AddComponentMenu("Custom/LoadingPanel (Логика слайдеров загрузки)")]
public class LoadingPanel : MonoBehaviour
{
    [Inject] DatasetValidator datasetValidator;

    [Tooltip("Слайдер прогресса загрузки ОБУЧАЮЩЕЙ выборки изображений")]
    [SerializeField] Slider trainLoadProgress;

    [Tooltip("Слайдер прогресса загрузки ТЕСТОВОЙ выборки изображений")]
    [SerializeField] Slider testLoadProgress;

    [Tooltip("Кнопка продолжить")]
    [SerializeField] Button continueBtn;

    [Tooltip("Заключение анализа:")]
    [SerializeField] TextMeshProUGUI verdict;

    void OnEnable()
    {
        datasetValidator.OnReady += EnableContinueButton;
    }

    void OnDisable()
    {
        datasetValidator.OnReady -= EnableContinueButton;
        verdict.text = "Вердикт:" + "  ---";
        continueBtn.interactable = false;
    }

    void Update()
    {
        trainLoadProgress.value = datasetValidator.trainProgress;
        testLoadProgress.value = datasetValidator.testProgress;
    }

    /// <summary>
    /// Меняет панель, когда загрузка датасета завершена.
    /// </summary>
    void EnableContinueButton()
    {
        continueBtn.interactable = true;
        verdict.text = "Вердикт:" + datasetValidator.verdict;
    }
}

using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

[AddComponentMenu("Custom/LoadingPanel (Логика слайдеров загрузки)")]
public class DatasetValidationPanel : MonoBehaviour
{
    [Inject] DatasetValidator datasetValidator;

    [Tooltip("Слайдер прогресса загрузки.")]
    [SerializeField] Slider loadProgress;

    [Tooltip("Кнопка продолжить")]
    [SerializeField] Button continueBtn;

    [Tooltip("Заключение анализа:")]
    [SerializeField] TextMeshProUGUI verdict;

    void OnEnable()
    {
        datasetValidator.onReady += EnableContinueButton;
    }

    void OnDisable()
    {
        datasetValidator.onReady -= EnableContinueButton;

        verdict.text = "Вердикт:" + "  ---";
        continueBtn.interactable = false;
    }

    void Update()
    {
        loadProgress.value = datasetValidator.loadProgress;
    }

    /// <summary>
    /// Меняет панель, когда загрузка датасета завершена.
    /// </summary>
    void EnableContinueButton()
    {
        continueBtn.interactable = true;
        verdict.text = "Вердикт:\n" + datasetValidator.verdict;
    }
}

using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

[AddComponentMenu("Custom/LoadingPanel (������ ��������� ��������)")]
public class LoadingPanel : MonoBehaviour
{
    [Inject] DatasetValidator datasetValidator;

    [Tooltip("������� ��������� �������� ��������� ������� �����������")]
    [SerializeField] Slider trainLoadProgress;

    [Tooltip("������� ��������� �������� �������� ������� �����������")]
    [SerializeField] Slider testLoadProgress;

    [Tooltip("������ ����������")]
    [SerializeField] Button continueBtn;

    [Tooltip("���������� �������:")]
    [SerializeField] TextMeshProUGUI verdict;

    void OnEnable()
    {
        datasetValidator.OnReady += EnableContinueButton;
    }

    void OnDisable()
    {
        datasetValidator.OnReady -= EnableContinueButton;
        verdict.text = "�������:" + "  ---";
        continueBtn.interactable = false;
    }

    void Update()
    {
        trainLoadProgress.value = datasetValidator.trainProgress;
        testLoadProgress.value = datasetValidator.testProgress;
    }

    /// <summary>
    /// ������ ������, ����� �������� �������� ���������.
    /// </summary>
    void EnableContinueButton()
    {
        continueBtn.interactable = true;
        verdict.text = "�������:" + datasetValidator.verdict;
    }
}

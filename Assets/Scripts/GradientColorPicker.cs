using UnityEngine;

public class GradientColorPicker : MonoBehaviour
{
    [Header("�������� �� 0 �� 1")]
    [SerializeField]
    private Gradient activatedGradient;

    [Header("�������� �� 0 �� maxValue")]   // ������������ �������� � �������� �����
    [SerializeField]
    private Gradient nonActivatedGradient;

    /// <summary>
    /// �������� ���� ��������������� �������, ��� ��� �� 0 �� 1.
    /// </summary>
    /// <param name="normalizedValue">�������� �� 0 �� 1.</param>
    public Color GetActivatedColor(float normalizedValue)
    {
        // ������ �� ������ �� �������
        normalizedValue = Mathf.Clamp01(normalizedValue);
        return activatedGradient.Evaluate(normalizedValue);
    }
}

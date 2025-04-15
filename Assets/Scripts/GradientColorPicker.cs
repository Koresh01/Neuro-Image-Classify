using UnityEngine;

public class GradientColorPicker : MonoBehaviour
{
    [Header("�������� �� 0 �� 1")]
    [SerializeField]
    private Gradient gradient;

    /// <summary>
    /// �������� ���� �� ��������� �� ���������������� �������� �� 0 �� 1.
    /// </summary>
    /// <param name="normalizedValue">�������� �� 0 �� 1.</param>
    public Color GetColor(float normalizedValue)
    {
        // ������ �� ������ �� �������
        normalizedValue = Mathf.Clamp01(normalizedValue);
        return gradient.Evaluate(normalizedValue);
    }
}

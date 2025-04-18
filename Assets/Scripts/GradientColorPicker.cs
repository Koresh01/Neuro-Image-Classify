using UnityEngine;

public class GradientColorPicker : MonoBehaviour
{
    [Header("Градиент от 0 до 1")]
    [SerializeField]
    private Gradient activatedGradient;

    [Header("Градиент от -maxAbsValue до +maxAbsValue")]
    [SerializeField]
    private Gradient nonActivatedGradient;

    [Tooltip("Максимальное абсолютное значение весов (используется для нормализации от -n до +n)")]
    [SerializeField]
    private float maxAbsValue = 1f;

    /// <summary>
    /// Получить цвет АКТИВИРОВАННОГО НЕЙРОНА, чей вес от 0 до 1.
    /// </summary>
    /// <param name="normalizedValue">Значение от 0 до 1.</param>
    public Color GetActivatedColor(float normalizedValue)
    {
        normalizedValue = Mathf.Clamp01(normalizedValue);
        return activatedGradient.Evaluate(normalizedValue);
    }

    /// <summary>
    /// Получить цвет НЕАКТИВИРОВАННОГО НЕЙРОНА, чей вес от -maxAbsValue до +maxAbsValue.
    /// </summary>
    /// <param name="value">Значение от -maxAbsValue до +maxAbsValue.</param>
    public Color GetNonActivatedColor(float value)
    {
        // Ограничим значение
        value = Mathf.Clamp(value, -maxAbsValue, maxAbsValue);

        // Преобразуем [-maxAbsValue, maxAbsValue] -> [0, 1]
        float t = (value + maxAbsValue) / (2f * maxAbsValue);
        return nonActivatedGradient.Evaluate(t);
    }
}

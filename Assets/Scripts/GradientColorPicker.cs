using UnityEngine;

public class GradientColorPicker : MonoBehaviour
{
    [Header("Градиент от 0 до 1 (для активированного нейрона)")]
    [SerializeField] private Gradient activatedGradient;

    [Header("Градиент от -value до +value (для НЕ активированного нейрона)")]
    [SerializeField] private float maxAbsColorValue = float.MaxValue;
    [SerializeField] private Gradient nonActivatedGradient;

    [Header("Градиент от -delta до +delta")]
    [Tooltip("Максимально ожидаемое изменение веса")]
    [SerializeField] private float maxAllowedDelta = 0.001f;

    [SerializeField] private Gradient deltaGradient;

    /// <summary>
    /// Получить цвет АКТИВИРОВАННОГО НЕЙРОНА.
    /// </summary>
    public Color GetActivatedColor(float normalizedValue)
    {
        return activatedGradient.Evaluate(Mathf.Clamp01(normalizedValue));
    }

    /// <summary>
    /// Получить цвет НЕ АКТИВИРОВАННОГО НЕЙРОНА.
    /// Значение нормализуется в диапазон [-maxAbsColorValue, +maxAbsColorValue] → [0, 1].
    /// </summary>
    public Color GetNonActivatedColor(float value)
    {
        if (maxAbsColorValue <= 0f)
        {
            Debug.LogWarning("maxAbsColorValue должен быть положительным");
            return Color.magenta;
        }

        // Ограничим значение, чтобы избежать выхода за границы
        value = Mathf.Clamp(value, -maxAbsColorValue, maxAbsColorValue);

        // Преобразуем [-maxAbsColorValue, +maxAbsColorValue] → [0, 1]
        float normalized = Mathf.InverseLerp(-maxAbsColorValue, maxAbsColorValue, value);
        return nonActivatedGradient.Evaluate(normalized);
    }

    /// <summary>
    /// Получить цвет по дельте изменения веса.
    /// </summary>
    public Color GetDeltaColor(float delta)
    {
        // Приводим дельту к диапазону [-maxAllowedDelta, +maxAllowedDelta] → [0, 1]
        float normalized = Mathf.InverseLerp(-maxAllowedDelta, maxAllowedDelta, delta);
        return deltaGradient.Evaluate(normalized);
    }
}

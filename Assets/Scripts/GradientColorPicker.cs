using UnityEngine;

public class GradientColorPicker : MonoBehaviour
{
    [Header("Градиент от 0 до 1 (для активации нейрона)")]
    [SerializeField] private Gradient activatedGradient;

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
    /// Получить цвет по дельте изменения веса.
    /// </summary>
    public Color GetDeltaColor(float delta)
    {
        // Приводим дельту к диапазону [-maxAllowedDelta, +maxAllowedDelta] → [0, 1]
        float normalized = Mathf.InverseLerp(-maxAllowedDelta, maxAllowedDelta, delta);
        return deltaGradient.Evaluate(normalized);
    }
}

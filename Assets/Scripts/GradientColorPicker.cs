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
        value = Mathf.Abs(value);

        float normalizedValue = 0;
        if (value > maxAbsValue)
            value = 1;
        else if (value < 0)
            normalizedValue = 0;
        else
            normalizedValue = value / maxAbsValue;
        return nonActivatedGradient.Evaluate(normalizedValue);
    }
}

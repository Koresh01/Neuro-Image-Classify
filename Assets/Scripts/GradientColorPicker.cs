using UnityEngine;

public class GradientColorPicker : MonoBehaviour
{
    [Header("Градиент от 0 до 1")]
    [SerializeField]
    private Gradient activatedGradient;

    [Header("Градиент от 0 до maxValue")]   // максимальное значение в матрицах весов
    [SerializeField]
    private Gradient nonActivatedGradient;

    /// <summary>
    /// Получить цвет АКТИВИРОВАННОГО НЕЙРОНА, чей вес от 0 до 1.
    /// </summary>
    /// <param name="normalizedValue">Значение от 0 до 1.</param>
    public Color GetActivatedColor(float normalizedValue)
    {
        // Защита от выхода за границы
        normalizedValue = Mathf.Clamp01(normalizedValue);
        return activatedGradient.Evaluate(normalizedValue);
    }
}

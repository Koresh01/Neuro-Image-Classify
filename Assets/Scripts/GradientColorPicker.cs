using UnityEngine;

public class GradientColorPicker : MonoBehaviour
{
    [Header("Градиент от 0 до 1")]
    [SerializeField]
    private Gradient gradient;

    /// <summary>
    /// Получить цвет из градиента по нормализованному значению от 0 до 1.
    /// </summary>
    /// <param name="normalizedValue">Значение от 0 до 1.</param>
    public Color GetColor(float normalizedValue)
    {
        // Защита от выхода за границы
        normalizedValue = Mathf.Clamp01(normalizedValue);
        return gradient.Evaluate(normalizedValue);
    }
}

using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Скрипт для управления элементом настройки слоя нейросети, который позволяет пользователю задать количество нейронов для каждого слоя через UI.
/// </summary>
public class LayerSetupElement : MonoBehaviour
{
    [SerializeField] InputField neironsNumber;
    [SerializeField] Button delBtn;

    private void OnEnable()
    {
        delBtn.onClick.AddListener(DestroySelf);
    }

    private void OnDisable()
    {
        delBtn.onClick.RemoveAllListeners();
    }

    /// <summary>
    /// Самоуничтожение элемента scroll view.
    /// </summary>
    void DestroySelf()
    {
        Destroy(gameObject);
    }
}

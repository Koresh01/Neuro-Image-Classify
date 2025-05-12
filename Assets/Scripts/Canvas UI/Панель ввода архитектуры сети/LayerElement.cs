using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// У нас есть UI панель для настройки слоёв и нейронов пользователем. На этой панели имеется vertical scroll view с образцами слоёв нейросети, где можно вписать кол-во нейронов конкретного слоя. Текущий скрипт висит на каждом образце слоя.
/// </summary>
public class LayerElement : MonoBehaviour
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

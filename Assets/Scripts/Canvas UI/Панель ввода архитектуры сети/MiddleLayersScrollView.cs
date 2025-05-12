using Zenject;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Контролирует отображение скрытых слоёв нейросети в вертикальном ScrollView.
/// </summary>
public class MiddleLayersScrollView : MonoBehaviour
{
    [Inject] private Network network;

    [Header("Vertical Scroll View")]
    [SerializeField] private Transform content;

    [Header("Префаб элемента слоя")]
    [SerializeField] private GameObject layerPrefab;

    [Header("+/- слой")]
    [SerializeField] private Button addButton;

    private void OnEnable()
    {
        Refresh();
        addButton.onClick.AddListener(AddLayer);
    }

    private void OnDisable()
    {
        addButton.onClick.RemoveListener(AddLayer);
    }

    /// <summary>
    /// Очищает ScrollView и заполняет его слоями из сети.
    /// </summary>
    private void Refresh()
    {
        ClearContent();

        // Пропускаем входной (0) и выходной (последний) слой
        for (int i = 1; i < network.t.Count - 1; i++)
        {
            var layer = Instantiate(layerPrefab, content);
            layer.GetComponentInChildren<InputField>().text = network.t[i].GetLength(1).ToString();
        }
    }

    /// <summary>
    /// Удаляет все дочерние элементы из контента.
    /// </summary>
    private void ClearContent()
    {
        for (int i = content.childCount - 1; i >= 0; i--)
        {
            Destroy(content.GetChild(i).gameObject);
        }
    }

    /// <summary>
    /// Добавляет новый слой в ScrollView.
    /// </summary>
    private void AddLayer()
    {
        Instantiate(layerPrefab, content);
    }
}

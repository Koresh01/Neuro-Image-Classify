using Zenject;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Контроллер промежуточных слоёв нейросети. На панели настройки этих слоёв пользователем имеется Vertical Scroll View, задача этого скрипта синхронизировать работу этого вертикального списка с тем что там вводится.
/// </summary>
public class ScrollViewOfMiddleLayers : MonoBehaviour
{
    [Header("Vertical Scroll View:")]
    [SerializeField] Transform content;

    [Header("Префаб эл-та вертикального списка:")]
    [SerializeField] GameObject layerObj;

    [Header("+/- слой:")]
    [SerializeField] Button addLayer;

    void OnEnable()
    {
        addLayer.onClick.AddListener(AddLayer);
    }

    void OnDisable()
    {
        addLayer.onClick.RemoveAllListeners();
    }

    


    /// <summary>
    /// Добавляет образец слоя в scroll view.
    /// </summary>
    void AddLayer()
    {
        GameObject obj = Instantiate(layerObj, content);
    }
}

using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("Custom/PopUpPanelsController (Контроллер всплывающих панелей.)")]
public class PopUpPanelsController : MonoBehaviour
{
    [Header("Всплывающие панели:")]
    [SerializeField] private List<GameObject> panels;

    private void Start()
    {
        foreach (GameObject panel in panels)
        {
            Button closeBtn = panel.transform.Find("Button (Закрыть панельку)").GetComponent<Button>();

            // Используем лямбда-выражение для передачи параметра
            closeBtn.onClick.AddListener(() => ClosePanel(panel));

        }
    }

    private GameObject GetPanel(string name)
    {
        foreach (GameObject panel in panels)
        {
            if (panel.name == name)
                return panel;
        }

        throw new ArgumentException("Не нашли панель с таким именем");
    }

    private void ClosePanel(GameObject panel)
    {
        panel.SetActive(false);
    }

    /// <summary>
    /// Закрывает панель.
    /// </summary>
    /// <param name="name">Название панели.</param>
    public void ClosePanel(string name)
    {
        GameObject panel = GetPanel(name);
        panel.SetActive(false);
    }

    /// <summary>
    /// Показывает панель с указанным именем.
    /// </summary>
    /// <param name="name">Имя панели.</param>
    public void ShowPanel(string name)
    {

        GameObject panel = GetPanel(name);
        panel.SetActive(true);
    }
}

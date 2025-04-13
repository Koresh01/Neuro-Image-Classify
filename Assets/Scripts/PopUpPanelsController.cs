using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("Custom/PopUpPanelsController (���������� ����������� �������.)")]
public class PopUpPanelsController : MonoBehaviour
{
    [Header("����������� ������:")]
    [SerializeField] private List<GameObject> panels;

    private void Start()
    {
        foreach (GameObject panel in panels)
        {
            Button closeBtn = panel.GetComponentInChildren<Button>();
            
            // ���������� ������-��������� ��� �������� ���������
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

        throw new ArgumentException("�� ����� ������ � ����� ������");
    }

    private void ClosePanel(GameObject panel)
    {
        panel.SetActive(false);
    }

    /// <summary>
    /// ��������� ������.
    /// </summary>
    /// <param name="name">�������� ������.</param>
    public void ClosePanel(string name)
    {
        GameObject panel = GetPanel(name);
        panel.SetActive(false);
    }

    /// <summary>
    /// ���������� ������ � ��������� ������.
    /// </summary>
    /// <param name="name">��� ������.</param>
    public void ShowPanel(string name)
    {

        GameObject panel = GetPanel(name);
        panel.SetActive(true);
    }
}

using TMPro;
using UnityEngine;
using Zenject;
using System.Collections.Generic;

/// <summary>
/// ������������ �������� ��������� ����� � �������� ����� ���������.
/// </summary>
public class CategoryLabelsDrawer : MonoBehaviour
{
    [Inject] private DatasetValidator datasetValidator;
    [Inject] private ActivatedLayersGenerator activatedLayersGenerator;
    [Inject] private Network network;

    [SerializeField] private GameObject textPrefab; // ������ � ����������� TextMeshPro (3D)
    [SerializeField] private Vector3 offsetFromLayer = new Vector3(0, -1f, 1.2f); // �������� �������� �� ���������� ����

    private readonly List<GameObject> spawnedLabels = new(); // ������ ��������� ��������, ����� ����� �� �������

    /// <summary>
    /// ������������ �������� ���������, �������������� ������ ������.
    /// </summary>
    public void GenerateLabels()
    {
        ClearPreviousLabels();

        if (datasetValidator.categoryNames == null || datasetValidator.categoryNames.Count == 0)
        {
            Debug.LogWarning("������ ��������� ���� ��� �� �����.");
            return;
        }

        float spacingX = activatedLayersGenerator.pixelSpacing;
        float spacingZ = activatedLayersGenerator.pixelSpacing_Z;
        int categoryCount = datasetValidator.categoryNames.Count;
        int outputLayerIndex = network.t.Count - 1;

        // ������ ������ ���������
        Vector3 basePosition = new Vector3(
            -(categoryCount / 2f) * spacingX,
            offsetFromLayer.y,
            outputLayerIndex * spacingZ + offsetFromLayer.z
        );

        for (int i = 0; i < categoryCount; i++)
        {
            string category = datasetValidator.categoryNames[i];
            Vector3 position = basePosition + new Vector3(i * spacingX, 0, 0);

            GameObject labelObj = Instantiate(textPrefab, position, Quaternion.identity, transform);
            labelObj.name = $"Label_{category}";
            spawnedLabels.Add(labelObj);

            if (labelObj.TryGetComponent(out TextMeshPro tmp))
            {
                tmp.text = category;
                tmp.color = Color.white;
            }
            else
            {
                Debug.LogError("������ �� �������� ��������� TextMeshPro.");
            }
        }
    }

    /// <summary>
    /// ������� ��� ����� ��������� �������.
    /// </summary>
    private void ClearPreviousLabels()
    {
        foreach (var label in spawnedLabels)
        {
            if (label != null)
                Destroy(label);
        }

        spawnedLabels.Clear();
    }
}

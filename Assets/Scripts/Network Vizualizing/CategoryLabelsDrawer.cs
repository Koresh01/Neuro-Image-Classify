using TMPro;
using UnityEngine;
using Zenject;

/// <summary>
/// ������������ �������� ��������� ����� � ���������(��������) ����� ���������.
/// </summary>
public class CategoryLabelsDrawer : MonoBehaviour
{
    [Inject] DatasetValidator datasetValidator;
    public GameObject textPrefab; // ������ � TextMeshPro (3D)
    public Vector3 startPosition = new Vector3(0, 0, 0);
    public float verticalSpacing = 1.5f;

    /// <summary>
    /// ������������ �������� ���������.
    /// </summary>
    public void GenerateLabels()
    {
        for (int i = 0; i < datasetValidator.categoryNames.Count; i++)
        {
            string category = datasetValidator.categoryNames[i];
            Vector3 position = startPosition + new Vector3(0, -i * verticalSpacing, 0);

            GameObject label = Instantiate(textPrefab, position, Quaternion.identity, transform);
            label.name = $"Label_{category}";

            TextMeshPro tmp = label.GetComponent<TextMeshPro>();
            if (tmp != null)
            {
                tmp.text = category;
                // tmp.fontSize = 1.5f;
                tmp.color = Color.white;
            }
            else
            {
                Debug.LogError("Prefab �� �������� TextMeshPro ���������.");
            }
        }
    }
}

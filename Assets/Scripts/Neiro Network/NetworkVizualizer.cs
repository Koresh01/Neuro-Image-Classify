using UnityEngine;
using System.Collections.Generic;
using Zenject;
using static UnityEditor.PlayerSettings;
using System.Linq;

[AddComponentMenu("Custom/Network Vizualizer (��������� ��������� ����)")]
public class NetworkVizualizer : MonoBehaviour
{
    [Tooltip("��������� ���� �� ����� ������/������...")]
    [Inject] Network network;

    [Tooltip("��������� ��������.")]
    [Inject] DatasetValidator datasetValidator;

    [Header("Transform-��������� ��� ��������")]
    [SerializeField] Transform pixelsContainer;

    [Header("������ �������.")]
    [SerializeField] GameObject pixelPrefab;
    

    [Range(1f, 2f), SerializeField] float pixelsOffset;
    [Range(1f, 40f), SerializeField] float pixelsZoffset;

    private List<GameObject> lines3D;
    private List<GameObject> pixels3D;

    private void Start()
    {
        pixels3D = new List<GameObject>();
        lines3D = new List<GameObject>();
    }

    void OnEnable()
    {
        network.onReady += CreatePixels;
    }

    void OnDisable()
    {
        network.onReady -= CreatePixels;
    }

    void CreatePixels()
    {
        // �������� �������:
        int Layers = network.t.Count;
        for (int layer = 0; layer < Layers; layer++)
        {
            // ������ ������� �����������
            if (layer == 0)
            {
                Vector2 imgSize = datasetValidator.imageSize;
                for (int x = 0; x < imgSize.x; x++)
                {
                    for (int y = 0; y < imgSize.y; y++)
                    {
                        float startX = -imgSize.x / 2f;
                        float startY = -imgSize.y / 2f;

                        float xPos = (startX + x) * pixelsOffset;
                        float yPos = (startY + y) * pixelsOffset;
                        float zPos = layer * pixelsZoffset;

                        Vector3 pixelPos = new Vector3(xPos, yPos, zPos);
                        pixelPos *= pixelsOffset;   // ������ �� �������� ��� ����������?
                        pixels3D.Add(Instantiate(pixelPrefab, pixelPos, Quaternion.identity, pixelsContainer)); // � ���� ������ �������� �� ������...

                        GameObject pixel = pixels3D[pixels3D.Count - 1];
                        pixel.GetComponent<PixelColorSetter>().SetColor(Color.green);   // ���� ���������� ������ ����
                    }
                }
            }

            // ������ ������������� ����:
            if (layer > 0 && layer < Layers - 1) { 
                int width = (int)Mathf.Sqrt(network.t[layer].GetLength(1));
                Vector2 imageSize = new Vector2(width, width);
                for (int x = 0; x < imageSize.x; x++)
                {
                    for (int y = 0; y < imageSize.y; y++)
                    {
                        float startX = -imageSize.x / 2f;
                        float startY = -imageSize.y / 2f;

                        float xPos = (startX + x) * pixelsOffset;
                        float yPos = (startY + y) * pixelsOffset;
                        float zPos = layer * pixelsZoffset;

                        Vector3 pixelPos = new Vector3(xPos, yPos, zPos);
                        pixels3D.Add(Instantiate(pixelPrefab, pixelPos, Quaternion.identity, pixelsContainer));
                    }
                }
            }

            // ������ �������� ������
            int OUT_DIM = datasetValidator.categoryNames.Count;
            if (layer == Layers - 1)
            {
                for (int x = 0; x < OUT_DIM; x++)
                {
                    float startX = -OUT_DIM / 2f;

                    float xPos = (startX + x) * pixelsOffset;
                    float zPos = layer * pixelsZoffset;

                    Vector3 pixelPos = new Vector3(xPos, 0, zPos);
                    pixels3D.Add(Instantiate(pixelPrefab, pixelPos, Quaternion.identity, pixelsContainer));
                }
            }
        }
    }
}

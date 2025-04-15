using UnityEngine;
using System.Collections.Generic;
using Zenject;

[AddComponentMenu("Custom/Network Vizualizer (Отрисовка состояния сети)")]
public class NetworkVizualizer : MonoBehaviour
{
    [Tooltip("Нейронная сеть со всеми слоями/весами...")]
    [Inject] Network network;

    [Tooltip("Получатель цвета:")]
    [Inject] GradientColorPicker gradientColorPicker;

    [Tooltip("Валидатор датасета.")]
    [Inject] DatasetValidator datasetValidator;

    [Header("Transform-контейнер для пикселей")]
    [SerializeField] Transform pixelsContainer;

    [Header("Transform-контейнер для линий")]
    [SerializeField] Transform connectionContainer;

    [Header("Префаб пикселя.")]
    [SerializeField] GameObject pixelPrefab;

    [Range(1f, 2f), SerializeField] float pixelsOffset;

    private List<GameObject> lines3D;
    private List<GameObject> pixels3D;

    private void Start()
    {
        pixels3D = new List<GameObject>();
        lines3D = new List<GameObject>();
    }

    private void OnEnable()
    {
        network.onReady += Vizualize;
    }

    private void OnDisable()
    {
        network.onReady -= Vizualize;
    }

    private void Vizualize()
    {
        RedrawNetwork(network.h, network.W);
    }

    public void RedrawNetwork(List<Matrix> activations, List<Matrix> weights)
    {
        DrawPixels(activations);
        //DrawConnections(weights);
    }

    /// <summary>
    /// Отрисовка нейронов.
    /// </summary>
    private void DrawPixels(List<Matrix> activations)
    {
        Quaternion prefabRotation = pixelPrefab.transform.rotation;

        // Входной слой — рисуем изображение как есть
        Vector2Int imgSize = datasetValidator.imageSize;
        for (int y = 0; y < imgSize.y; y++)
        {
            for (int x = 0; x < imgSize.x; x++)
            {
                Vector3 pos = new Vector3(x * pixelsOffset, y * pixelsOffset, 0);
                pos -= new Vector3(imgSize.x / 2f, imgSize.y / 2f, 0) * pixelsOffset;
                GameObject pixel = Instantiate(pixelPrefab, pos, prefabRotation, pixelsContainer);


                float value = (float)activations[0][0, x+y];
                Color color = gradientColorPicker.GetColor(value);
                pixel.GetComponent<Renderer>().material.color = color;

                pixels3D.Add(pixel);
            }
        }

        // Скрытые и выходной слои — рисуем квадратиками в глубину
        for (int layerInx = 1; layerInx < activations.Count-1; layerInx++)
        {
            int neurons = activations[layerInx].GetLength(1);
            int side = Mathf.CeilToInt(Mathf.Sqrt(neurons));

            for (int i = 0; i < neurons; i++)
            {
                int x = i % side;
                int y = i / side;
                Vector3 pos = new Vector3(x * pixelsOffset, y * pixelsOffset, layerInx * Mathf.Max(imgSize.x, imgSize.y));
                pos -= new Vector3(side/2f, side / 2f, 0) * pixelsOffset;
                GameObject pixel = Instantiate(pixelPrefab, pos, prefabRotation, pixelsContainer);

                float value = (float)activations[layerInx][0, i];
                Color color = gradientColorPicker.GetColor(value);
                pixel.GetComponent<Renderer>().material.color = color;

                pixels3D.Add(pixel);
            }
        }

        // Последний слой рисуем ввиде линии:
        int OUT_DIM = activations[activations.Count - 1].GetLength(1);
        for (int x = 0; x < OUT_DIM; x++)
        {
            Vector3 pos = new Vector3(x * pixelsOffset, 0, (activations.Count - 1) * Mathf.Max(imgSize.x, imgSize.y));
            pos -= new Vector3(OUT_DIM/2f, 0, 0) * pixelsOffset;
            GameObject pixel = Instantiate(pixelPrefab, pos, prefabRotation, pixelsContainer);
            pixels3D.Add(pixel);
        }
    }

    /// <summary>
    /// Отрисовка связей между слоями.
    /// </summary>
    private void DrawConnections(List<Matrix> weights)
    {
        // Первый слой — связи от входа к первому скрытому
        int startIndex = 0; // откуда брать пиксели
        Vector2Int imgSize = datasetValidator.imageSize;
        int inputCount = imgSize.x * imgSize.y;

        for (int layer = 0; layer < weights.Count; layer++)
        {
            int prevCount = weights[layer].GetLength(0); // neurons in previous layer
            int currCount = weights[layer].GetLength(1); // neurons in current layer

            for (int i = 0; i < prevCount; i++)
            {
                Vector3 from = pixels3D[startIndex + i].transform.position;

                for (int j = 0; j < currCount; j++)
                {
                    Vector3 to = pixels3D[startIndex + prevCount + j].transform.position;

                    float weight = (float)weights[layer][i, j];
                    Color color = gradientColorPicker.GetColor(weight);

                    GameObject lineObj = new GameObject($"Weight_{layer}_{i}_{j}");
                    lineObj.transform.SetParent(connectionContainer, false);

                    var lr = lineObj.AddComponent<LineRenderer>();
                    lr.positionCount = 2;
                    lr.SetPositions(new[] { from, to });

                    lr.startWidth = 0.02f;
                    lr.endWidth = 0.02f;
                    lr.material = new Material(Shader.Find("Sprites/Default"));
                    lr.startColor = color;
                    lr.endColor = color;

                    lines3D.Add(lineObj);
                }
            }

            startIndex += prevCount;
        }
    }
}

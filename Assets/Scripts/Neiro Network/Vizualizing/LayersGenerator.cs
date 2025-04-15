using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;


[AddComponentMenu("Custom/LayersGenerator (Строитель слоёв)")]
public class LayersGenerator : MonoBehaviour
{
    [Tooltip("Получатель цвета:")]
    [Inject] GradientColorPicker gradientColorPicker;
    [Tooltip("Валидатор датасета.")]
    [Inject] DatasetValidator datasetValidator;
    // -------------------------------------------------------
    [Header("Префаб пикселя.")]
    [SerializeField] GameObject pixelPrefab;

    [Header("Transform-контейнер для пикселей")]
    [SerializeField] Transform pixelsContainer;

    [Header("Расстояние между пикселями:")]
    [Range(1f, 2f), SerializeField] float pixelsOffset;

    [Tooltip("Список пикселей.")]
    [NonSerialized] public List<GameObject> pixels3D;

    private void Start()
    {
        pixels3D = new List<GameObject>();
    }

    /// <summary>
    /// Создаёт слои нейросети.
    /// </summary>
    /// <param name="activations"></param>
    public void DrawLayers(List<Matrix> activations)
    {
        Quaternion prefabRotation = pixelPrefab.transform.rotation;

        Vector2Int imgSize = datasetValidator.imageSize;

        // Входной слой
        CreateInputLayer(activations[0], imgSize, prefabRotation);

        // Скрытые слои
        for (int layerIndex = 1; layerIndex < activations.Count - 1; layerIndex++)
        {
            CreateHiddenLayer(activations[layerIndex], layerIndex, imgSize, prefabRotation);
        }

        // Выходной слой
        CreateOutputLayer(activations[^1], activations.Count - 1, imgSize, prefabRotation);
    }

    private void CreateInputLayer(Matrix matrix, Vector2Int imgSize, Quaternion rotation)
    {
        for (int y = 0; y < imgSize.y; y++)
        {
            for (int x = 0; x < imgSize.x; x++)
            {
                Vector3 pos = new Vector3(x * pixelsOffset, y * pixelsOffset, 0);
                pos -= new Vector3(imgSize.x / 2f, imgSize.y / 2f, 0) * pixelsOffset;

                CreatePixel(pos, (float)matrix[0, x + y], rotation);
            }
        }
    }

    private void CreateHiddenLayer(Matrix matrix, int layerIndex, Vector2Int imgSize, Quaternion rotation)
    {
        int neurons = matrix.GetLength(1);
        int side = Mathf.CeilToInt(Mathf.Sqrt(neurons));

        for (int i = 0; i < neurons; i++)
        {
            int x = i % side;
            int y = i / side;

            Vector3 pos = new Vector3(x * pixelsOffset, y * pixelsOffset, layerIndex * Mathf.Max(imgSize.x, imgSize.y));
            pos -= new Vector3(side / 2f, side / 2f, 0) * pixelsOffset;

            CreatePixel(pos, (float)matrix[0, i], rotation);
        }
    }

    private void CreateOutputLayer(Matrix matrix, int layerIndex, Vector2Int imgSize, Quaternion rotation)
    {
        int outDim = matrix.GetLength(1);

        for (int x = 0; x < outDim; x++)
        {
            Vector3 pos = new Vector3(x * pixelsOffset, 0, layerIndex * Mathf.Max(imgSize.x, imgSize.y));
            pos -= new Vector3(outDim / 2f, 0, 0) * pixelsOffset;

            CreatePixel(pos, (float)matrix[0, x], rotation);
        }
    }

    private void CreatePixel(Vector3 position, float value, Quaternion rotation)
    {
        GameObject pixel = Instantiate(pixelPrefab, position, rotation, pixelsContainer);
        pixel.GetComponent<Renderer>().material.color = gradientColorPicker.GetColor(value);
        pixels3D.Add(pixel);
    }


}

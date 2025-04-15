using System.Collections.Generic;
using UnityEngine;
using Zenject;

[AddComponentMenu("Custom/LinesGenerator (Отрисовщик линий-весов нейросети)")]
public class LinesGenerator : MonoBehaviour
{
    [Tooltip("Получатель цвета:")]
    [Inject] GradientColorPicker gradientColorPicker;

    [Tooltip("Валидатор датасета.")]
    [Inject] DatasetValidator datasetValidator;

    [Tooltip("Визуализатор слоёв.")]
    [Inject] LayersGenerator layersGenerator;

    [Header("Transform-контейнер для линий")]
    [SerializeField] Transform linesContainer;

    [Header("Настройки соединений")]
    [Range(0f, 1f)]
    [Tooltip("Плотность соединений между нейронами (0 - ничего, 1 - всё).")]
    [SerializeField] float connectionDensity = 1f;

    private List<GameObject> lines3D;

    private void Start()
    {
        lines3D = new List<GameObject>();
    }

    public void DrawConnections(List<Matrix> weights)
    {
        int startIndex = 0;
        Vector2Int imgSize = datasetValidator.imageSize;
        int inputCount = imgSize.x * imgSize.y;

        System.Random rng = new System.Random(); // Можно заменить на UnityEngine.Random если нужно

        for (int layer = 0; layer < weights.Count; layer++)
        {
            int prevCount = weights[layer].GetLength(0);
            int currCount = weights[layer].GetLength(1);

            for (int i = 0; i < prevCount; i++)
            {
                Vector3 from = layersGenerator.pixels3D[startIndex + i].transform.position;

                for (int j = 0; j < currCount; j++)
                {
                    // Ограничиваем количество соединений по плотности
                    if (connectionDensity < 1f && rng.NextDouble() > connectionDensity)
                        continue;

                    Vector3 to = layersGenerator.pixels3D[startIndex + prevCount + j].transform.position;

                    float weight = (float)weights[layer][i, j];
                    Color color = gradientColorPicker.GetColor(weight);

                    GameObject lineObj = new GameObject($"Weight_{layer}_{i}_{j}");
                    lineObj.transform.SetParent(linesContainer, false);

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

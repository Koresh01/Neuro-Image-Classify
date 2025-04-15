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

    [Header("Материал для отрисовки линий (должен быть без ZTest и с поддержкой цвета)")]
    [SerializeField] Material lineMaterial;

    [Header("Максимальное кол-во линий между слоями:")]
    [Range(0, 80)]
    [SerializeField] int maxLinesBetweenLayers = 100;

    private struct LineData
    {
        public Vector3 from;
        public Vector3 to;
        public Color color;
    }

    private List<LineData> lines;

    private void Start()
    {
        lines = new List<LineData>();
    }

    public void DrawConnections(List<Matrix> weights)
    {
        lines.Clear();

        int startIndex = 0;
        Vector2Int imgSize = datasetValidator.imageSize;

        for (int layer = 0; layer < weights.Count; layer++)
        {
            int prevCount = weights[layer].GetLength(0);
            int currCount = weights[layer].GetLength(1);

            int totalPossible = prevCount * currCount;

            // Выбираем только maxLinesBetweenLayers случайных связей
            int linesToDraw = Mathf.Min(maxLinesBetweenLayers, totalPossible);
            HashSet<(int, int)> selectedPairs = new HashSet<(int, int)>();

            System.Random rand = new System.Random();

            while (selectedPairs.Count < linesToDraw)
            {
                int i = rand.Next(prevCount);
                int j = rand.Next(currCount);
                selectedPairs.Add((i, j)); // HashSet автоматически исключает дубликаты
            }

            foreach (var pair in selectedPairs)
            {
                int i = pair.Item1;
                int j = pair.Item2;

                Vector3 from = layersGenerator.pixels3D[startIndex + i].transform.position;
                Vector3 to = layersGenerator.pixels3D[startIndex + prevCount + j].transform.position;

                float weight = (float)weights[layer][i, j];
                Color color = gradientColorPicker.GetColor(weight);

                lines.Add(new LineData
                {
                    from = from,
                    to = to,
                    color = color
                });
            }

            startIndex += prevCount;
        }
    }



    private void OnRenderObject()
    {
        if (lines == null || lines.Count == 0 || lineMaterial == null)
            return;

        lineMaterial.SetPass(0);
        GL.PushMatrix();
        GL.MultMatrix(transform.localToWorldMatrix);
        GL.Begin(GL.LINES);

        foreach (var line in lines)
        {
            GL.Color(line.color);
            GL.Vertex(line.from);
            GL.Vertex(line.to);
        }

        GL.End();
        GL.PopMatrix();
    }
}

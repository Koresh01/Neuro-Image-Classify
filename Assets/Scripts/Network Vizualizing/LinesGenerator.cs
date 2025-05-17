using System.Collections.Generic;
using UnityEngine;
using Zenject;

[AddComponentMenu("Custom/LinesGenerator (Рисует связи нейросети)")]
public class LinesGenerator : MonoBehaviour
{
    [Inject] Network network;
    [Inject] GradientColorPicker colorPicker;
    [Inject] LayersGenerator layerVisualizer;

    [Header("Материал для линий")]
    [SerializeField] Material lineMaterial;

    [Header("Максимум соединений между слоями")]
    [Range(0, 10000)]
    [SerializeField] int maxConnectionsPerLayer = 50;


    // Линии и их цвет по изменению веса
    private List<LineData> lines = new();
    private Dictionary<(int layer, int from, int to), float> previousWeights = new();

    /// <summary>
    /// Детеминированный генератор — фиксированное поведение
    /// </summary>
    private System.Random rand;

    private struct LineData
    {
        public Vector3 from;
        public Vector3 to;
        public Color color;
    }


    public void DrawLines()
    {
        lines.Clear();
        rand = new System.Random(12345); // стабильно трогаем одни и те же связи

        int indexOffset = 0;

        for (int layer = 0; layer < network.W.Count; layer++)
        {
            var weightMatrix = network.W[layer];
            int prevCount = weightMatrix.Rows;
            int currCount = weightMatrix.Columns;

            int totalConnections = Mathf.Min(maxConnectionsPerLayer, prevCount * currCount);

            for (int c = 0; c < totalConnections; c++)
            {
                int i = rand.Next(prevCount);
                int j = rand.Next(currCount);

                Vector3 from = layerVisualizer.pixelPositions[indexOffset + i];
                Vector3 to = layerVisualizer.pixelPositions[indexOffset + prevCount + j];

                // реакция цвета линии по изменению веса линии
                float weight = (float)weightMatrix[i, j];
                var key = (layer, i, j);

                float previousWeight = previousWeights.ContainsKey(key) ? previousWeights[key] : weight;
                float delta = weight - previousWeight;

                Color color = colorPicker.GetDeltaColor(delta);
                

                lines.Add(new LineData { from = from, to = to, color = color });
                previousWeights[key] = weight;
            }

            indexOffset += prevCount;
        }
    }


    private void OnRenderObject()
    {
        if (lines.Count == 0 || lineMaterial == null)
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

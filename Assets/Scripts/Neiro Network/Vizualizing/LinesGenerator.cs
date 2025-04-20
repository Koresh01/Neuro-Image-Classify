using System.Collections.Generic;
using UnityEngine;
using Zenject;

[AddComponentMenu("Custom/LinesGenerator (Рисует связи нейросети)")]
public class LinesGenerator : MonoBehaviour
{
    [Inject] GradientColorPicker colorPicker;
    [Inject] LayersGenerator layerVisualizer;

    [Header("Материал для линий")]
    [SerializeField] Material lineMaterial;

    [Header("Максимум соединений между слоями")]
    [Range(0, 10000)]
    [SerializeField] int maxConnectionsPerLayer = 50;

    private List<LineData> lines = new();

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

    public void DrawLines(List<Matrix> weights)
    {
        lines.Clear();
        rand = new System.Random(12345);    // чтоб трогать одни и те же линии

        int indexOffset = 0;

        for (int layer = 0; layer < weights.Count; layer++)
        {
            var weightMatrix = weights[layer];
            int prevCount = weightMatrix.GetLength(0);
            int currCount = weightMatrix.GetLength(1);

            int totalConnections = Mathf.Min(maxConnectionsPerLayer, prevCount * currCount);

            for (int c = 0; c < totalConnections; c++)
            {
                int i = rand.Next(prevCount);
                int j = rand.Next(currCount);

                Vector3 from = layerVisualizer.pixelPositions[indexOffset + i];
                Vector3 to = layerVisualizer.pixelPositions[indexOffset + prevCount + j];

                float weight = (float)weightMatrix[i, j];
                Color color = colorPicker.GetNonActivatedColor(weight);

                lines.Add(new LineData { from = from, to = to, color = color });
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

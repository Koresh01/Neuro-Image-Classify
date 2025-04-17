using UnityEngine;
using System.Collections.Generic;
using Zenject;
using System.Diagnostics;

[AddComponentMenu("Custom/Network Vizualizer (Отрисовка состояния сети)")]
public class NetworkVizualizer : MonoBehaviour
{
    [Tooltip("Визуализатор слоёв.")]
    [Inject] LayersGenerator layersGenerator;
    [Tooltip("Визуализатор линий весов.")]
    [Inject] LinesGenerator linesGenerator;
    // -------------------------------------------------------


    [Tooltip("Нейронная сеть со всеми слоями/весами...")]
    [Inject] Network network;


    private void OnEnable()
    {
        network.onReady += Vizualize;
    }

    private void OnDisable()
    {
        network.onReady -= Vizualize;
    }

    [ContextMenu("Переотрисовать нейронку.")]
    private void Vizualize()
    {
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        RedrawNetwork(network.h, network.W);

        stopwatch.Stop();
        UnityEngine.Debug.Log($"Отрисовки линий и пикселей: {stopwatch.Elapsed.TotalSeconds} секунд");
    }

    public void RedrawNetwork(List<Matrix> activations, List<Matrix> weights)
    {
        layersGenerator.DrawLayers(activations);
        linesGenerator.DrawLines(weights);
    }

    
}

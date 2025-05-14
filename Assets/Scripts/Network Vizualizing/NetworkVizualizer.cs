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

    [ContextMenu("Переотрисовать нейронку.")]
    public void Vizualize()
    {
        RedrawNetwork(network.h, network.W);
    }

    public void RedrawNetwork(List<Matrix> activations, List<Matrix> weights)
    {
        layersGenerator.DrawLayers(activations);
        linesGenerator.DrawLines(weights);
    }

    
}

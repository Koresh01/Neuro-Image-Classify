using UnityEngine;
using System.Collections.Generic;
using Zenject;

[AddComponentMenu("Custom/Network Vizualizer (Отрисовка состояния сети)")]
public class NetworkVizualizer : MonoBehaviour
{
    [Tooltip("Визуализатор слоёв.")]
    [Inject] LayersGenerator layersGenerator;
    [Tooltip("Визуализатор линий весов.")]
    [Inject] LinesGenerator connectionsGenerator;
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

    private void Vizualize()
    {
        RedrawNetwork(network.h, network.W);
    }

    public void RedrawNetwork(List<Matrix> activations, List<Matrix> weights)
    {
        layersGenerator.DrawLayers(activations);
        connectionsGenerator.DrawConnections(weights);
    }

    
}

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
        RedrawNetwork();
    }

    void RedrawNetwork()
    {
        layersGenerator.DrawLayers();
        linesGenerator.DrawLines();
    }

    
}

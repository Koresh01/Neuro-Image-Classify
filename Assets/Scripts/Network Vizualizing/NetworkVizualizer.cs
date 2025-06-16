using UnityEngine;
using System.Collections.Generic;
using Zenject;
using System.Diagnostics;

[AddComponentMenu("Custom/Network Vizualizer (Отрисовка состояния сети)")]
public class NetworkVizualizer : MonoBehaviour
{
    [Tooltip("Визуализатор активированных нейронов.")]
    [Inject] ActivatedLayersGenerator activatedLayersGenerator;

    [Tooltip("Визуализатор линий весов.")]
    [Inject] LinesGenerator linesGenerator;

    [ContextMenu("Переотрисовать нейронку.")]
    public void Vizualize()
    {
        RedrawNetwork();
    }

    void RedrawNetwork()
    {
        // Рисуем слой активированных нейронов
        activatedLayersGenerator.DrawLayers();

        // Рисуем слой НЕ активированных нейронов
        // nonActivatedLayersGenerator.DrawLayers();

        // Рисуем линии
        linesGenerator.DrawLines();
    }

    
}

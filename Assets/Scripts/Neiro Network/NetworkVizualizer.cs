using UnityEngine;
using Zenject;

[AddComponentMenu("Custom/Network Vizualizer (Отрисовка состояния сети)")]
public class NetworkVizualizer : MonoBehaviour
{
    [Inject] Network network;
    void OnEnable()
    {
        network.onReady += FirstDraw;
    }

    void OnDisable()
    {
        network.onReady -= FirstDraw;
    }

    /// <summary>
    /// Отрисовывает нейронку самый первый раз.
    /// </summary>
    void FirstDraw()
    {

    }
}

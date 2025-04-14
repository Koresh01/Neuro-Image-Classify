using UnityEngine;
using Zenject;

[AddComponentMenu("Custom/Network Vizualizer (��������� ��������� ����)")]
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
    /// ������������ �������� ����� ������ ���.
    /// </summary>
    void FirstDraw()
    {

    }
}

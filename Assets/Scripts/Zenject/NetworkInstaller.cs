using UnityEngine;
using Zenject;

public class NetworkInstaller : MonoInstaller
{
    [Tooltip("Нейронная сеть.")]
    [SerializeField] private Network network;

    [Tooltip("Визуализатор нейронки.")]
    [SerializeField] private NetworkVizualizer networkVizualizer;


    public override void InstallBindings()
    {
        Container.Bind<Network>().FromInstance(network).AsSingle();
        Container.Bind<NetworkVizualizer>().FromInstance(networkVizualizer).AsSingle();

        // Регистрируем ImageProcessor как синглтон
        Container.Bind<ImageProcessor>().AsSingle();
    }
}
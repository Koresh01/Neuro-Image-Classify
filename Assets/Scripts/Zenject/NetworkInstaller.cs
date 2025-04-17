using UnityEngine;
using Zenject;

public class NetworkInstaller : MonoInstaller
{
    [Tooltip("��������� ����.")]
    [SerializeField] private Network network;

    [Tooltip("������������ ��������.")]
    [SerializeField] private NetworkVizualizer networkVizualizer;


    public override void InstallBindings()
    {
        Container.Bind<Network>().FromInstance(network).AsSingle();
        Container.Bind<NetworkVizualizer>().FromInstance(networkVizualizer).AsSingle();

        // ������������ ImageProcessor ��� ��������
        Container.Bind<ImageProcessor>().AsSingle();
    }
}
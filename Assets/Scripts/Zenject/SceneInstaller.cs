using UnityEngine;
using Zenject;

[AddComponentMenu("Custom/SceneInstaller (Бинды сцены)")]
public class SceneInstaller : MonoInstaller
{
    [Tooltip("Валидатор датасета.")]
    [SerializeField] private DatasetValidator datasetValidator;

    [Tooltip("Нейронная сеть.")]
    [SerializeField] private Network network;
    public override void InstallBindings()
    {
        Container.Bind<DatasetValidator>().FromInstance(datasetValidator).AsSingle();
        Container.Bind<Network>().FromInstance(network).AsSingle();
    }
}
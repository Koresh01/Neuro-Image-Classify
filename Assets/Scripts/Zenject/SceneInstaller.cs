using UnityEngine;
using Zenject;

[AddComponentMenu("Custom/SceneInstaller (Бинды сцены)")]
public class SceneInstaller : MonoInstaller
{
    [Tooltip("Валидатор датасета.")]
    [SerializeField] private DatasetValidator datasetValidator;

    [Tooltip("Нейронная сеть.")]
    [SerializeField] private Network network;

    [Tooltip("Визуализатор нейронки.")]
    [SerializeField] private NetworkVizualizer networkVizualizer;

    [Tooltip("Контроллер получения цветов.")]
    [SerializeField] private GradientColorPicker gradientColorPicker;

    [Tooltip("Визуализатор слоёв.")]
    [SerializeField] private LayersGenerator layersGenerator;

    [Tooltip("Визуализатор линий весов.")]
    [SerializeField] private LinesGenerator connectionsGenerator;
    public override void InstallBindings()
    {
        Container.Bind<DatasetValidator>().FromInstance(datasetValidator).AsSingle();
        
        Container.Bind<Network>().FromInstance(network).AsSingle();
        Container.Bind<NetworkVizualizer>().FromInstance(networkVizualizer).AsSingle();

        Container.Bind<GradientColorPicker>().FromInstance(gradientColorPicker).AsSingle();

        Container.Bind<LayersGenerator>().FromInstance(layersGenerator).AsSingle();
        Container.Bind<LinesGenerator>().FromInstance(connectionsGenerator).AsSingle();
    }
}
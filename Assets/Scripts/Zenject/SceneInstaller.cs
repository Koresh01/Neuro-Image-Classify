using UnityEngine;
using Zenject;

[AddComponentMenu("Custom/SceneInstaller (Бинды сцены)")]
public class SceneInstaller : MonoInstaller
{
    [Tooltip("Валидатор датасета.")]
    [SerializeField] private DatasetValidator datasetValidator;

    [Tooltip("Контроллер получения цветов.")]
    [SerializeField] private GradientColorPicker gradientColorPicker;

    [Tooltip("Визуализатор слоёв.")]
    [SerializeField] private LayersGenerator layersGenerator;

    [Tooltip("Визуализатор линий весов.")]
    [SerializeField] private LinesGenerator connectionsGenerator;
    public override void InstallBindings()
    {
        Container.Bind<DatasetValidator>().FromInstance(datasetValidator).AsSingle();

        Container.Bind<GradientColorPicker>().FromInstance(gradientColorPicker).AsSingle();

        Container.Bind<LayersGenerator>().FromInstance(layersGenerator).AsSingle();
        Container.Bind<LinesGenerator>().FromInstance(connectionsGenerator).AsSingle();
    }
}
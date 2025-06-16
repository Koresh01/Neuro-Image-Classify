using UnityEngine;
using Zenject;

[AddComponentMenu("Custom/SceneInstaller (Бинды сцены)")]
public class SceneInstaller : MonoInstaller
{
    [Header("Валидатор датасета.")]
    [SerializeField] private DatasetValidator datasetValidator;

    [Header("Контроллер получения цветов.")]
    [SerializeField] private GradientColorPicker gradientColorPicker;

    [Header("Визуализатор активированных слоёв нейросети.")]
    [SerializeField] private ActivatedLayersGenerator activatedLayersGenerator;

    [Header("Визуализатор названий категорий (около выходного слоя нейросети):")]
    [SerializeField] private CategoryLabelsDrawer categoryLabelsDrawer;


    [Header("Визуализатор линий весов.")]
    [SerializeField] private LinesGenerator connectionsGenerator;
    public override void InstallBindings()
    {
        Container.Bind<DatasetValidator>().FromInstance(datasetValidator).AsSingle();

        Container.Bind<GradientColorPicker>().FromInstance(gradientColorPicker).AsSingle();

        Container.Bind<ActivatedLayersGenerator>().FromInstance(activatedLayersGenerator).AsSingle();
        Container.Bind<CategoryLabelsDrawer>().FromInstance(categoryLabelsDrawer).AsSingle();

        Container.Bind<LinesGenerator>().FromInstance(connectionsGenerator).AsSingle();

        // Singleton:
        Container.Bind<CategoryManager>().AsSingle();
    }
}
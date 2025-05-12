using UnityEngine;
using Zenject;

[AddComponentMenu("Custom/UICanvasInstaller (Бинды UI)")]
public class UICanvasInstaller : MonoInstaller
{
    [Tooltip("Контроллер всплывающих панелей.")]
    [SerializeField] PopUpPanelsController popUpPanelsController;

    [Tooltip("Настройщик промежуточных слоёв нейросети.")]
    [SerializeField] MiddleLayersScrollView scrollViewOfMiddleLayers;
    public override void InstallBindings()
    {
        Container.Bind<PopUpPanelsController>().FromInstance(popUpPanelsController).AsSingle();
        Container.Bind<MiddleLayersScrollView>().FromInstance(scrollViewOfMiddleLayers).AsSingle();
    }
}
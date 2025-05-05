using UnityEngine;
using Zenject;

[AddComponentMenu("Custom/UICanvasInstaller (Бинды UI)")]
public class UICanvasInstaller : MonoInstaller
{
    [Tooltip("Контроллер всплывающих панелей.")]
    [SerializeField] PopUpPanelsController popUpPanelsController;

    [Tooltip("Панель ввода конфигурации слоёв сети.")]
    [SerializeField] NetworkConfigPanel networkConfigPanel;
    public override void InstallBindings()
    {
        Container.Bind<PopUpPanelsController>().FromInstance(popUpPanelsController).AsSingle();
        Container.Bind<NetworkConfigPanel>().FromInstance(networkConfigPanel).AsSingle();
    }
}
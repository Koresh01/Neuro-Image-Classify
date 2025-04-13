using UnityEngine;
using Zenject;

[AddComponentMenu("Custom/UICanvasInstaller (Бинды UI)")]
public class UICanvasInstaller : MonoInstaller
{
    [Tooltip("Контроллер всплывающих панелей.")]
    [SerializeField] private PopUpPanelsController popUpPanelsController;
    public override void InstallBindings()
    {
        Container.Bind<PopUpPanelsController>().FromInstance(popUpPanelsController).AsSingle();
    }
}
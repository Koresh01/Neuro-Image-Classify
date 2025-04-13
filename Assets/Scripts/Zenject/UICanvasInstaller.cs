using UnityEngine;
using Zenject;

[AddComponentMenu("Custom/UICanvasInstaller (����� UI)")]
public class UICanvasInstaller : MonoInstaller
{
    [Tooltip("���������� ����������� �������.")]
    [SerializeField] private PopUpPanelsController popUpPanelsController;
    public override void InstallBindings()
    {
        Container.Bind<PopUpPanelsController>().FromInstance(popUpPanelsController).AsSingle();
    }
}
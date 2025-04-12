using UnityEngine;
using Zenject;

// русский
public class SceneInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
    }
}

/*
 using UnityEngine;
using Zenject;

public class SceneInstaller : MonoInstaller
{
    [Tooltip("Спавнер шариков.")]
    [SerializeField] private SpheresSpawner spheresSpawner;

    [Tooltip("Камера.")]
    [SerializeField] private Camera cameraMain;

    [Tooltip("Контроллер прогресса.")]
    [SerializeField] private ProgressListener progressListener;

    [Tooltip("Окошко апгрейда уровня.")]
    [SerializeField] private PanelEnabler panelEnabler;

    [Tooltip("Контроллер накопленных ресурсов.")]
    [SerializeField] private ResourcesListener resourcesListener;

    [Tooltip("Менеджер звуков.")]
    [SerializeField] private SoundManager soundManager;

    public override void InstallBindings()
    {
        // Указывает, что SpheresSpawner будет одним и тем же объектом во всей игре.
        Container.Bind<SpheresSpawner>().FromInstance(spheresSpawner).AsSingle();

        Container.Bind<Camera>().FromInstance(cameraMain).AsSingle();
        Container.Bind<ProgressListener>().FromInstance(progressListener).AsSingle();
        Container.Bind<PanelEnabler>().FromInstance(panelEnabler).AsSingle();
        Container.Bind<ResourcesListener>().FromInstance(resourcesListener).AsSingle();
        Container.Bind<SoundManager>().FromInstance(soundManager).AsSingle();
    }
}


/*
 И если где то в коде мне понадобится скрипт SpheresSpawner, то достаточно будет прописать:
using UnityEngine;
using Zenject;

public class Enemy : MonoBehaviour
{
    [Inject] private SpheresSpawner spheresSpawner;

    private void Start()
    {
        Debug.Log("spheresSpawner получен: " + spheresSpawner.name);
    }
}
 */

using VContainer;
using VContainer.Unity;
using UnityEngine;
using Cinemachine;

public class GameLifetimeScope : LifetimeScope
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private LoaderSettings loaderSettings;
    [SerializeField] private ItemConfig itemConfig;
    [SerializeField] private CelestialConfig planetConfig;
    [SerializeField] private ShipConfig shipConfig;
    [SerializeField] private EmoteConfig emoteConfig;
    [SerializeField] private ShopItem itemPrefab;
    [SerializeField] private UIManager uiManager;
    [SerializeField] private Texture2D cursor;


    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterComponent(virtualCamera);
        builder.RegisterComponent(uiManager);
        builder.RegisterInstance(loaderSettings);
        builder.RegisterInstance(planetConfig);
        builder.RegisterInstance(shipConfig);
        builder.RegisterInstance(emoteConfig);
        builder.RegisterInstance(itemConfig);
        builder.RegisterInstance(itemPrefab);
        builder.RegisterInstance(mainCamera);
        builder.RegisterEntryPoint<CameraManager>(Lifetime.Singleton).AsSelf();
        builder.RegisterEntryPoint<SceneLoader>().WithParameter("cursor", cursor).AsSelf();
    }
}

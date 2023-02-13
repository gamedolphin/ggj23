using UnityEngine;
using VContainer;
using VContainer.Unity;
using Coherence.Toolkit;

public class SceneLifetimeScope : LifetimeScope
{
    protected override void Configure(IContainerBuilder builder)
    {
        builder.Register(container =>
        {
            var settings = container.Resolve<SceneSettings>();
            var saveData = new SaveData(settings.overrideKey);
            return saveData;
        }, Lifetime.Scoped).AsSelf();

        builder.Register(container =>
        {
            var data = container.Resolve<SaveData>();
            var rng = new System.Random(data.Seed);
            return rng;
        }, Lifetime.Scoped).AsSelf();

        builder.RegisterComponentInHierarchy<CoherenceMonoBridge>().AsSelf();
        builder.Register<Connect>(Lifetime.Scoped).AsSelf();
        builder.RegisterEntryPoint<Manager>(Lifetime.Scoped).AsSelf();
        builder.RegisterComponentInHierarchy<PlanetSystem>().AsSelf();

        builder.RegisterBuildCallback(container =>
        {
            var instantiator = container.Resolve<Instantiator>();
            SceneContainer.Map[gameObject.scene.handle] = instantiator;
        });
    }
}

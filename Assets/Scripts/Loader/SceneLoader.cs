using VContainer.Unity;
using VContainer;
using UnityEngine.SceneManagement;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

public class SceneLoader : IAsyncStartable
{
    private LifetimeScope parent;
    private LoaderSettings settings;
    private Texture2D cursor;
    private UIManager ui;

    public SceneLoader(LifetimeScope parent, LoaderSettings settings,
                       Texture2D cursor, UIManager ui)
    {
        this.parent = parent;
        this.settings = settings;
        this.cursor = cursor;
        this.ui = ui;
        this.ui.SetLoading(true);
    }

    async UniTask LoadSceneAsync()
    {
        var count = 0;

        foreach (var setting in settings.settings)
        {
            Debug.Log($"Loading scene {count}");

            var layer = $"Scene{count}";

            using (LifetimeScope.EnqueueParent(parent))
            using (LifetimeScope.Enqueue(builder =>
            {
                builder.RegisterInstance(setting);
                builder.Register<Instantiator>(Lifetime.Scoped)
                    .WithParameter("layer", layer);
            }))
            {
                await SceneManager.LoadSceneAsync(settings.Name, LoadSceneMode.Additive);
                SetLayer(layer);
            }

            count++;
        }
    }

    private void SetLayer(string layer)
    {
        int layerID = LayerMask.NameToLayer(layer);
        var scene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);
        foreach (var tr in scene.GetRootGameObjects())
        {
            SetLayerOnGameObject(tr, layerID);
        }
    }

    private void SetLayerOnGameObject(GameObject obj, int layer)
    {
        obj.layer = layer;
        foreach (Transform child in obj.transform)
        {
            SetLayerOnGameObject(child.gameObject, layer);
        }
    }

    public async UniTask StartAsync(CancellationToken cancellation)
    {
        Cursor.SetCursor(cursor, Vector3.zero, CursorMode.Auto);

        Debug.Log("Loading scenes...");

        await LoadSceneAsync();

        Debug.Log("Loaded all scenes.");

        this.ui.SetLoading(false);
    }

    private int current = 0;
}

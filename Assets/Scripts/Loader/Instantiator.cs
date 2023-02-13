using VContainer;
using VContainer.Unity;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class Instantiator
{
    private IObjectResolver container;
    public int layer;

    public Instantiator(IObjectResolver container, string layer)
    {
        this.container = container;
        this.layer = LayerMask.NameToLayer(layer);
    }

    public T Instantiate<T>(T prefab) where T : Object
    {
        var obj = container.Instantiate(prefab);
        if (obj is Component comp)
        {
            container.InjectGameObject(comp.gameObject);

            SetLayer(comp.gameObject, layer);
        }
        else if (obj is GameObject gobj)
        {
            SetLayer(gobj, layer);
        }

        return obj;
    }

    public Transform Instantiate(Transform prefab)
    {
        var obj = GameObject.Instantiate(prefab);
        SetLayer(obj.gameObject, layer);

        return obj;
    }

    private void SetLayer(GameObject obj, int layer)
    {
        obj.layer = layer;
        foreach (Transform child in obj.transform)
        {
            SetLayer(child.gameObject, layer);
        }
    }
}

using System;
using Coherence.Toolkit;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;
using VContainer.Unity;
using System.Collections.Generic;

public static class SceneContainer
{
    public static Dictionary<int, Instantiator> Map = new Dictionary<int, Instantiator>();
}

[Serializable]
public class ShipInstantiator : INetworkObjectInstantiator
{

    public CoherenceSync OnRemoteInstantiate(CoherenceSync prefab, Vector3 position, Quaternion rotation)
    {
        var scene = SceneManager.GetActiveScene();
        var container = SceneContainer.Map[scene.handle];

        var sync = container.Instantiate(prefab);
        return sync;
    }


    public void OnNetworkEntityDestroyed(CoherenceSync obj, CoherenceSync.DestructionBehavior destructionBehavior)
    {
        GameObject.Destroy(obj.gameObject);
    }
}

using Cinemachine;
using UnityEngine;
using System.Collections.Generic;
using VContainer.Unity;
using System.Linq;

public struct CameraInfo
{
    public Transform player;
    public BoxCollider collider;
}

public class CameraManager : ITickable
{
    private Camera mainCamera;
    private CinemachineVirtualCamera vcam;
    private CinemachineConfiner confiner;

    public int currentLayer;
    private Dictionary<int, CameraInfo> info = new Dictionary<int, CameraInfo>();

    public CameraManager(CinemachineVirtualCamera vcam, Camera mainCamera,
    LoaderSettings settings)
    {
        this.vcam = vcam;
        this.confiner = vcam.GetComponent<CinemachineConfiner>();
        this.mainCamera = mainCamera;

        int count = 0;
        foreach (var setting in settings.settings)
        {
            if (setting.hasCamera)
            {
                this.currentLayer = LayerMask.NameToLayer($"Scene{count}");

                break;
            }

            count++;
        }

        this.SetCurrent(this.currentLayer);
    }

    public void Reset()
    {
        this.confiner.InvalidatePathCache();
    }

    public void Register(Transform player, BoxCollider collider, int layer)
    {
        Debug.Log($"register {layer}");
        info[layer] = new CameraInfo
        {
            player = player,
            collider = collider,
        };
    }

    public void SetCurrent(int layer)
    {
        if (!info.ContainsKey(layer))
        {
            return;
        }

        var current = info[layer];

        this.vcam.Follow = current.player;
        this.vcam.LookAt = current.player;

        this.confiner.m_BoundingVolume = current.collider;

        foreach (var kvp in info)
        {
            if (kvp.Key != layer)
            {
                this.mainCamera.cullingMask &= ~(1 << kvp.Key);
            }
        }

        this.mainCamera.cullingMask |= (1 << layer);
        this.currentLayer = layer;
    }

    void ITickable.Tick()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            var current = this.currentLayer;
            var keys = info.Keys.Where(k => k != this.currentLayer).ToList();
            if (keys.Count < 1)
            {
                UnityEngine.Debug.Log($"Not enough keys not matching: {current}");
                return;
            }

            var key = keys[0];
            UnityEngine.Debug.Log($"switching to {key}");
            SetCurrent(key);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Unity.Mathematics;

public class CameraZoom : MonoBehaviour
{
    private CinemachineVirtualCamera cam;

    [SerializeField] private float sensitivity = 10f;
    [SerializeField] private float min = 2f;
    [SerializeField] private float max = 10f;

    private void Awake()
    {
        cam = GetComponent<CinemachineVirtualCamera>();
    }

    private void LateUpdate()
    {
        var axis = Input.GetAxis("Mouse ScrollWheel");
        if (axis != 0)
        {
            float cameraDistance = axis * sensitivity;
            cam.m_Lens.OrthographicSize -= cameraDistance;
            cam.m_Lens.OrthographicSize = math.clamp(cam.m_Lens.OrthographicSize, min, max);
            cam.GetComponent<CinemachineConfiner>().InvalidatePathCache();
        }
    }
}

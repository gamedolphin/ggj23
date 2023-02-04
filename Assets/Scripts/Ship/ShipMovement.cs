using UnityEngine;
using Unity.Mathematics;

public class ShipMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 10.0f;
    [SerializeField] private float rotateSpeed = 10.0f;
    [SerializeField] private float arriveDistance = 5f;

    private Camera mainCamera;
    private Vector3 currentTarget;
    private Vector3 currentVelocity = Vector3.zero;
    private Vector3 currentAcc = Vector3.zero;

    private const float minDistance = 1.0f;

    [SerializeField] private Transform clickEffect;
    [SerializeField] private Transform crosshair;

    private void Awake()
    {
        mainCamera = Camera.main;
        currentTarget = transform.position;
    }

    private void Start()
    {
        crosshair.SetParent(null);
    }

    private void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = transform.position.z - mainCamera.transform.position.z;
            Vector3 objectPos = mainCamera.ScreenToWorldPoint(mousePos);

            UpdateTarget(objectPos);
            ShowEffect(objectPos);
        }
    }

    private void ShowEffect(Vector3 position)
    {
        Instantiate(clickEffect, position, Quaternion.identity);
    }


    private void FixedUpdate()
    {
        MoveToTarget2();
    }

    private void UpdateTarget(Vector2 position)
    {
        currentTarget = position;

        crosshair.gameObject.SetActive(true);
        crosshair.position = position;
    }

    private void HideCrosshair()
    {
        if (crosshair.gameObject.activeSelf)
        {
            crosshair.gameObject.SetActive(false);
        }
    }

    private void MoveToTarget2()
    {
        var diff = currentTarget - transform.position;

        var distance = diff.magnitude;

        if (distance < minDistance)
        {
            HideCrosshair();
            return;
        }

        var dt = Time.fixedDeltaTime;

        var desired = diff.normalized;

        transform.up = Vector3.RotateTowards(transform.up, desired, rotateSpeed * dt, 0.0f);

        var maxSpeed = moveSpeed;

        if (distance < arriveDistance)
        {
            maxSpeed = math.remap(0, arriveDistance, 0, moveSpeed, distance);
        }

        transform.position += transform.up.normalized * maxSpeed * dt;
    }
}

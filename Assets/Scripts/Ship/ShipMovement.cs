using UnityEngine;
using Unity.Mathematics;
using VContainer;

[RequireComponent(typeof(AudioSource))]
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
    private AudioSource audioSfx;
    private ShipModel shipModel;

    [SerializeField] private Transform model;
    [SerializeField] private Transform clickEffect;
    [SerializeField] private Transform crosshair;

    private Manager manager;

    [Inject]
    public void Construct(Manager manager)
    {
        this.manager = manager;
    }

    private void Awake()
    {
        audioSfx = GetComponent<AudioSource>();
        mainCamera = Camera.main;
        currentTarget = transform.position;
        shipModel = model.GetComponent<ShipModel>();
    }

    private void Start()
    {
        crosshair.SetParent(null);
    }

    private void Update()
    {
        if (Input.GetButtonDown("Fire2") && manager.IsCurrent)
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
        audioSfx.Play();
        var obj = Instantiate(clickEffect);
        obj.position = position;
        obj.rotation = Quaternion.identity;
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

        model.up = Vector3.RotateTowards(model.up, desired, rotateSpeed * dt, 0.0f);

        var maxSpeed = moveSpeed;

        if (distance < arriveDistance)
        {
            maxSpeed = math.remap(0, arriveDistance, 0, moveSpeed, distance);
        }

        transform.position += model.up.normalized * maxSpeed * dt;
    }
}

using UnityEngine;
using TMPro;
using VContainer;

public class Portal : MonoBehaviour
{
    [SerializeField] private Transform nameTag;
    [SerializeField] private Transform image;

    private Manager manager;
    private Instantiator container;

    public float spawnAngle;

    [Inject]
    public void Construct(Manager manager, Instantiator container)
    {
        this.manager = manager;
        this.container = container;
    }

    private void Start()
    {
        image.rotation = Quaternion.Euler(0, 0, spawnAngle);

        var textObj = container.Instantiate(nameTag);
        var tmp = textObj.GetComponentInChildren<TextMeshProUGUI>();
        tmp.text = name;

        textObj.transform.SetParent(transform, false);
    }

    private void OnMouseDown()
    {
        if (manager.player == null)
        {
            return;
        }

        if (Vector3.Distance(manager.player.transform.position, transform.position) > 2f)
        {
            return;
        }

        Debug.Log($"Heading to new system from {name}");

        manager.GoToNewSystem();
    }
}

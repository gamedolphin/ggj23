using UnityEngine;
using TMPro;

public class Portal : MonoBehaviour
{
    [SerializeField] private Transform nameTag;
    [HideInInspector] public GameManager manager;

    public float spawnAngle;

    private void Start()
    {
        foreach (Transform child in transform)
        {
            child.rotation = Quaternion.Euler(0, 0, spawnAngle);
        }

        var textObj = Instantiate(nameTag);
        var tmp = textObj.GetComponentInChildren<TextMeshProUGUI>();
        tmp.text = name;

        textObj.transform.SetParent(transform, false);
    }

    private void OnMouseDown()
    {
        if (manager.Player == null)
        {
            return;
        }

        if (Vector3.Distance(manager.Player.transform.position, transform.position) > 2f)
        {
            return;
        }

        Debug.Log($"Heading to new system from {name}");

        manager.GoToNewSystem();
    }
}

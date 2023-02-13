using UnityEngine;
using VContainer;

[RequireComponent(typeof(Collider2D))]
public class Highlighter : MonoBehaviour
{
    [SerializeField] private GameObject highlight;

    private Manager manager;

    [Inject]
    public void Construct(Manager manager)
    {
        this.manager = manager;
    }

    private void Start()
    {
        highlight.SetActive(false);
    }

    private void OnMouseOver()
    {
        if (manager.player != null &&
                Vector3.Distance(manager.player.transform.position, transform.position) < 2f)
        {
            highlight.SetActive(true);
        }
    }

    private void OnMouseExit()
    {
        highlight.SetActive(false);
    }
}

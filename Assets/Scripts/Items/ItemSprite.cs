using UnityEngine;
using VContainer;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(AudioSource))]
public class ItemSprite : MonoBehaviour
{
    private Manager manager;

    public ItemInfo info;

    public ShopItem item;

    public IItemHolder attachedTo;

    [SerializeField] private TMPro.TextMeshProUGUI nameText;

    private AudioSource audioSfx;

    [Inject]
    public void Construct(Manager manager)
    {
        this.manager = manager;
    }

    private void Start()
    {
        audioSfx = GetComponent<AudioSource>();
        nameText.text = info.name;
        GetComponent<SpriteRenderer>().sprite = info.sprite;
    }

    private void OnMouseDown()
    {
        if (!manager.IsCurrent)
        {
            return;
        }

        if (manager.player != null &&
            Vector3.Distance(manager.player.transform.position, transform.position) < 2)
        {
            item.OnTransfer(manager.player);
        }
    }
}

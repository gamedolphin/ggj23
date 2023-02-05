using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ItemSprite : MonoBehaviour
{
    public ItemInfo info;

    public ShopItem item;

    public IItemHolder attachedTo;

    [SerializeField] private TMPro.TextMeshProUGUI nameText;

    private void Start()
    {
        GetComponent<SpriteRenderer>().sprite = info.sprite;
        nameText.text = info.name;
    }

    private void OnMouseDown()
    {
        if (GameManager.player != null && attachedTo != null)
        {
            if (Vector3.Distance(GameManager.player.transform.position, transform.position) > 2f)
            {
                return;
            }

            try
            {
                Debug.Log($"transferring {info.name} to player");
                attachedTo.GiveItem(item);
                GameManager.player.TakeItem(item);
                attachedTo = GameManager.player;
            }
            catch (System.Exception ex)
            {
                Debug.Log($"Unable to transfer: {ex}");
            }
        }
    }
}

using UnityEngine;
using VContainer;
using Coherence.Toolkit;

[System.Serializable]
public class ItemInfo
{
    public string name;
    public Sprite sprite;
}

public interface IItemHolder
{
    void GiveItem(ShopItem item);
    void TakeItem(ShopItem item);
}

[RequireComponent(typeof(CoherenceSync))]
[RequireComponent(typeof(SpringJoint2D))]
public class ShopItem : MonoBehaviour
{
    private Instantiator container;

    public int ItemIndex;
    public ItemSprite itemPrefab;
    public IItemHolder holder;

    private CoherenceSync _sync;
    public CoherenceSync sync
    {
        get
        {
            if (_sync != null)
            {
                return _sync;
            }

            _sync = GetComponent<CoherenceSync>();
            return _sync;
        }
    }

    private Transform child;
    private ItemConfig items;

    [Inject]
    public void Construct(Instantiator container, ItemConfig items)
    {
        this.container = container;
        this.items = items;
    }

    public void Setup(int index)
    {
        if (child != null)
        {
            DestroyImmediate(child.gameObject);
        }

        var info = this.items.items[index];

        var spr = container.Instantiate(itemPrefab);
        spr.info = info;
        spr.attachedTo = holder;
        spr.item = this;

        GetComponent<SpringJoint2D>().connectedBody = spr.GetComponent<Rigidbody2D>();

        child = spr.transform;

        Debug.Log($"Item held by {holder}");
    }

    private void Start()
    {
        Setup(ItemIndex);
    }

    private void OnDestroy()
    {
        if (child != null)
        {
            DestroyImmediate(child.gameObject);
        }
    }
}

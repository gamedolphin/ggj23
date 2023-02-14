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
    private Manager manager;
    private CoherenceMonoBridge bridge;

    [Inject]
    public void Construct(Instantiator container, ItemConfig items,
                          Manager manager, CoherenceMonoBridge bridge)
    {
        this.container = container;
        this.items = items;
        this.manager = manager;
        this.bridge = bridge;
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
    }

    public void OnTransfer(ShipModel player)
    {
        if (bridge.isConnected)
        {
            sync.SendCommand<ShopItem>(nameof(ShopItem.Transfer),
                                   Coherence.MessageTarget.AuthorityOnly,
                                       player.sync, ((uint)bridge.ClientId));
        }
        else
        {
            transform.SetParent(player.transform);
        }
    }

    public void Transfer(CoherenceSync playerSync,
                         uint clientID)
    {
        if (transform.parent != manager.player.transform)
        {
            transform.SetParent(playerSync.transform);
        }
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

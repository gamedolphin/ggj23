using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

[RequireComponent(typeof(SpringJoint2D))]
public class ShopItem : MonoBehaviour
{
    public PlanetItem info;
    public ItemSprite itemPrefab;

    private Transform child;

    public IItemHolder holder;

    public void Setup(PlanetItem info)
    {
        if (child != null)
        {
            DestroyImmediate(child.gameObject);
        }

        var spr = Instantiate(itemPrefab);
        spr.info = info;
        spr.attachedTo = holder;
        spr.item = this;

        GetComponent<SpringJoint2D>().connectedBody = spr.GetComponent<Rigidbody2D>();

        child = spr.transform;

        this.info = info;

        Debug.Log($"Item held by {holder}");
    }

    private void Start()
    {
        Setup(info);
    }

    private void OnDestroy()
    {
        if (child != null)
        {
            DestroyImmediate(child.gameObject);
        }
    }
}

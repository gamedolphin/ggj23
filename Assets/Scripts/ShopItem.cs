using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemInfo
{
    public string name;
    public Sprite sprite;
}

[RequireComponent(typeof(SpringJoint2D))]
public class ShopItem : MonoBehaviour
{
    public ItemInfo info;
    public ItemSprite itemPrefab;

    private Transform child;

    public void Setup(ItemInfo info)
    {
        if (child != null)
        {
            DestroyImmediate(child.gameObject);
        }

        var spr = Instantiate(itemPrefab);
        spr.info = info;
        spr.attachedTo = transform;

        GetComponent<SpringJoint2D>().connectedBody = spr.GetComponent<Rigidbody2D>();

        child = spr.transform;

        this.info = info;
    }

    private void OnEnable()
    {
        Setup(info);
    }

    private void OnDisable()
    {
        if (child != null)
        {
            DestroyImmediate(child.gameObject);
        }
    }

    private void OnDestroy()
    {
        if (child != null)
        {
            DestroyImmediate(child.gameObject);
        }
    }
}

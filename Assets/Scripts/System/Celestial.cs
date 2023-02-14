using UnityEngine;
using System.Collections.Generic;
using TMPro;
using VContainer;
using VContainer.Unity;

[System.Serializable]
public class PlanetItem : ItemInfo
{
    public int Index;
}

public delegate bool OnTakeItem(int index);

public class Celestial : MonoBehaviour, IItemHolder
{
    private Manager manager;
    private ShopItem itemPrefab;
    private SaveData data;
    private Instantiator container;

    [Inject]
    public void Construct(Manager manager, ShopItem itemPrefab,
                          SaveData data, Instantiator container)
    {
        this.manager = manager;
        this.itemPrefab = itemPrefab;
        this.data = data;
        this.container = container;
    }

    [SerializeField] private Transform nameTag;

    public float minDistance;
    public bool isHome;
    public float spawnAngle;
    public float spawnScale;
    public List<PlanetItem> infos = new List<PlanetItem>();
    public event OnTakeItem OnTakeItem;

    private List<Transform> items = new List<Transform>();

    private void Start()
    {
        foreach (Transform child in transform)
        {
            child.rotation = Quaternion.Euler(0, 0, spawnAngle);
            child.localScale = Vector3.one * spawnScale;
        }

        var textObj = container.Instantiate(nameTag);
        var tmp = textObj.GetComponentInChildren<TextMeshProUGUI>();
        tmp.text = isHome ? name + " (HOME)" : name;

        textObj.transform.SetParent(transform, false);

        foreach (var info in infos)
        {
            var obj = container.Instantiate(itemPrefab);
            obj.ItemIndex = info.Index;
            obj.holder = this;

            obj.transform.position = transform.position;

            items.Add(obj.transform);
        }
    }

    public void GiveItem(ShopItem item)
    {
        RemoveItem(item);
    }

    public void RemoveItem(ShopItem item)
    {
        items.Remove(item.transform);

        if (isHome)
        {
            var index = item.ItemIndex;
            data.ItemIndexes.Remove(index);
            data.Save();
        }
    }

    public void TakeItem(ShopItem item)
    {
        items.Add(item.transform);
        item.transform.SetParent(null);
        item.transform.position = transform.position;
        item.holder = this;

        if (isHome)
        {
            var index = item.ItemIndex;
            data.ItemIndexes.Add(index);
            data.Save();
        }

        if (OnTakeItem != null && OnTakeItem.Invoke(item.ItemIndex))
        {
            RemoveItem(item);
            Destroy(item.gameObject);
        }
    }

    private void OnMouseDown()
    {
        AddToPlanet();
    }

    private void AddToPlanet()
    {
        if (manager.player != null)
        {
            if (Vector3.Distance(manager.player.transform.position, transform.position) > 2f)
            {
                return;
            }

            manager.player.TransferToPlanet(this);
        }
    }
}

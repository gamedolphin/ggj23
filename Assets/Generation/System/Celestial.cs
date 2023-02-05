using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class PlanetItem : ItemInfo
{
    public int Index;
}

public class Celestial : MonoBehaviour, IItemHolder
{
    [HideInInspector] public GameManager manager;

    [HideInInspector] public PlanetNeeds needs;

    public float minDistance;

    [SerializeField] private Transform nameTag;

    [SerializeField] private ShopItem itemPrefab;

    public bool isHome;
    public float spawnAngle;
    public float spawnScale;

    public List<PlanetItem> infos = new List<PlanetItem>();

    private bool showState = false;

    private List<Transform> items = new List<Transform>();

    public SaveData data;

    private void Start()
    {
        foreach (Transform child in transform)
        {
            child.rotation = Quaternion.Euler(0, 0, spawnAngle);
            child.localScale = Vector3.one * spawnScale;
        }

        var textObj = Instantiate(nameTag);
        var tmp = textObj.GetComponentInChildren<TextMeshProUGUI>();
        tmp.text = isHome ? name + " (HOME)" : name;

        textObj.transform.SetParent(transform, false);

        foreach (var info in infos)
        {
            var obj = Instantiate(itemPrefab);
            obj.info = info;
            obj.holder = this;

            obj.transform.SetParent(transform, false);

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
            var index = item.info.Index;
            data.ItemIndexes.Remove(index);
            data.Save();
        }
    }

    public void TakeItem(ShopItem item)
    {
        items.Add(item.transform);
        item.transform.SetParent(transform);
        item.holder = this;

        if (isHome)
        {
            var index = item.info.Index;
            data.ItemIndexes.Add(index);
            data.Save();
        }

        if (needs != null && needs.EatItem(item.info.Index))
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
        if (manager.Player != null)
        {
            if (Vector3.Distance(manager.Player.transform.position, transform.position) > 2f)
            {
                return;
            }

            manager.Player.TransferToPlanet(this);
        }
    }
}

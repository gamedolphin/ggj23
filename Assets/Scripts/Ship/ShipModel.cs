using System.Collections.Generic;
using TMPro;
using UnityEngine;
using NaughtyAttributes;
using VContainer;

public class ShipModel : MonoBehaviour, IItemHolder
{
    [SerializeField] private Sprite[] wing;
    [SerializeField] private Sprite[] top;
    [SerializeField] private Sprite[] engine;
    [SerializeField] private Sprite[] weapons;

    [SerializeField] private SpriteRenderer wingSprite;
    [SerializeField] private SpriteRenderer topSprite;
    [SerializeField] private SpriteRenderer engineSprite;
    [SerializeField] private SpriteRenderer weaponSprite;

    [SerializeField] private TextMeshProUGUI nameText;

    [HideInInspector] public int Seed;

    [HideInInspector] public Manager manager;

    [Inject]
    public void Construct(Manager manager)
    {
        this.manager = manager;
    }

    private List<ShopItem> carrying = new List<ShopItem>();

    [Button("Randomize")]
    private void Randomize()
    {
        this.Seed = Random.Range(0, 1000);
        Setup();
    }

    public void SetName(string name)
    {
        nameText.text = name;
    }

    private void Start()
    {
        Setup();
    }

    public void GiveItem(ShopItem item)
    {
        // add auth transfer things here;
    }

    public void TransferToPlanet(Celestial planet)
    {
        foreach (var item in carrying)
        {
            planet.TakeItem(item);
        }

        carrying.Clear();
    }

    public void TakeItem(ShopItem item)
    {
        if (carrying.Contains(item))
        {
            throw new System.Exception("already have this");
        }

        if (carrying.Count > 3)
        {
            throw new System.Exception("already at max");
        }

        item.transform.SetParent(transform);
        carrying.Add(item);
        item.holder = this;
    }

    [Button("Setup")]
    public void Setup()
    {
        var rng = new System.Random(this.Seed);

        engineSprite.sprite = engine[rng.Next(0, engine.Length)];
        weaponSprite.sprite = weapons[rng.Next(0, weapons.Length)];
        topSprite.sprite = top[rng.Next(0, top.Length)];
        wingSprite.sprite = wing[rng.Next(0, wing.Length)];
    }
}

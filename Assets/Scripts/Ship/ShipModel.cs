using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using NaughtyAttributes;
using VContainer;
using UnityEngine.UI;
using Coherence.Toolkit;

[RequireComponent(typeof(CoherenceSync))]
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
    [SerializeField] private Image emoteSprite;

    [SerializeField] private TextMeshProUGUI nameText;

    [HideInInspector] public int Seed;

    [HideInInspector] public Manager manager;

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

    private Emotes emote;
    private EmoteConfig config;
    private CoherenceMonoBridge bridge;

    [Inject]
    public void Construct(Manager manager, Emotes emote,
                          EmoteConfig config, CoherenceMonoBridge bridge)
    {
        this.manager = manager;
        this.emote = emote;
        this.config = config;
        this.bridge = bridge;
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

        this.emote.onClick += OnEmoteClick;
        emoteSprite.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        this.emote.onClick -= OnEmoteClick;
    }

    private void OnEmoteClick(int index)
    {
        if (bridge.isConnected)
        {
            sync.SendCommand<ShipModel>(nameof(ShipModel.OnEmote),
                                    Coherence.MessageTarget.All,
                                    index);
        }
        else
        {
            OnEmote(index);
        }
    }

    private Coroutine hider;

    public void OnEmote(int index)
    {
        var spr = config.emoteList[index];
        emoteSprite.sprite = spr;
        emoteSprite.gameObject.SetActive(true);
        if (hider != null)
        {
            StopCoroutine(hider);
        }

        hider = StartCoroutine(HideEmote());
    }

    private IEnumerator HideEmote()
    {
        yield return new WaitForSeconds(3);
        emoteSprite.gameObject.SetActive(false);
    }
}

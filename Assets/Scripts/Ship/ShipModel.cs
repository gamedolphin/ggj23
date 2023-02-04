using System;
using TMPro;
using UnityEngine;
using NaughtyAttributes;

public class ShipModel : MonoBehaviour
{
    [SerializeField] public int seed;
    [SerializeField] public string playerName;

    [SerializeField] private Sprite[] wing;
    [SerializeField] private Sprite[] top;
    [SerializeField] private Sprite[] engine;
    [SerializeField] private Sprite[] weapons;

    [SerializeField] private SpriteRenderer wingSprite;
    [SerializeField] private SpriteRenderer topSprite;
    [SerializeField] private SpriteRenderer engineSprite;
    [SerializeField] private SpriteRenderer weaponSprite;

    [SerializeField] private TextMeshProUGUI nameText;

    private Transform textParent;

    [Button("Randomize")]
    private void Randomize()
    {
        seed = UnityEngine.Random.Range(0, 1000);
        Setup();
    }

    public void SetName(string name)
    {
        playerName = name;
        nameText.text = name;
    }

    private void Awake()
    {
        textParent = nameText.transform.parent;
    }

    private void LateUpdate()
    {
        textParent.rotation = Quaternion.Inverse(transform.rotation);
    }

    [Button("Setup")]
    public void Setup()
    {
        var rng = new System.Random(seed);
        engineSprite.sprite = engine[rng.Next(0, engine.Length)];
        weaponSprite.sprite = weapons[rng.Next(0, weapons.Length)];
        topSprite.sprite = top[rng.Next(0, top.Length)];
        wingSprite.sprite = wing[rng.Next(0, wing.Length)];
    }
}

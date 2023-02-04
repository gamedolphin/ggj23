using System;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class ShipModel : MonoBehaviour
{
    [SerializeField] private int seed;

    [SerializeField] private Sprite[] wing;
    [SerializeField] private Sprite[] top;
    [SerializeField] private Sprite[] engine;
    [SerializeField] private Sprite[] weapons;

    [SerializeField] private SpriteRenderer wingSprite;
    [SerializeField] private SpriteRenderer topSprite;
    [SerializeField] private SpriteRenderer engineSprite;
    [SerializeField] private SpriteRenderer weaponSprite;

    [Button("Randomize")]
    private void Randomize()
    {
        seed = UnityEngine.Random.Range(0, 1000);
        Setup();
    }

    [Button("Setup")]
    private void Setup()
    {
        var rng = new System.Random(seed);
        engineSprite.sprite = engine[rng.Next(0, engine.Length)];
        weaponSprite.sprite = weapons[rng.Next(0, weapons.Length)];
        topSprite.sprite = top[rng.Next(0, top.Length)];
        wingSprite.sprite = wing[rng.Next(0, wing.Length)];
    }
}

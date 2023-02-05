using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

[RequireComponent(typeof(AudioSource))]
public class PlanetNeeds : MonoBehaviour
{
    [HideInInspector] public ItemConfig items;
    [HideInInspector] public SaveData data;

    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private Transform smoke;

    private AudioSource audioSfx;

    public void Start()
    {
        audioSfx = GetComponent<AudioSource>();
        LoadLastNeed();
    }

    public bool EatItem(int index)
    {
        if (data.LastNeed == index)
        {
            ShowSFX();

            UpdateCurrentNeed();
            return true;
        }

        return false;
    }

    [Button("SHOW SFX")]
    private void ShowSFX()
    {
        audioSfx.Play();
        Instantiate(smoke);
    }

    private void LoadLastNeed()
    {
        if (data.LastNeed < 0)
        {
            data.LastNeed = Random.Range(0, items.items.Length);
            data.Save();
        }

        UpdateSprite();
    }

    private void UpdateSprite()
    {
        var spr = items.items[data.LastNeed].sprite;
        sprite.sprite = spr;
    }

    private void UpdateCurrentNeed()
    {
        data.LastNeed = Random.Range(0, items.items.Length);
        data.Save();

        UpdateSprite();
    }
}

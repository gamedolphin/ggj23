using UnityEngine;
using NaughtyAttributes;
using VContainer;

[RequireComponent(typeof(AudioSource))]
public class PlanetNeeds : MonoBehaviour
{
    private ItemConfig items;
    private SaveData data;
    private Instantiator container;

    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private Transform smoke;

    private AudioSource audioSfx;

    [Inject]
    public void Construct(ItemConfig items, SaveData data, Instantiator container)
    {
        this.items = items;
        this.data = data;
        this.container = container;
    }

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
        container.Instantiate(smoke);
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

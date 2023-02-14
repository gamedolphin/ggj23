using UnityEngine;
using UnityEngine.UI;
using VContainer;

public delegate void OnEmojiClick(int spr);

public class Emotes : MonoBehaviour
{
    private EmoteConfig config;
    private System.Random rng;
    private Instantiator container;
    private ItemConfig items;

    [Inject]
    public void Construct(EmoteConfig config, System.Random rng,
                          Instantiator container, ItemConfig items)
    {
        this.config = config;
        this.rng = rng;
        this.container = container;
        this.items = items;
    }


    [SerializeField] private Button button;

    public event OnEmojiClick onClick;

    private void Start()
    {
        for (int i = 0; i < 7; i++)
        {
            CreateButton();
        }
    }

    private void CreateButton()
    {
        var index = rng.Next(0, config.emoteList.Length);
        var spr = config.emoteList[index];
        var obj = container.Instantiate(button);
        obj.transform.SetParent(transform, false);

        obj.GetComponent<Image>().sprite = spr;
        obj.onClick.AddListener(() =>
        {
            onClick?.Invoke(index);
        });
    }
}

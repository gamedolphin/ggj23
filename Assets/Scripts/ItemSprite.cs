using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ItemSprite : MonoBehaviour
{
    public ItemInfo info;

    [SerializeField] private TMPro.TextMeshProUGUI nameText;

    private void Start()
    {
        GetComponent<SpriteRenderer>().sprite = info.sprite;
        nameText.text = info.name;
    }
}

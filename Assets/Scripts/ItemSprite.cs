using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ItemSprite : MonoBehaviour
{
    public ItemInfo info;

    public Transform attachedTo;

    [SerializeField] private TMPro.TextMeshProUGUI nameText;

    private void Start()
    {
        GetComponent<SpriteRenderer>().sprite = info.sprite;
        nameText.text = info.name;
    }

    private void OnMouseDown()
    {
        if (GameManager.player != null && attachedTo != null)
        {
            attachedTo.SetParent(GameManager.player);
        }
    }
}

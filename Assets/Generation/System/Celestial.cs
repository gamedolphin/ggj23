using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class Celestial : MonoBehaviour
{
    public float minDistance;

    [SerializeField] private Transform nameTag;

    [SerializeField] private ShopItem itemPrefab;

    public List<ItemInfo> infos = new List<ItemInfo>();

    private bool showState = false;

    private List<Transform> items = new List<Transform>();

    private void Start()
    {
        var textObj = Instantiate(nameTag);
        var tmp = textObj.GetComponentInChildren<TextMeshProUGUI>();
        tmp.text = name;

        textObj.transform.SetParent(transform, false);
        textObj.transform.rotation = Quaternion.Inverse(transform.rotation);

        foreach (var info in infos)
        {
            var obj = Instantiate(itemPrefab);
            obj.info = info;

            obj.transform.SetParent(transform, false);
            obj.gameObject.SetActive(false);

            items.Add(obj.transform);
        }
    }

    private void OnMouseDown()
    {
        showState = !showState;

        foreach (Transform child in items)
        {
            child.gameObject.SetActive(showState);
        }
    }
}

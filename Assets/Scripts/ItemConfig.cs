using UnityEngine;

[CreateAssetMenu(fileName = "ItemConfig", menuName = "ScriptableObjects/SpawnItemConfig", order = 1)]
public class ItemConfig : ScriptableObject
{
    public ItemInfo[] items;
}

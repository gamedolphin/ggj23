using UnityEngine;

[CreateAssetMenu(fileName = "ShipConfig", menuName = "ScriptableObjects/SpawnShipConfig", order = 1)]
public class ShipConfig : ScriptableObject
{
    public ShipModel shipPrefab;
}

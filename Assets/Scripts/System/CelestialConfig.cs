using UnityEngine;

[CreateAssetMenu(fileName = "CelestialConfig", menuName = "ScriptableObjects/SpawnCelestialConfig", order = 1)]
public class CelestialConfig : ScriptableObject
{
    public Background bgPrefab;

    public Portal portalPrefab;

    public Celestial[] starPrefabs;

    public Celestial[] planetPrefabs;

    public PlanetNeeds needsPrefab;
}

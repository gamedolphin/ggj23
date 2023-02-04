using UnityEngine;
using Unity.Mathematics;
using NaughtyAttributes;

[RequireComponent(typeof(BoxCollider))]
public class SolarSystem : MonoBehaviour
{
    [SerializeField] public int seed;

    [Header("Background")]
    [SerializeField] private Background bgPrefab;

    [Header("Portal")]
    [SerializeField] private Portal portalPrefab;
    [SerializeField] private int portalCount;

    [Header("Stars")]
    [SerializeField] private Celestial[] starPrefabs;
    [SerializeField] private int minStars = 1;
    [SerializeField] private int maxStars = 2;
    [SerializeField] private float minStarScale = 1;
    [SerializeField] private float maxStarScale = 3;

    [Header("Planets")]
    [SerializeField] private Celestial[] planetPrefabs;
    [SerializeField] private int minPlanets = 1;
    [SerializeField] private int maxPlanets = 4;
    [SerializeField] private float minPlanetScale = 1;
    [SerializeField] private float maxPlanetScale = 2;

    [Button("Randomize")]
    private void Generate()
    {
        seed = UnityEngine.Random.Range(0, 1000);
        Setup();
    }

    [Button("Setup")]
    public void Setup()
    {
        Reset();

        var rng = new System.Random(seed);

        var sp = SetupBackground();

        var radius = 0.0f;

        var starCount = rng.Next(minStars, maxStars + 1);

        for (int i = 0; i < starCount; i++)
        {
            var prefab = starPrefabs[rng.Next(0, starPrefabs.Length)];
            var star = Instantiate(prefab);
            var pos = RandomOnCircle(radius, rng);

            star.transform.position = pos;

            star.transform.SetParent(transform, true);

            var scale = minStarScale + rng.NextDouble() * maxStarScale;

            star.transform.localScale = Vector3.one * (float)scale;

            var angle = 360 * (float)rng.NextDouble();
            star.transform.rotation = Quaternion.Euler(0, 0, angle);

            radius += star.minDistance * (float)scale;
        }


        var planetCount = rng.Next(minPlanets, maxPlanets + 1);
        for (int i = 0; i < planetCount; i++)
        {
            var prefab = planetPrefabs[rng.Next(0, planetPrefabs.Length)];
            var planet = Instantiate(prefab);
            var pos = RandomOnCircle(radius, rng);

            planet.transform.position = pos;

            planet.transform.SetParent(transform, true);

            var scale = minPlanetScale + rng.NextDouble() * maxPlanetScale;

            planet.transform.localScale = Vector3.one * (float)scale;

            var angle = 360 * (float)rng.NextDouble();
            planet.transform.rotation = Quaternion.Euler(0, 0, angle);

            var pl = planet.GetComponent<IPlanet>();
            if (pl != null)
            {
                pl.Seed = "Seed" + seed;
            }

            var lit = planet.GetComponent<ILit>();
            if (lit != null)
            {
                lit.LightSource = new Vector2(0.5f, 0.5f);
            }

            radius += planet.minDistance + (float)scale; ;
        }

        for (int i = 0; i < portalCount; i++)
        {
            var portal = Instantiate(portalPrefab);
            var pos = RandomOnCircle(radius, rng);

            portal.transform.position = pos;

            portal.transform.SetParent(transform, true);

            var angle = 360 * (float)rng.NextDouble();
            portal.transform.rotation = Quaternion.Euler(0, 0, angle);
        }

        UpdateBackground(sp, radius);
    }

    public Vector2 Rotate(Vector2 v, float degrees)
    {
        float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
        float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);

        float tx = v.x;
        float ty = v.y;
        v.x = (cos * tx) - (sin * ty);
        v.y = (sin * tx) + (cos * ty);
        return v;
    }

    private Background SetupBackground()
    {
        var bg = Instantiate(bgPrefab);
        bg.transform.SetParent(transform);

        return bg;
    }

    private void UpdateBackground(Background sp, float radius)
    {
        sp.Setup(seed, radius);

        var box = GetComponent<BoxCollider>();
        box.size = new Vector3(radius * 4, radius * 4, 10);
        box.center = new Vector3(4, 4, 0);
    }

    private void Reset()
    {
        while (transform.childCount > 0)
        {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }
    }

    private Vector3 RandomOnCircle(float radius, System.Random rng)
    {
        var rnd = (float)rng.NextDouble();
        var angle = 2.0f * Mathf.PI * rnd;

        var mineX = radius * Mathf.Cos(angle);
        var mineY = radius * Mathf.Sin(angle);

        return new Vector3(mineX, mineY, 0);
    }
}

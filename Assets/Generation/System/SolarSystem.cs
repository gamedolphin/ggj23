using UnityEngine;
using Unity.Mathematics;
using NaughtyAttributes;

public class SolarSystem : MonoBehaviour
{
    [SerializeField] private int seed;

    [Header("Background")]
    [SerializeField] private Background bgPrefab;

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
    private void Setup()
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

            radius += planet.minDistance + (float)scale; ;
        }

        UpdateBackground(sp, radius);
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

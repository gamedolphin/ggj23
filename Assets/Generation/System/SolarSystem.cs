using UnityEngine;
using System.Collections.Generic;
using NaughtyAttributes;

[RequireComponent(typeof(BoxCollider))]
public class SolarSystem : MonoBehaviour
{
    [HideInInspector] public GameManager manager;

    [SerializeField] public bool isHome;
    [SerializeField] public int seed;

    [Header("Background")]
    [SerializeField] private Background bgPrefab;

    [Header("Portal")]
    [SerializeField] private Portal portalPrefab;
    [SerializeField] public int portalCount;

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
    [SerializeField] private int minPlanetItems = 1;
    [SerializeField] private int maxPlanetItems = 3;

    [Header("Shop Details")]
    [SerializeField] private ItemConfig items;

    [Header("Planet Needs")]
    [SerializeField] private PlanetNeeds needPrefab;

    [HideInInspector] public SaveData data;

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

            star.name = Names.GetRandomPlanetName(rng);

            var angle = 360 * (float)rng.NextDouble();
            star.manager = manager;
            star.spawnAngle = angle;
            star.spawnScale = (float)scale;

            radius += star.minDistance * (float)scale;
        }


        var planetCount = rng.Next(minPlanets, maxPlanets + 1);
        for (int i = 0; i < planetCount; i++)
        {
            var prefab = planetPrefabs[rng.Next(0, planetPrefabs.Length)];
            var planet = Instantiate(prefab);
            var pos = RandomOnCircle(radius, rng);

            planet.manager = manager;
            planet.transform.position = pos;

            planet.transform.SetParent(transform, true);

            var scale = minPlanetScale + rng.NextDouble() * maxPlanetScale;

            var angle = 360 * (float)rng.NextDouble();
            planet.spawnAngle = angle;
            planet.spawnScale = (float)scale;

            if (isHome && i == 0)
            {
                planet.isHome = true;
            }

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

            planet.data = data;
            planet.name = Names.GetRandomPlanetName(rng);

            radius += planet.minDistance + (float)scale;

            var itemList = new List<PlanetItem>();

            if (isHome && i == 0)
            {
                var need = Instantiate(needPrefab);
                need.items = items;
                need.data = data;

                need.transform.SetParent(planet.transform, false);
                planet.needs = need;

                foreach (var index in data.ItemIndexes)
                {
                    var info = items.items[index];
                    itemList.Add(new PlanetItem
                    {
                        Index = index,
                        name = info.name,
                        sprite = info.sprite,
                    });
                }
            }
            else if (!isHome)
            {
                var itemCount = rng.Next(minPlanetItems, maxPlanetItems + 1);

                for (int j = 0; j < itemCount; j++)
                {
                    var index = rng.Next(0, items.items.Length);
                    var info = items.items[index];
                    itemList.Add(new PlanetItem
                    {
                        Index = index,
                        name = info.name,
                        sprite = info.sprite,
                    });
                }
            }

            planet.infos = itemList;
        }

        for (int i = 0; i < portalCount; i++)
        {
            var portal = Instantiate(portalPrefab);
            var pos = RandomOnCircle(radius, rng);

            portal.manager = manager;
            portal.transform.position = pos;

            portal.transform.SetParent(transform, true);

            var angle = 360 * (float)rng.NextDouble();
            portal.spawnAngle = angle;
            portal.name = Names.GetRandomPlanetName(rng);
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
        bg.transform.SetParent(transform, false);
        bg.transform.position = Vector3.zero;

        return bg;
    }

    private void UpdateBackground(Background sp, float radius)
    {
        sp.Setup(seed, radius);

        var box = GetComponent<BoxCollider>();
        box.size = new Vector3(radius * 4, radius * 4, 40);
        box.center = Vector3.zero;
    }

    [Button("Reset")]
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

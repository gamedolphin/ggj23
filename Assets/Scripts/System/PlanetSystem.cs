using UnityEngine;
using VContainer;
using VContainer.Unity;
using NaughtyAttributes;
using System.Collections.Generic;

[RequireComponent(typeof(BoxCollider))]
public class PlanetSystem : MonoBehaviour
{
    [Header("Portal")]
    [SerializeField] public int portalCount;

    [Header("Stars")]
    [SerializeField] private int minStars = 1;
    [SerializeField] private int maxStars = 2;
    [SerializeField] private float minStarScale = 1;
    [SerializeField] private float maxStarScale = 3;

    [Header("Planets")]
    [SerializeField] private int minPlanets = 1;
    [SerializeField] private int maxPlanets = 4;
    [SerializeField] private float minPlanetScale = 1;
    [SerializeField] private float maxPlanetScale = 2;
    [SerializeField] private int minPlanetItems = 1;
    [SerializeField] private int maxPlanetItems = 3;

    [Header("Planet Needs")]
    [SerializeField] private PlanetNeeds needPrefab;

    private ItemConfig items;
    private SaveData data;
    private Instantiator container;
    private CelestialConfig config;

    private BoxCollider _confiner;
    public BoxCollider confiner
    {
        get
        {
            if (_confiner != null)
            {
                return _confiner;
            }

            _confiner = GetComponent<BoxCollider>();
            return _confiner;
        }
    }

    [Inject]
    public void Construct(ItemConfig items, SaveData data,
                          Instantiator container, CelestialConfig config)
    {
        this.items = items;
        this.data = data;
        this.container = container;
        this.config = config;
    }

    [Button("Randomize")]
    private void Generate()
    {
        var seed = UnityEngine.Random.Range(0, 1000);
        Setup(seed, false);
    }

    [Button("Setup")]
    public void Setup(int seed, bool isHome, bool spawnItems = true)
    {
        var rng = new System.Random(seed);

        Reset();

        var sp = SetupBackground();

        var radius = 0.0f;

        var starCount = rng.Next(minStars, maxStars + 1);

        for (int i = 0; i < starCount; i++)
        {
            var prefab = config.starPrefabs[rng.Next(0, config.starPrefabs.Length)];
            var star = container.Instantiate(prefab);
            var pos = RandomOnCircle(radius, rng);

            star.transform.position = pos;

            star.transform.SetParent(transform, true);

            var scale = minStarScale + rng.NextDouble() * maxStarScale;

            star.name = Names.GetRandomPlanetName(rng);

            var angle = 360 * (float)rng.NextDouble();
            star.spawnAngle = angle;
            star.spawnScale = (float)scale;

            var pl = star.GetComponent<IPlanet>();
            if (pl != null)
            {
                pl.Seed = "Seed" + rng.NextDouble();
            }

            radius += star.minDistance * (float)scale;
        }


        var planetCount = rng.Next(minPlanets, maxPlanets + 1);
        for (int i = 0; i < planetCount; i++)
        {
            var prefab = config.planetPrefabs[rng.Next(0, config.planetPrefabs.Length)];
            var planet = container.Instantiate(prefab);
            var pos = RandomOnCircle(radius, rng);

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
                pl.Seed = "Seed" + rng.NextDouble();
            }

            var lit = planet.GetComponent<ILit>();
            if (lit != null)
            {
                lit.LightSource = new Vector2(0.5f, 0.5f);
            }

            planet.name = Names.GetRandomPlanetName(rng);

            radius += planet.minDistance + (float)scale;

            var itemList = new List<PlanetItem>();

            if (isHome && i == 0)
            {
                var need = container.Instantiate(needPrefab);

                need.transform.SetParent(planet.transform, false);
                planet.OnTakeItem += need.EatItem;

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
            else if (!isHome && spawnItems)
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
            var portal = container.Instantiate(config.portalPrefab);
            var pos = RandomOnCircle(radius, rng);

            portal.transform.position = pos;

            portal.transform.SetParent(transform, true);

            var angle = 360 * (float)rng.NextDouble();
            portal.spawnAngle = angle;
            portal.name = Names.GetRandomPlanetName(rng);
        }

        UpdateBackground(sp, seed, radius);
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
        var bg = container.Instantiate(config.bgPrefab);
        bg.transform.SetParent(transform, false);
        bg.transform.position = Vector3.zero;

        return bg;
    }

    private void UpdateBackground(Background sp, int seed, float radius)
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

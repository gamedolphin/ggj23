using UnityEngine;
using System.Collections.Generic;

public static class Names
{
    public static bool ready = false;

    public static List<string> firstNames = new List<string>();
    public static List<string> lastNames = new List<string>();

    public static List<string> planetNames = new List<string>();

    public static void Initialize()
    {
        if (ready)
        {
            return;
        }

        var asset = Resources.Load("names") as TextAsset;
        string fs = asset.text;
        string[] fLines = fs.Split('\n');
        firstNames = new List<string>(fLines);

        var lastAsset = Resources.Load("lastnames") as TextAsset;
        fs = lastAsset.text;
        fLines = fs.Split('\n');
        lastNames = new List<string>(fLines);

        var planetAsset = Resources.Load("planetnames") as TextAsset;
        fs = planetAsset.text;
        fLines = fs.Split('\n');
        planetNames = new List<string>(fLines);

        ready = true;
    }

    public static string GetRandomPlanetName(System.Random rng)
    {
        Initialize();

        return planetNames[rng.Next(0, planetNames.Count)].Trim();
    }

    public static string GetRandomName(System.Random rng)
    {
        Initialize();

        var firstName = firstNames[rng.Next(0, firstNames.Count)].Trim();
        var lastName = lastNames[rng.Next(0, lastNames.Count)].Trim();

        return $"{firstName} {lastName}";
    }
}

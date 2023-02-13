using UnityEngine;

[System.Serializable]
public class SceneSettings
{
    public string overrideKey;
    public bool hasCamera;
}

[CreateAssetMenu(fileName = "LoaderSettings", menuName = "ScriptableObjects/LoaderSettings", order = 1)]
public class LoaderSettings : ScriptableObject
{
    public string Name;

    public SceneSettings[] settings;
}

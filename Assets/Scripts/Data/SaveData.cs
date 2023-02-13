using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData : System.IDisposable
{
    private const string saveKey = "homewardbound";
    public string Key;
    public int Seed;
    public int LastNeed;
    public List<int> ItemIndexes;

    public void Save()
    {
        var str = JsonUtility.ToJson(this);
        PlayerPrefs.SetString(Key, str);
        PlayerPrefs.Save();
    }

    public SaveData(string key)
    {
        if (string.IsNullOrEmpty(key))
        {
            key = saveKey;
        }

        if (PlayerPrefs.HasKey(key))
        {
            var json = PlayerPrefs.GetString(key);

            var data = JsonUtility.FromJson<SaveData>(json);

            this.Clone(data);
        }
        else
        {
            var seed = Random.Range(0, 10000);
            this.Key = key;
            this.Seed = seed;
            this.LastNeed = -1;
            this.ItemIndexes = new List<int>();

            var str = JsonUtility.ToJson(this);
            PlayerPrefs.SetString(key, str);
            PlayerPrefs.Save();
        }
    }

    public void Clone(SaveData data)
    {
        this.Key = data.Key;
        this.Seed = data.Seed;
        this.LastNeed = data.LastNeed;
        this.ItemIndexes = data.ItemIndexes;
    }

    public void Dispose()
    {
        Save();
    }
}

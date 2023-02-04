using UnityEngine;
using UnityEngine.UI;
using Cinemachine;

[System.Serializable]
public class SaveData
{
    public int Seed;
}

public class GameManager : MonoBehaviour
{
    public static Transform player;

    [SerializeField] private string overrideKey;

    [SerializeField] private TMPro.TextMeshProUGUI nameStr;

    [SerializeField] private SolarSystem system;

    [SerializeField] private Texture2D cursor;

    [SerializeField] private ShipModel ship;

    [SerializeField] private Button startButton;
    [SerializeField] private GameObject welcomeScreen;
    [SerializeField] private Transform followCam;

    private const string saveKey = "homeward";

    private System.Random rng;
    private SaveData data;
    private bool started = false;
    private string playerName = "";

    public SaveData GetData(string key)
    {
        if (PlayerPrefs.HasKey(key))
        {
            var json = PlayerPrefs.GetString(key);
            return JsonUtility.FromJson<SaveData>(json);
        }
        else
        {
            var seed = Random.Range(0, 10000);
            var data = new SaveData
            {
                Seed = seed,
            };

            var str = JsonUtility.ToJson(data);
            PlayerPrefs.SetString(key, str);
            PlayerPrefs.Save();

            return data;
        }
    }

    private void Awake()
    {
        var key = saveKey;
        if (overrideKey != "")
        {
            key = overrideKey;
        }

        data = GetData(key);

        rng = new System.Random(data.Seed);

        playerName = Names.GetRandomName(rng);

        SetName();
    }

    private void Start()
    {
        Cursor.SetCursor(cursor, Vector3.zero, CursorMode.Auto);

        system.seed = data.Seed;
        system.Setup();

        startButton.onClick.AddListener(OnStart);
    }

    private void OnStart()
    {
        if (started)
        {
            return;
        }

        var player = Instantiate(ship);
        player.seed = data.Seed;
        player.Setup();
        player.SetName(playerName);

        var cam = followCam.GetComponent<CinemachineVirtualCamera>();
        cam.Follow = player.transform;
        cam.LookAt = player.transform;
        cam.GetComponent<CinemachineConfiner>().InvalidatePathCache();

        welcomeScreen.SetActive(false);

        GameManager.player = player.transform;
    }

    private void SetName()
    {
        nameStr.text = nameStr.text.Replace("NAME", playerName);
    }
}

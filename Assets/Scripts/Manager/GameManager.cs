// using UnityEngine;
// using UnityEngine.UI;
// using Cinemachine;
// using NaughtyAttributes;

// [RequireComponent(typeof(Network))]
// public class GameManager : MonoBehaviour
// {
//     public static ShipModel player;
//     public ShipModel Player;

//     private Network network;

//     [SerializeField] private string overrideKey;

//     [SerializeField] private TMPro.TextMeshProUGUI nameStr;

//     [SerializeField] public SolarSystem system;

//     [SerializeField] private ShipModel ship;

//     [SerializeField] private Button startButton;
//     [SerializeField] private Button homeButton;
//     [SerializeField] private GameObject welcomeScreen;
//     [SerializeField] private Transform followCam;

//     private const string saveKey = "homewardbound";

//     private System.Random rng;
//     [SerializeField] private SaveData data;
//     private bool started = false;
//     private string playerName = "";

//     public SaveData GetData(string key)
//     {
//         if (PlayerPrefs.HasKey(key))
//         {
//             var json = PlayerPrefs.GetString(key);
//             return JsonUtility.FromJson<SaveData>(json);
//         }
//         else
//         {
//             var seed = Random.Range(0, 10000);
//             var data = new SaveData(key);

//             var str = JsonUtility.ToJson(data);
//             PlayerPrefs.SetString(key, str);
//             PlayerPrefs.Save();

//             return data;
//         }
//     }

//     public void Save(string key)
//     {
//         var str = JsonUtility.ToJson(data);
//         PlayerPrefs.SetString(key, str);
//         PlayerPrefs.Save();
//     }

//     private void Awake()
//     {
//         var key = saveKey;
//         if (overrideKey != "")
//         {
//             key = overrideKey;
//         }

//         data = GetData(key);

//         rng = new System.Random(data.Seed);

//         playerName = Names.GetRandomName(rng);

//         SetName();
//     }

//     private void OnApplicationQuit()
//     {
//         var key = saveKey;
//         if (overrideKey != "")
//         {
//             key = overrideKey;
//         }

//         Save(key);
//     }

//     private void Start()
//     {
//         network = GetComponent<Network>();

//         system.data = data;
//         system.manager = this;

//         GoHome();
//     }

//     [Button("GO TO NEW SYSTEM")]
//     public async void GoToNewSystem()
//     {
//         network.Disconnect();
//         var state = await network.JoinRandomRoom();
//         if (state.seed != 0)
//         {
//             system.seed = state.seed;
//         }

//         system.isHome = false;
//         system.portalCount = 2;
//         system.Setup(state.owner);
//         ResetCam();

//         homeButton.gameObject.SetActive(true);

//         player.transform.position = Vector3.zero;

//         Debug.Log($"Welcome to new system!");
//     }

//     public void GoHome()
//     {
//         network.Disconnect();

//         system.seed = data.Seed;
//         system.isHome = true;
//         system.portalCount = 1;
//         system.Setup();
//         ResetCam();

//         homeButton.gameObject.SetActive(false);

//         if (Player != null)
//         {
//             Player.transform.position = Vector3.zero;
//         }
//     }

//     public void OnStart()
//     {
//         if (started)
//         {
//             return;
//         }

//         var player = Instantiate(ship);
//         player.seed = data.Seed;
//         player.SetName(playerName);

//         var cam = followCam.GetComponent<CinemachineVirtualCamera>();
//         cam.Follow = player.transform;
//         cam.LookAt = player.transform;

//         welcomeScreen.SetActive(false);

//         Player = player;
//         GameManager.player = player;
//     }

//     private void ResetCam()
//     {
//         var cam = followCam.GetComponent<CinemachineVirtualCamera>();
//         cam.GetComponent<CinemachineConfiner>().InvalidatePathCache();
//     }

//     private void SetName()
//     {
//         nameStr.text = nameStr.text.Replace("NAME", playerName);
//     }
// }

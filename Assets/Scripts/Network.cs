using UnityEngine;
using Coherence.Toolkit;
using Coherence.Runtime;
using System.Threading.Tasks;

public class Network : MonoBehaviour
{
    [SerializeField] private CoherenceMonoBridge bridge;
    [SerializeField] private GameObject loadingUI;

    private async Task<string> ConnectBridge()
    {
        try
        {
            // var ready = await PlayResolver.EnsurePlayConnection();
            // if (ready)
            // {
            //     return "";
            // }

            var regions = await PlayResolver.FetchRegions();
            return regions[0];
        }
        catch (System.Exception ex)
        {
            Debug.Log($"Network error : {ex}");
        }

        return "";
    }

    public async Task<int> JoinRandomRoom()
    {
        var region = await ConnectBridge();
        if (region == "")
        {
            Debug.Log("Region fetch failed");
            return 0;
        }

        loadingUI.SetActive(true);

        var rooms = await PlayResolver.FetchRooms(region);
        if (rooms.Count == 0)
        {
            var room = await PlayResolver.CreateRoom(region);
            bridge.JoinRoom(room);
            loadingUI.SetActive(false);
            return 1;
        }
        else
        {
            var randomRoom = rooms[Random.Range(0, rooms.Count)];
            bridge.JoinRoom(randomRoom);
            loadingUI.SetActive(false);

            return 2;
        }
    }
}

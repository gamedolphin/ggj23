using UnityEngine;
using Coherence.Toolkit;
using Coherence.Runtime;
using System.Collections.Generic;
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

    public void Disconnect()
    {
        bridge.Disconnect();
    }

    public async Task<NetworkState> JoinRandomRoom()
    {
        loadingUI.SetActive(true);

        var region = await ConnectBridge();
        if (region == "")
        {
            Debug.Log("Region fetch failed");
            return new NetworkState
            {
                seed = 0,
                owner = true,
            };
        }

        var rooms = await PlayResolver.FetchRooms(region);
        var newRoom = Random.Range(0, 100) > 50;
        if (rooms.Count == 0 || newRoom)
        {
            var seed = Random.Range(0, 1000);
            var options = RoomCreationOptions.Default;
            options.MaxClients = 4;
            options.KeyValues["seed"] = $"{seed}";

            var room = await PlayResolver.CreateRoom(region, options);
            bridge.JoinRoom(room);
            loadingUI.SetActive(false);
            return new NetworkState
            {
                seed = seed,
                owner = true,
            };
        }
        else
        {
            var randomRoom = rooms[Random.Range(0, rooms.Count)];
            bridge.JoinRoom(randomRoom);
            loadingUI.SetActive(false);

            var seedStr = randomRoom.KV["seed"];
            if (seedStr == "")
            {
                return new NetworkState
                {
                    seed = 0,
                    owner = false,
                };
            }

            var seed = 0;
            if (int.TryParse(seedStr, out seed))
            {
                return new NetworkState
                {
                    seed = seed,
                    owner = false,
                };
            }

            bridge.Disconnect();

            return new NetworkState
            {
                seed = 0,
                owner = false,
            };
        }
    }
}

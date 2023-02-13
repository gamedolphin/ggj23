using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Coherence.Runtime;
using Coherence.Toolkit;
using Cysharp.Threading.Tasks;

public struct NetworkState
{
    public int seed;
    public bool owner;
}

public struct RoomWithSeed
{
    public RoomData room;
    public int seed;
}

public class Connect : System.IDisposable
{
    private readonly CoherenceMonoBridge bridge;
    private readonly UIManager ui;

    private ulong lastVisitedRoom;
    private HashSet<ulong> visitedRooms = new HashSet<ulong>();

    public Connect(CoherenceMonoBridge bridge, UIManager ui)
    {
        this.bridge = bridge;
        this.ui = ui;

        bridge.onConnected.AddListener(OnConnect);
        bridge.onDisconnected.AddListener(OnDisconnected);
        bridge.onConnectionError.AddListener(OnConnectionError);

        Debug.Log("Connect ready!");
    }

    public void Dispose()
    {
        bridge.onConnected.RemoveListener(OnConnect);
        bridge.onDisconnected.RemoveListener(OnDisconnected);
        bridge.onConnectionError.RemoveListener(OnConnectionError);
    }

    private async UniTask<int> ConnectToRoom(RoomData room)
    {
        var utcs = new UniTaskCompletionSource<int>();

        var onConn = (UnityEngine.Events.UnityAction<CoherenceMonoBridge>)((_) =>
        {
            Debug.Log("connected");
            utcs.TrySetResult(1);
        });

        var onError = (UnityEngine.Events.UnityAction<CoherenceMonoBridge, Coherence.Connection.ConnectionException>)((CoherenceMonoBridge bridge,
                                Coherence.Connection.ConnectionException reason) =>
        {
            Debug.Log($"Error : {reason}");
            utcs.TrySetException(new System.Exception(reason.ToString()));
        });

        bridge.onConnected.AddListener(onConn);
        bridge.onConnectionError.AddListener(onError);

        bridge.JoinRoom(room);
        try
        {
            return await utcs.Task;
        }
        catch (System.Exception ex)
        {
            throw ex;
        }
        finally
        {
            bridge.onConnected.RemoveListener(onConn);
            bridge.onConnectionError.RemoveListener(onError);
        }
    }

    public void Disconnect()
    {
        bridge.Disconnect();
    }

    private void OnConnect(CoherenceMonoBridge bridge)
    {

    }

    private void OnDisconnected(CoherenceMonoBridge bridge,
                                Coherence.Connection.ConnectionCloseReason reason)
    {

    }

    private void OnConnectionError(CoherenceMonoBridge bridge,
                                Coherence.Connection.ConnectionException reason)
    {

    }

    public async UniTask<NetworkState> JoinRandomRoom()
    {
        Debug.Log("finding rooms");
        ui.SetLoading(true);
        try
        {
            var regions = await PlayResolver.FetchRegions();
            Debug.Log("fetched regions");
            if (regions.Count < 1)
            {
                Debug.Log("no regions found");
                ui.SetLoading(false);
                return new NetworkState
                {
                    seed = 0,
                    owner = true,
                };
            }

            var region = regions[0];
            Debug.Log($"fetching rooms for region {region}");
            var rooms = await PlayResolver.FetchRooms(region);
            Debug.Log($"got rooms for region {region}");
            if (FindRoomToJoin(rooms, out var room, out var state))
            {
                Debug.Log($"joining room {room} and {state}");
                // wait 3 seconds before joining, to let owner create the system.
                await UniTask.Delay(System.TimeSpan.FromSeconds(2));
                await ConnectToRoom(room);

                ui.SetLoading(false);
                Debug.Log("joined a room");
                return state;
            }
            else
            {
                Debug.Log($"creating a new room and joining");
                var roomNew = await CreateRoomToJoin(region);
                Debug.Log($"new room ready, connecting");
                await ConnectToRoom(roomNew.room);
                Debug.Log($"connected to new room");

                ui.SetLoading(false);
                return new NetworkState
                {
                    seed = roomNew.seed,
                    owner = true,
                };
            }
        }
        catch (System.Exception ex)
        {
            UnityEngine.Debug.Log($"Joining exception: {ex}");

            ui.SetLoading(false);
            return new NetworkState
            {
                seed = 0,
                owner = true,
            };
        }
        finally
        {
            ui.SetLoading(false);
        }
    }

    private async UniTask<RoomWithSeed> CreateRoomToJoin(string region)
    {
        var seed = Random.Range(1, 1000);
        var options = RoomCreationOptions.Default;
        options.MaxClients = 4;
        options.KeyValues["seed"] = $"{seed}";

        var room = await PlayResolver.CreateRoom(region, options);
        return new RoomWithSeed
        {
            seed = seed,
            room = room,
        };
    }

    private bool FindRoomToJoin(IReadOnlyList<RoomData> rooms,
                                out RoomData room, out NetworkState state)
    {
        room = new RoomData();
        state = new NetworkState
        {
            seed = 0,
            owner = true
        };

        if (rooms.Count < 1)
        {
            return false;
        }

        room = rooms[0];

        var seedStr = room.KV["seed"];
        if (seedStr == "")
        {
            return false;
        }

        var seed = 0;
        if (int.TryParse(seedStr, out seed))
        {
            state.seed = seed;
            state.owner = false;
        }

        return true;

        var allowedToVisit = rooms
            .Where(r => r.UniqueId != lastVisitedRoom && r.ConnectedPlayers < r.MaxPlayers)
            .Where(r =>
            {
                if (visitedRooms.Contains(r.UniqueId))
                {
                    return Random.Range(0, 100) > 50; // 50% chance to visit an already visited room
                }

                return false;
            });

        if (allowedToVisit.Count() < 1)
        {
            return false;
        }

        room = allowedToVisit.ElementAt(Random.Range(0, allowedToVisit.Count()));

        // var seedStr = room.KV["seed"];
        if (seedStr == "")
        {
            return false;
        }

        //var seed = 0;
        if (int.TryParse(seedStr, out seed))
        {
            state.seed = seed;
            state.owner = false;
        }

        return true;
    }
}

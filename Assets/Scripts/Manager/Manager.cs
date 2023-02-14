using VContainer;
using VContainer.Unity;
using UnityEngine;

public class Manager : IStartable, System.IDisposable
{
    private readonly Connect connect;
    private readonly SceneSettings settings;
    private readonly UIManager ui;
    private readonly SaveData data;
    private readonly PlanetSystem system;
    private readonly ShipConfig shipConfig;
    private readonly CameraManager cam;
    private readonly Instantiator container;

    private readonly System.Random rng;
    private readonly string playerName;

    private bool started = false;

    public ShipModel player;

    public Manager(Connect connect, SceneSettings settings,
                   UIManager ui, SaveData data, PlanetSystem system,
                   ShipConfig shipConfig, System.Random rng,
                   CameraManager cam,
                   Instantiator container)
    {
        this.connect = connect;
        this.settings = settings;
        this.ui = ui;
        this.data = data;
        this.system = system;
        this.shipConfig = shipConfig;
        this.cam = cam;
        this.container = container;

        this.rng = rng;
        this.playerName = Names.GetRandomName(rng);
    }

    public bool IsCurrent => container.IsCurrent;

    void IStartable.Start()
    {
        Debug.Log("ready");
        this.ui.OnStart += this.OnStart;
        this.ui.SetPlayerName(this.playerName);

        this.ui.ToggleMainMenu(true);
    }

    public async void GoToNewSystem()
    {
        ui.ShowHome(true);

        var networkState = await this.connect.JoinRandomRoom();

        Debug.Log($"New system has seed {networkState.seed}");

        var seed = networkState.seed == 0 ? Random.Range(0, 1000) : networkState.seed;

        system.portalCount = 2;
        system.Setup(seed, false, networkState.owner);

        if (player != null)
        {
            player.transform.position = Vector3.zero;
        }

        ResetCam();
    }

    private void OnStart()
    {
        Debug.Log("here");

        if (started)
        {
            return;
        }

        this.player = container.Instantiate(shipConfig.shipPrefab);
        this.player.Seed = this.data.Seed;
        this.player.SetName(this.playerName);

        this.cam.Register(this.player.transform, this.system.confiner,
                          this.container.layer);

        GoHome();

        ResetCam();

        ui.ToggleMainMenu(false);

        if (this.cam.currentLayer == this.container.layer)
        {
            this.cam.SetCurrent(this.container.layer);
        }

        started = true;
    }

    private void ResetCam()
    {
        if (this.cam.currentLayer == this.container.layer)
        {
            this.cam.Reset();
        }
    }

    private void GoHome()
    {
        connect.Disconnect();
        system.portalCount = 1;
        system.Setup(this.data.Seed, true, false);

        if (player != null)
        {
            player.transform.position = Vector3.zero;
        }

        ui.ShowHome(false);
        ResetCam();
    }

    public bool IsActive()
    {
        return this.cam.currentLayer == this.container.layer;
    }

    public void Dispose()
    {
        this.ui.OnStart -= this.OnStart;
    }
}

using Fusion;
using Fusion.Sockets;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkManager : MonoBehaviour, INetworkRunnerCallbacks
{
    // --- 싱글톤 인스턴스 ---
    public static NetworkManager Instance { get; private set; }

    // --- 인스펙터에서 설정 ---
    [Tooltip("스폰할 플레이어의 프리팹")]
    [SerializeField] private NetworkPrefabRef _playerPrefab;

    // --- 내부 변수 ---
    private NetworkRunner _runner;
    private Dictionary<PlayerRef, NetworkObject> _spawnedCharacters = new Dictionary<PlayerRef, NetworkObject>();

    private void Awake()
    {
        // 싱글톤 패턴 설정
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 호스트로 게임을 시작하는 UI 버튼 등에서 호출합니다.
    /// </summary>
    public void StartHost()
    {
        StartGame(GameMode.Host);
    }

    /// <summary>
    /// 클라이언트로 게임에 참여하는 UI 버튼 등에서 호출합니다.
    /// </summary>
    public void StartClient()
    {
        StartGame(GameMode.Client);
    }

    /// <summary>
    /// Fusion 세션을 시작하는 핵심 로직입니다.
    /// </summary>
    private async void StartGame(GameMode mode)
    {
        _runner = gameObject.AddComponent<NetworkRunner>();
        _runner.ProvideInput = true;
        _runner.AddCallbacks(this);

        gameObject.AddComponent<NetworkSceneManagerDefault>();

        await _runner.StartGame(new StartGameArgs()
        {
            GameMode = mode,
            SessionName = "TestVRRoom",
            // SceneRef.FromIndex를 사용하여 씬을 지정합니다.
            Scene = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex),
            SceneManager = gameObject.GetComponent<NetworkSceneManagerDefault>()
        });
    }
    // --- INetworkRunnerCallbacks 구현 ---

    /// <summary>
    /// 새로운 플레이어가 세션에 참여했을 때 서버/호스트에서 호출됩니다.
    /// </summary>
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        // 서버/호스트만 플레이어를 스폰할 권한을 가집니다.
        if (runner.IsServer)
        {
            Debug.Log($"플레이어 {player} 참가, 아바타를 스폰합니다.");

            // 플레이어 프리팹을 스폰하고, 해당 플레이어에게 상태 권한(State Authority)을 부여합니다.
            NetworkObject networkPlayerObject = runner.Spawn(_playerPrefab, Vector3.zero, Quaternion.identity, player);

            // 스폰된 캐릭터를 딕셔너리에 저장하여 추적합니다.
            _spawnedCharacters.Add(player, networkPlayerObject);
        }
    }

    /// <summary>
    /// 플레이어가 세션을 떠났을 때 서버/호스트에서 호출됩니다.
    /// </summary>
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        // 서버/호스트만 플레이어를 디스폰할 수 있습니다.
        if (runner.IsServer)
        {
            // 떠난 플레이어의 캐릭터를 찾아 디스폰합니다.
            if (_spawnedCharacters.TryGetValue(player, out NetworkObject networkObject))
            {
                runner.Despawn(networkObject);
                _spawnedCharacters.Remove(player);
                Debug.Log($"플레이어 {player} 퇴장, 아바타를 디스폰합니다.");
            }
        }
    }

    // --- 아래는 필수지만 이 예제에서는 사용하지 않는 콜백들입니다. ---

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        // 이 예제에서는 AvatarHardwareRig가 입력을 처리하므로 비워둡니다.
        // 만약 이동 입력을 여기서 처리하고 싶다면, 아래와 같이 작성할 수 있습니다.
        var data = new AvatarHardwareRig.NetworkInputData();

        // 예시: 키보드 입력 받기 (VR에서는 XR 입력으로 대체)
        // data.joystickInput.x = Input.GetAxis("Horizontal");
        // data.joystickInput.y = Input.GetAxis("Vertical");

        // input.Set(data);
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
    public void OnConnectedToServer(NetworkRunner runner) { }
    public void OnDisconnectedFromServer(NetworkRunner runner) { }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data) { }
    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }

    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
    }

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
    }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
    {
    }

    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
    {
    }
}
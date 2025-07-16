using UnityEngine;
using Fusion;
using Fusion.Sockets;

using System;
using System.Collections;
using System.Collections.Generic; 

public class NetworkManager : MonoBehaviour, INetworkRunnerCallbacks
{
    // 인스펙터에서 NetworkRunner 프리팹을 연결해줍니다.
    [SerializeField]
    private NetworkRunner _runnerPrefab;


    [SerializeField]
    private NetworkPrefabRef _playerPrefab;

    private NetworkRunner _runner;

    IEnumerator Start()
    {
        // 씬이 로드된 후 아주 잠깐 (0.1초) 기다립니다.
        // 이 짧은 시간이 시스템이 안정화될 시간을 줍니다.
        yield return new WaitForSeconds(0.1f);

        _runner = Instantiate(_runnerPrefab);
        _runner.AddCallbacks(this);

        // 비동기 작업인 StartGame을 호출하고 끝날 때까지 기다리지 않습니다.
        // 코루틴 내에서는 await를 직접 사용할 수 없기 때문입니다.
        _ = _runner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.AutoHostOrClient,
            SessionName = "TestVRRoom"
        });
    }
    // 플레이어가 룸에 접속했을 때 자동으로 호출되는 콜백 함수
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log($"Player {player} has joined.");
        // 접속한 플레이어가 '나' 자신이라면
        if (runner.IsServer)
        {
            Debug.Log($"OnPlayerJoined on Server. Spawning avatar for player {player}.");

            // 새로 접속한 'player'가 누구든 상관없이 그 플레이어를 위해 아바타를 스폰합니다.
            // 'player' 변수에는 방금 접속한 플레이어의 정보가 담겨있습니다.
            runner.Spawn(_playerPrefab, Vector3.up, Quaternion.identity, player);

        }
    }

    
    #region Unused INetworkRunnerCallbacks Methods
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) { }
    
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

    void INetworkRunnerCallbacks.OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
    }

    void INetworkRunnerCallbacks.OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
    }

    void INetworkRunnerCallbacks.OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
    }

    void INetworkRunnerCallbacks.OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
    }

    void INetworkRunnerCallbacks.OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
    {
    }

    void INetworkRunnerCallbacks.OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
    {
    }

    void INetworkRunnerCallbacks.OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
    }

    void INetworkRunnerCallbacks.OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
    }

    

    

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        var myInput = new PlayerNetworkInput();

        // 매 프레임 키보드 입력을 새로 읽어옵니다.
        myInput.Movement.x = Input.GetAxis("Horizontal");
        myInput.Movement.y = Input.GetAxis("Vertical");

        // 읽어온 입력 값을 Fusion에 전달
        input.Set(myInput);
    }

   

    #endregion

}

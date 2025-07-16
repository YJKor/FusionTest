using UnityEngine;
using Fusion;
using Fusion.Sockets;

using System;
using System.Collections;
using System.Collections.Generic; 

public class NetworkManager : MonoBehaviour, INetworkRunnerCallbacks
{
    // �ν����Ϳ��� NetworkRunner �������� �������ݴϴ�.
    [SerializeField]
    private NetworkRunner _runnerPrefab;


    [SerializeField]
    private NetworkPrefabRef _playerPrefab;

    private NetworkRunner _runner;

    IEnumerator Start()
    {
        // ���� �ε�� �� ���� ��� (0.1��) ��ٸ��ϴ�.
        // �� ª�� �ð��� �ý����� ����ȭ�� �ð��� �ݴϴ�.
        yield return new WaitForSeconds(0.1f);

        _runner = Instantiate(_runnerPrefab);
        _runner.AddCallbacks(this);

        // �񵿱� �۾��� StartGame�� ȣ���ϰ� ���� ������ ��ٸ��� �ʽ��ϴ�.
        // �ڷ�ƾ �������� await�� ���� ����� �� ���� �����Դϴ�.
        _ = _runner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.AutoHostOrClient,
            SessionName = "TestVRRoom"
        });
    }
    // �÷��̾ �뿡 �������� �� �ڵ����� ȣ��Ǵ� �ݹ� �Լ�
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log($"Player {player} has joined.");
        // ������ �÷��̾ '��' �ڽ��̶��
        if (runner.IsServer)
        {
            Debug.Log($"OnPlayerJoined on Server. Spawning avatar for player {player}.");

            // ���� ������ 'player'�� ������ ������� �� �÷��̾ ���� �ƹ�Ÿ�� �����մϴ�.
            // 'player' �������� ��� ������ �÷��̾��� ������ ����ֽ��ϴ�.
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

        // �� ������ Ű���� �Է��� ���� �о�ɴϴ�.
        myInput.Movement.x = Input.GetAxis("Horizontal");
        myInput.Movement.y = Input.GetAxis("Vertical");

        // �о�� �Է� ���� Fusion�� ����
        input.Set(myInput);
    }

   

    #endregion

}

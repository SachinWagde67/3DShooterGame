using Fusion;
using Fusion.Sockets;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour, INetworkRunnerCallbacks {

    [SerializeField] private NetworkPrefabRef playerPrefab;

    private NetworkObject playerObject;

    public void SetPlayerPrefab(NetworkPrefabRef player) {
        playerPrefab = player;
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player) {

        if(runner.IsServer) {

            Vector3 spawnPos = new Vector3(UnityEngine.Random.Range(-5, 5), 2, UnityEngine.Random.Range(-5, 5));
            playerObject = runner.Spawn(playerPrefab, spawnPos, Quaternion.identity, player);
            runner.SetPlayerObject(player, playerObject);
        }
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) {

        runner.Despawn(playerObject);
    }

    public void OnInput(NetworkRunner runner, NetworkInput input) {

        NetworkInputData data = new NetworkInputData();

        data.move = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        data.buttons.Set((int)PlayerButtons.Jump, Input.GetKey(KeyCode.Space));
        data.buttons.Set((int)PlayerButtons.Run, Input.GetKey(KeyCode.LeftShift));

        input.Set(data);
    }


    public void OnConnectedToServer(NetworkRunner runner) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }
    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
}

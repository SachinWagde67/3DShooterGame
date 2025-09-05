using Fusion;
using UnityEngine;

public class GameStart : MonoBehaviour {

    [SerializeField] private NetworkRunner runnerPrefab;
    [SerializeField] private NetworkPrefabRef playerSpawnerPrefab;

    private NetworkRunner runner;

    public NetworkRunner CreateRunner() {

        if(runner == null) {

            runner = Instantiate(runnerPrefab);

            PlayerSpawner spawner = runner.gameObject.AddComponent<PlayerSpawner>();
            spawner.SetPlayerPrefab(playerSpawnerPrefab);
            runner.AddCallbacks(spawner);

            if(runner.GetComponent<INetworkSceneManager>() == null) {
                runner.gameObject.AddComponent<NetworkSceneManagerDefault>();
            }
        }

        return runner;
    }
}

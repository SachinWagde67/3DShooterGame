using Fusion;
using UnityEngine;

public class GameStart : MonoBehaviour {

    [SerializeField] private NetworkRunner runnerPrefab;
    [SerializeField] private NetworkPrefabRef playerSpawnerPrefab;

    async void Start() {

        NetworkRunner runner = Instantiate(runnerPrefab);

        PlayerSpawner spawner = runner.gameObject.AddComponent<PlayerSpawner>();
        spawner.SetPlayerPrefab(playerSpawnerPrefab);
        runner.AddCallbacks(spawner);

        await runner.StartGame(new StartGameArgs() {

            GameMode = GameMode.AutoHostOrClient,
            SessionName = "TestSession",
            Scene = SceneRef.FromIndex(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex),
            SceneManager = runner.GetComponent<NetworkSceneManagerDefault>()
        });
    }
}

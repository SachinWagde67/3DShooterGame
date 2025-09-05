using Fusion;
using UnityEngine;

public class GameStart : MonoBehaviour {

    [SerializeField] private NetworkRunner runnerPrefab;

    async void Start() {

        var runner = Instantiate(runnerPrefab);
        await runner.StartGame(new StartGameArgs() {
            GameMode = GameMode.Shared, // or Host if you want server-client
            SessionName = "TestSession",
            Scene = SceneRef.FromIndex(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex),
            SceneManager = runner.GetComponent<NetworkSceneManagerDefault>()
        });
    }
}

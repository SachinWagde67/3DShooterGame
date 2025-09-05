using Fusion;
using TMPro;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviour {

    [Header("UI References")]
    [SerializeField] private TMP_InputField roomIdInput;
    [SerializeField] private TMP_InputField maxPlayersInput;
    [SerializeField] private Button createButton;
    [SerializeField] private Button joinButton;

    [Header("Dependencies")]
    [SerializeField] private GameStart gameStart;

    private NetworkRunner runner;

    private void Awake() {
        createButton.onClick.AddListener(CreateRoom);
        joinButton.onClick.AddListener(JoinRoom);
    }

    private async void CreateRoom() {

        string roomId = "Room" + Random.Range(1, 100).ToString();
        if(string.IsNullOrEmpty(roomId)) {
            Debug.LogWarning("Room ID cannot be empty!");
            return;
        }

        int maxPlayers = 2;
        if(!string.IsNullOrEmpty(maxPlayersInput.text)) {
            int.TryParse(maxPlayersInput.text, out maxPlayers);
        }

        await StartRunner(GameMode.Host, roomId, maxPlayers);
    }

    private async void JoinRoom() {

        string roomId = roomIdInput.text;
        if(string.IsNullOrEmpty(roomId)) {
            Debug.LogWarning("Room ID cannot be empty!");
            return;
        }

        await StartRunner(GameMode.Client, roomId, 2);
    }

    private async Task StartRunner(GameMode mode, string roomId, int maxPlayers) {

        if(runner == null) {
            runner = gameStart.CreateRunner();
        }

        StartGameArgs args = new StartGameArgs() {
            GameMode = mode,
            SessionName = roomId,
            PlayerCount = maxPlayers,
            Scene = SceneRef.FromIndex(1),      //Game Scene (Actual Play Scene)
            SceneManager = runner.GetComponent<INetworkSceneManager>()
        };

        StartGameResult result = await runner.StartGame(args);
        if(result.Ok) {
            Debug.Log($"{mode} started in room {roomId} with max {maxPlayers} players");
        } else {
            Debug.LogError($"Failed to start {mode}: {result.ShutdownReason}");
        }
    }
}

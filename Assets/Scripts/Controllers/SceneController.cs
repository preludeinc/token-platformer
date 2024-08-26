using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    [SerializeField] private UIController ui;
    [SerializeField] PlayerController pc;
    private int score = 0;

    private void Awake()
    {
        Messenger.AddListener(GameEvent.INCREASE_SCORE, OnIncreaseScore);
        Messenger.AddListener(GameEvent.PLAYER_DEAD, OnPlayerDead);
        Messenger.AddListener(GameEvent.START_GAME, OnStartGame);
        Messenger.AddListener(GameEvent.RESTART_GAME, OnRestartGame);
        Messenger.AddListener(GameEvent.WIN_GAME, OnWinGame);
    }

    private void OnDestroy()
    {
        Messenger.RemoveListener(GameEvent.INCREASE_SCORE, OnIncreaseScore);
        Messenger.RemoveListener(GameEvent.PLAYER_DEAD, OnPlayerDead);
        Messenger.RemoveListener(GameEvent.START_GAME, OnStartGame);
        Messenger.RemoveListener(GameEvent.RESTART_GAME, OnRestartGame);
        Messenger.RemoveListener(GameEvent.WIN_GAME, OnWinGame);
    }

    private void Start()
    {
        ui.UpdateScore(score);
        Messenger.Broadcast(GameEvent.START_GAME);
    }

    private void OnIncreaseScore()
    {
        score++;
        ui.UpdateScore(score);
    }

    private void OnPlayerDead()
    {
        ui.ShowGameOverPopup();
    }

    private void OnRestartGame()
    {
        SceneManager.LoadScene(0);
    }

    private void OnStartGame()
    {
        ui.ShowControlsPopup();
    }

    private void OnWinGame()
    {
        StartCoroutine(ui.ShowWinConfetti());
    }
}

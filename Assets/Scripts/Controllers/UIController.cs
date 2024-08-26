using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class UIController : MonoBehaviour
{
    [Header("Menus")]
    [SerializeField] private MenuPopup menuPopup;
    [SerializeField] private MenuPopup controlsPopup;
    [SerializeField] private GameOverPopup gameOverPopup;
    [SerializeField] private WinPopup winPopup;
    private int popupsActive = 0;

    [Header("Visible On-Screen")]
    [SerializeField] private Sprite healthHeart;
    [SerializeField] private Sprite emptyHeart;
    [SerializeField] Image[] heartSprites;
    [SerializeField] private TextMeshProUGUI score;
    [SerializeField] private Transform win;

    [Header("Sound")]
    [SerializeField] private SoundManager soundManager;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip sadTrombone;

    [Header("Win Condition")]
    [SerializeField] private ParticleSystem confetti;
    [SerializeField] private PlayerController pc;

    private void Awake()
    {
        Messenger.AddListener(GameEvent.MENU_OPENED, OnMenuOpened);
        Messenger.AddListener(GameEvent.MENU_CLOSED, OnMenuClosed);
        Messenger<float>.AddListener(GameEvent.HEALTH_CHANGED, OnHealthChanged);
        Messenger<int>.AddListener(GameEvent.SCORE_CHANGED, OnScoreChanged);
    }

    private void OnDestroy()
    {
        Messenger.RemoveListener(GameEvent.MENU_OPENED, OnMenuOpened);
        Messenger.RemoveListener(GameEvent.MENU_CLOSED, OnMenuClosed);
        Messenger<float>.RemoveListener(GameEvent.HEALTH_CHANGED,
                                        OnHealthChanged);
        Messenger<int>.AddListener(GameEvent.SCORE_CHANGED, OnScoreChanged);
    }

    // Start is called before the first frame update
    void Start() 
    { 
        UpdateHealth(1.0f); 
    }

    // Update is called once per frame
    void Update()
    {
        UpdateHealth(pc.Health);

        if (Input.GetKeyDown(KeyCode.Escape) && !menuPopup.IsActive() &&
            popupsActive == 0)
        {
            menuPopup.Open();
        }
    }

    public void UpdateScore(int newScore)
    {
        string strScore = newScore.ToString();
        score.text = "Score: " + strScore;
    }

    public void SetGameActive(bool active)
    {
        if (active)
        {
            Messenger.Broadcast(GameEvent.GAME_ACTIVE);
            Time.timeScale = 1;  // unpauses game
        }
        else
        {
            Messenger.Broadcast(GameEvent.GAME_INACTIVE);
            Time.timeScale = 0;  // pauses game
        }
    }

    private void OnMenuOpened()
    {
        if (popupsActive == 0)
        {
            SetGameActive(false);
        }
        popupsActive++;
    }

    private void OnMenuClosed()
    {
        popupsActive--;
        if (popupsActive == 0)
        {
            SetGameActive(true);
        }
    }

    public void OnHealthChanged(float health_remaining)
    {
        UpdateHealth(health_remaining);
    }

    public void OnScoreChanged(int score) { UpdateScore(score); }

    private void UpdateHealth(float healthRemaining)
    {
        for (int i = 0; i < heartSprites.Length; i++)
        {
            if (i < healthRemaining)
            {
                heartSprites[i].sprite = healthHeart;
            }
            else
            {
                heartSprites[i].sprite = emptyHeart;
            }

            if (i < pc.MaxHealth)
            {
                heartSprites[i].enabled = true;
            }
            else
            {
                heartSprites[i].enabled = false;
            }
        }
    }

    public void ShowGameOverPopup()
    {
        gameOverPopup.Open();
        audioSource.Pause();
        soundManager.PlaySoundClip(sadTrombone, transform, 1f);
    }

    public void ShowControlsPopup() { controlsPopup.Open(); }

    public IEnumerator ShowWinConfetti()
    {
        win.gameObject.SetActive(true);
        confetti.gameObject.SetActive(true);
        Messenger.Broadcast(GameEvent.DANCE_PARTY);
        yield return new WaitForSeconds(2.0f);
        SetGameActive(false);
        ShowWinPopup();
    }

    public void ShowWinPopup() { winPopup.Open(); }
}

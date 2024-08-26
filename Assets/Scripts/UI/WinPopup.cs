using UnityEngine;

public class WinPopup : GameOverPopup
{
    [SerializeField] private SoundManager soundManager;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip winSound;

    // Start is called before the first frame update
    void Start()
    {
        audioSource.volume = 0f;
        soundManager.PlaySoundClip(winSound, transform, 0.5f);
    }
}
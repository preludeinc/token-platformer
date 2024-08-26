using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] SoundManager soundManager;
    [SerializeField] private AudioSource soundClipObject; 

    private void Awake()
    {
        if (soundManager == null)
        {
            soundManager = this;
        }
    }

    public void PlaySoundClip(AudioClip audio, Transform spawnObject, float soundVolume)
    {
        // spawns an audio game object
        AudioSource audioSrc = Instantiate(soundClipObject, 
            spawnObject.position, Quaternion.identity);

        // assigns audio
        audioSrc.clip = audio;

        // determines volume
        audioSrc.volume = soundVolume;
        audioSrc.Play();

        // clip-length
        float lengthClip = audioSrc.clip.length;
        Destroy(audioSrc.gameObject, lengthClip);
    }
}

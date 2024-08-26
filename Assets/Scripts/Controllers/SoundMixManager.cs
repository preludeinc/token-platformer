using UnityEngine;
using UnityEngine.Audio;

public class SoundMixManager : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMix;

    public float LogtoLinear(float value)
    {
        return Mathf.Log10(value) * 20;
    }

    public void SetVolumeMaster(float volume)
    {
        audioMix.SetFloat("masterVol", LogtoLinear(volume));
    }

    public void SetVolumeMusic(float volume)
    {
        audioMix.SetFloat("musicVol", LogtoLinear(volume));   
    }

    public void SetVolumeSoundEffects(float volume)
    {
        audioMix.SetFloat("soundFXVol", LogtoLinear(volume));
    }
}

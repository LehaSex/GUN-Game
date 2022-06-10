using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    [Header("Audio")]
    public float MaxVolume = 0.2f;
    public AudioClip MenuTheme;
    public AudioClip MatchTheme;

    [Header("UI")]
    public Button ToggleAudioButton;
    public Sprite MuteImage;
    public Sprite UnmuteImage;

    private AudioSource audioSource;
    
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        ToggleAudioButton.onClick.AddListener(ToggleMute);
    }

    private void OnDestroy()
    {
        ToggleAudioButton.onClick.RemoveListener(ToggleMute);
    }

    /// <summary>
    /// Играет музыку в главном меню
    /// </summary>
    public void PlayMenuTheme()
    {
        audioSource.Stop();
        audioSource.clip = MenuTheme;
        audioSource.Play();
    }

    /// <summary>
    /// Играет музыку в матче
    /// </summary>
    public void PlayMatchTheme()
    {
        audioSource.Stop();
        audioSource.clip = MatchTheme;
        audioSource.Play();
    }

    /// <summary>
    /// Мут или не мут музыки
    /// </summary>
    public void ToggleMute()
    {
        var isMaxVolume = audioSource.volume == MaxVolume;
        audioSource.volume = isMaxVolume ? 0 : MaxVolume;
        ToggleAudioButton.GetComponent<Image>().sprite = !isMaxVolume ? UnmuteImage : MuteImage;
    }
}

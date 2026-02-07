using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class RandomAudioClipPlayer : MonoBehaviour
{
    [SerializeField] private AudioClip[] _audioClips;

    private AudioSource _audioSource;

    void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    public void PlayRandomClip()
    {
        if (_audioClips.Length == 0) return;

        int randomIndex = Random.Range(0, _audioClips.Length);
        _audioSource.clip = _audioClips[randomIndex];
        _audioSource.Play();
    }
}

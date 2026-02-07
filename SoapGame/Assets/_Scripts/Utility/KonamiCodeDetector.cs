using UnityEngine;
using UnityEngine.InputSystem;

public class KonamiCodeDetector : MonoBehaviour
{
    [SerializeField]private AudioSource _audioSource;

    private Key[] konamiCode = new Key[]
    {
        Key.UpArrow,
        Key.UpArrow,
        Key.DownArrow,
        Key.DownArrow,
        Key.LeftArrow,
        Key.RightArrow,
        Key.LeftArrow,
        Key.RightArrow,
        Key.B,
        Key.A
    };

    private int currentIndex = 0;

     void Start()
    {
        if (_audioSource == null)
        {
            Debug.LogError("AudioSource component missing from KonamiCodeDetector GameObject.");
        }
    }

     void OnEnable()
    {
        if (_audioSource != null)
        {
            _audioSource.Play();
        }
    }

     void OnDisable()
    {
        if (_audioSource != null)
        {
            _audioSource.Stop();
        }
    }

    void Update()
    {
        if (Keyboard.current == null)
            return;

        CheckInput();
    }

    void CheckInput()
    {
        // Check all keys only when one was pressed this frame
        if (!Keyboard.current.anyKey.wasPressedThisFrame)
            return;

        if (Keyboard.current[konamiCode[currentIndex]].wasPressedThisFrame)
        {
            currentIndex++;

            if (currentIndex >= konamiCode.Length)
            {
                OnKonamiCodeEntered();
                currentIndex = 0;
            }
        }
        else
        {
            // Reset on wrong key
            currentIndex = 0;
        }
    }

    void OnKonamiCodeEntered()
    {
        GameManager.PlayerUpgradeManager.MaxAllUpgrades();
        GameManager.Shop.Refresh();
        if (_audioSource != null)
        {
            _audioSource.Play();
        }
    }
}

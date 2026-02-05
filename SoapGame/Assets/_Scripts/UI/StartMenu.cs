using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[System.Serializable]
public class MenuScreen
{
    public string Name;
    public CanvasGroup CanvasGroup;
}

public class StartMenu : MonoBehaviour
{
    [Header("Menu Screens")]
    [SerializeField] private MenuScreen[] menuScreens;
    private Dictionary<string, CanvasGroup> _screens;

    [Header("Buttons")]
    [SerializeField] private Button _startButton;
    [SerializeField] private Button _settingsButton;
    [SerializeField] private Button _creditsButton;
    [SerializeField] private Button _settingsBackButton;
    [SerializeField] private Button _creditsBackButton;

    [Header("Settings Menu")]
    [SerializeField] private Slider _musicVolumeSlider;
    [SerializeField] private Slider _sfxVolumeSlider;
    [SerializeField] private TextMeshProUGUI _musicVolumeText;
    [SerializeField] private TextMeshProUGUI _sfxVolumeText;

    void Awake()
    {
        _screens = new Dictionary<string, CanvasGroup>();
        foreach (var screen in menuScreens)
        {
            _screens[screen.Name] = screen.CanvasGroup;
        }

        _musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        _sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);

        _startButton.onClick.AddListener(() =>
        {
            SceneManager.LoadScene("MainScene");
        });

        _settingsButton.onClick.AddListener(() =>
        {
            ShowScreen("Settings");
        });
        
        _creditsButton.onClick.AddListener(() =>
        {
            ShowScreen("Credits");
        });

        _settingsBackButton.onClick.AddListener(() =>
        {
            ShowScreen("Main");
        });

        _creditsBackButton.onClick.AddListener(() =>
        {
            ShowScreen("Main");
        });

        ShowScreen("Main");
    }

    void OnDestroy()
    {
        _musicVolumeSlider.onValueChanged.RemoveListener(OnMusicVolumeChanged);
        _sfxVolumeSlider.onValueChanged.RemoveListener(OnSFXVolumeChanged);
        _startButton.onClick.RemoveAllListeners();
        _settingsButton.onClick.RemoveAllListeners();
        _creditsButton.onClick.RemoveAllListeners();
        _settingsBackButton.onClick.RemoveAllListeners();
        _creditsBackButton.onClick.RemoveAllListeners();
    }

    void Start()
    {
        _musicVolumeSlider.value = AudioManager.Instance.MusicVolume;
        _sfxVolumeSlider.value = AudioManager.Instance.SFXVolume;

        UpdateMusicVolumeText(AudioManager.Instance.MusicVolume);
        UpdateSFXVolumeText(AudioManager.Instance.SFXVolume);
    }

    private void ShowScreen(string screenName)
    {
        switch(screenName)
        {
            case "Main":
                DisableAllScreens();
                EnableScreen(_screens["Main"]);
                break;
            case "Settings":
                EnableScreen(_screens["Settings"]);
                break;
            case "Credits":
                EnableScreen(_screens["Credits"]);
                break;
        }
    }

    private void DisableAllScreens()
    {
        foreach (var screen in _screens.Values)
        {
            screen.alpha = 0f;
            screen.interactable = false;
            screen.blocksRaycasts = false;
        }
    }

    private void EnableScreen(CanvasGroup screen)
    {
        screen.alpha = 1f;
        screen.interactable = true;
        screen.blocksRaycasts = true;
    }

    public void OnMusicVolumeChanged(float value)
    {
        AudioManager.Instance.MusicVolume = value;
        UpdateMusicVolumeText(value);
    }

    public void OnSFXVolumeChanged(float value)
    {
        AudioManager.Instance.SFXVolume = value;
        UpdateSFXVolumeText(value);
    }

    private void UpdateMusicVolumeText(float value)
    {
        _musicVolumeText.text = $"{(int)(value * 100)}%";
    }

    private void UpdateSFXVolumeText(float value)
    {
        _sfxVolumeText.text = $"{(int)(value * 100)}%";
    }
}

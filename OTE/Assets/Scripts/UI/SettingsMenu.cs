using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Toggle fullscreenToggle;

    private SettingsData currentSettings;

    private void Start()
    {
        currentSettings = SettingsIO.LoadSettings();

        UpdateUI();

        ApplyGameSettings(currentSettings);
    }

    private void UpdateUI()
    {
        if (fullscreenToggle != null)
        {
            fullscreenToggle.onValueChanged.RemoveAllListeners();
            
            fullscreenToggle.isOn = currentSettings.isFullscreen;
            
            fullscreenToggle.onValueChanged.AddListener(SetFullscreen);
        }
    }


    public void SetFullscreen(bool isFullscreen)
    {
        currentSettings.isFullscreen = isFullscreen;

        SettingsIO.SaveSettings(currentSettings);

        ApplyGameSettings(currentSettings);
    }
    
    public static void ApplyGameSettings(SettingsData data)
    {
        if (data.isFullscreen)
        {
            Resolution nativeRes = Screen.currentResolution;
            Screen.SetResolution(nativeRes.width, nativeRes.height, FullScreenMode.FullScreenWindow);
        }
        else
        {
            Screen.SetResolution(1280, 720, FullScreenMode.Windowed);
        }

        // --- ЗВУК (Заготовка на будущее) ---
        // AudioListener.volume = data.volume;
    }
}
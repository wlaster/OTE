using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Управляет UI настроек: синхронизация переключателей с `SettingsData` и применение настроек.
/// </summary>
public class SettingsMenu : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Toggle fullscreenToggle;

    private SettingsData currentSettings;

    /// <summary>
    /// Загружает настройки и обновляет UI при старте сцены.
    /// </summary>
    private void Start()
    {
        currentSettings = SettingsIO.LoadSettings();

        UpdateUI();

        ApplyGameSettings(currentSettings);
    }

    /// <summary>
    /// Обновляет элементы UI в соответствии с текущими настройками и привязывает обработчики.
    /// </summary>
    private void UpdateUI()
    {
        if (fullscreenToggle != null)
        {
            fullscreenToggle.onValueChanged.RemoveAllListeners();

            fullscreenToggle.isOn = currentSettings.isFullscreen;

            fullscreenToggle.onValueChanged.AddListener(SetFullscreen);
        }
    }


    /// <summary>
    /// Устанавливает полноэкранный режим, сохраняет настройки и применяет их.
    /// </summary>
    public void SetFullscreen(bool isFullscreen)
    {
        currentSettings.isFullscreen = isFullscreen;

        SettingsIO.SaveSettings(currentSettings);

        ApplyGameSettings(currentSettings);
    }
    
    /// <summary>
    /// Применяет переданные настройки (в основном — режим экрана).
    /// </summary>
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
    }
}
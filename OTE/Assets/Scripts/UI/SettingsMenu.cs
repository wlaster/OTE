using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] private Toggle fullscreenToggle;

    private void Start()
    {
        LoadSettings();
    }

    private void LoadSettings()
    {
        // 1 = Fullscreen Window (безрамочный), 0 = обычное окно
        bool isFullscreen = PlayerPrefs.GetInt("IsFullscreen", 1) == 1;

        if (fullscreenToggle != null)
        {
            fullscreenToggle.isOn = isFullscreen;
            fullscreenToggle.onValueChanged.RemoveAllListeners(); // на всякий случай очищаем
            fullscreenToggle.onValueChanged.AddListener(SetFullscreen);
        }

        ApplyFullscreen(isFullscreen);
    }

    // Вызывается при изменении Toggle
    public void SetFullscreen(bool isFullscreen)
    {
        ApplyFullscreen(isFullscreen);

        PlayerPrefs.SetInt("IsFullscreen", isFullscreen ? 1 : 0);
        PlayerPrefs.Save();
    }

    private void ApplyFullscreen(bool isFullscreen)
    {
        if (isFullscreen)
        {
            Resolution nativeRes = Screen.currentResolution;
            Screen.SetResolution(nativeRes.width, nativeRes.height, FullScreenMode.FullScreenWindow);
        }
        else
        {
            // Обычный оконный режим
            Screen.fullScreenMode = FullScreenMode.Windowed;
        }
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
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
        // Загружаем сохраненное значение. Если его нет, по умолчанию будет 1 (true).
        bool isFullscreen = PlayerPrefs.GetInt("IsFullscreen", 1) == 1;
        
        if (fullscreenToggle != null)
        {
            fullscreenToggle.isOn = isFullscreen;
        }
        
        // Применяем настройку при запуске
        Screen.fullScreen = isFullscreen;
    }

    // Этот метод будет вызываться при изменении состояния Toggle
    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        
        // Сохраняем настройку. 1 для true, 0 для false.
        PlayerPrefs.SetInt("IsFullscreen", isFullscreen ? 1 : 0);
        PlayerPrefs.Save(); // Немедленно записываем на диск
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button continueButton;

    [Header("Panels")]
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private GameObject settingsPanel;

    [Header("Scene Names")]
    [SerializeField] private string newGameSceneName = "Level1_Prototype";

    private void Start()
    {
        // --- ВОТ КАК ТЕПЕРЬ ВЫГЛЯДИТ ПРИМЕНЕНИЕ ---
        // 1. Загружаем данные
        SettingsData savedData = SettingsIO.LoadSettings();
        
        // 2. Вызываем статический метод из соседнего скрипта.
        // Нам НЕ НУЖНА ссылка на SettingsMenu, и неважно, включена ли панель.
        SettingsMenu.ApplyGameSettings(savedData);
        // ------------------------------------------

        CheckForSaveFile();
        ShowMainPanel();
    }

    private void ApplySavedSettings()
    {
        SettingsData data = SettingsIO.LoadSettings();
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

    private void CheckForSaveFile()
    {
        bool saveFileExists = PlayerPrefs.GetInt("SaveFileExists", 0) == 1;
        if (continueButton != null) continueButton.interactable = saveFileExists;
    }

    // --- ЛОГИКА ПЕРЕКЛЮЧЕНИЯ ПАНЕЛЕЙ ---

    public void OpenSettings()
    {
        mainPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    public void CloseSettings() // Это для кнопки "Назад"
    {
        settingsPanel.SetActive(false);
        mainPanel.SetActive(true);
    }

    private void ShowMainPanel()
    {
        mainPanel.SetActive(true);
        settingsPanel.SetActive(false);
    }

    // --- ИГРОВЫЕ МЕТОДЫ ---

    public void NewGame()
    {
        SaveSystem.DeleteSaveFile();
        SceneManager.LoadScene(newGameSceneName);
    }

    public void ContinueGame()
    {
        // Создаем временный объект для загрузки
        GameObject tempManager = new GameObject("TempGameManager");
        tempManager.AddComponent<GameManager>().LoadGame();
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
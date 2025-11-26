using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Управляет главным меню: переключение панелей, создание новой игры и продолжение сохранения.
/// </summary>
public class MainMenuManager : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button continueButton;

    [Header("Panels")]
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private GameObject settingsPanel;

    [Header("Scene Names")]
    [SerializeField] private string newGameSceneName = "Level1_Prototype";

    /// <summary>
    /// Инициализация меню: применяет сохранённые настройки и проверяет наличие сохранения.
    /// </summary>
    private void Start()
    {
        SettingsData savedData = SettingsIO.LoadSettings();
        SettingsMenu.ApplyGameSettings(savedData);

        CheckForSaveFile();
        ShowMainPanel();
    }

    /// <summary>
    /// Применяет настройки экрана из сохранений (широко экрана/окно).
    /// </summary>
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

    /// <summary>
    /// Включает/отключает кнопку "Продолжить" в зависимости от наличия файла сохранения.
    /// </summary>
    private void CheckForSaveFile()
    {
        bool saveFileExists = PlayerPrefs.GetInt("SaveFileExists", 0) == 1;
        if (continueButton != null) continueButton.interactable = saveFileExists;
    }

    

    /// <summary>
    /// Открывает панель настроек, скрывая главное меню.
    /// </summary>
    public void OpenSettings()
    {
        mainPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    /// <summary>
    /// Закрывает панель настроек и возвращает главное меню.
    /// </summary>
    public void CloseSettings() 
    {
        settingsPanel.SetActive(false);
        mainPanel.SetActive(true);
    }

    /// <summary>
    /// Отображает главное меню и скрывает настройки.
    /// </summary>
    private void ShowMainPanel()
    {
        mainPanel.SetActive(true);
        settingsPanel.SetActive(false);
    }

    

    /// <summary>
    /// Начинает новую игру: удаляет старое сохранение и загружает сцену новой игры.
    /// </summary>
    public void NewGame()
    {
        SaveSystem.DeleteSaveFile();
        SceneManager.LoadScene(newGameSceneName);
    }

    /// <summary>
    /// Продолжает игру из сохранения, создавая временный `GameManager` для загрузки данных.
    /// </summary>
    public void ContinueGame()
    {
        GameObject tempManager = new GameObject("TempGameManager");
        tempManager.AddComponent<GameManager>().LoadGame();
    }

    /// <summary>
    /// Выходит из приложения.
    /// </summary>
    public void QuitGame()
    {
        Application.Quit();
    }
}